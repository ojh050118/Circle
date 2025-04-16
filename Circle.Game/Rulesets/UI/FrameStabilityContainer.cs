using System;
using System.Diagnostics;
using Circle.Game.Screens.Play;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;
using osu.Framework.Timing;

namespace Circle.Game.Rulesets.UI
{
    public partial class FrameStabilityContainer : Container, IFrameStableClock
    {
        public double GameplayStartTime { get; }
        public bool AllowBackwardsSeeks { get; set; }

        internal bool FrameStablePlayback { get; set; } = true;

        protected override bool RequiresChildrenUpdate => base.RequiresChildrenUpdate && state != PlaybackState.NotValid;

        private const double max_catchup_milliseconds = 10;

        private readonly Bindable<bool> isCatchingUp = new Bindable<bool>();

        private readonly Bindable<bool> waitingOnFrames = new Bindable<bool>();

        /// <summary>
        /// A local manual clock which tracks the reference clock.
        /// Values are transferred from <see cref="referenceClock"/> each update call.
        /// </summary>
        private readonly ManualClock manualClock;

        /// <summary>
        /// The main framed clock which has stability applied to it.
        /// This gets exposed to children as an <see cref="IGameplayClock"/>.
        /// </summary>
        private readonly FramedClock framedClock;

        private readonly Stopwatch stopwatch = new Stopwatch();
        private double? lastBackwardsSeekLogTime;

        private IGameplayClock? parentGameplayClock;

        /// <summary>
        /// A clock which is used as reference for time, rate and running state.
        /// </summary>
        private IClock referenceClock = null!;

        /// <summary>
        /// The current direction of playback to be exposed to frame stable children.
        /// </summary>
        /// <remarks>
        /// Initially it is presumed that playback will proceed in the forward direction.
        /// </remarks>
        private int direction = 1;

        private PlaybackState state;

        private bool firstConsumption = true;

        public FrameStabilityContainer(double gameplayStartTime = double.MinValue)
        {
            RelativeSizeAxes = Axes.Both;

            framedClock = new FramedClock(manualClock = new ManualClock());

            GameplayStartTime = gameplayStartTime;
        }

        public override bool UpdateSubTree()
        {
            stopwatch.Restart();

            do
            {
                // update clock is always trying to approach the aim time.
                // it should be provided as the original value each loop.
                updateClock();

                if (state == PlaybackState.NotValid)
                    break;

                base.UpdateSubTree();
                UpdateSubTreeMasking();
            } while (state == PlaybackState.RequiresCatchUp && stopwatch.ElapsedMilliseconds <= max_catchup_milliseconds);

            return true;
        }

        [BackgroundDependencyLoader(true)]
        private void load(IGameplayClock? clock)
        {
            if (clock != null)
            {
                parentGameplayClock = clock;
                IsPaused.BindTo(parentGameplayClock.IsPaused);
            }

            referenceClock = clock ?? Clock;
        }

        private void updateClock()
        {
            if (waitingOnFrames.Value)
            {
                // if waiting on frames, run one update loop to determine if frames have arrived.
                state = PlaybackState.Valid;
            }
            else if (IsPaused.Value)
            {
                // time should not advance while paused, nor should anything run.
                state = PlaybackState.NotValid;
                return;
            }
            else
            {
                state = PlaybackState.Valid;
            }

            double proposedTime = referenceClock.CurrentTime;

            if (FrameStablePlayback)
                // if we require frame stability, the proposed time will be adjusted to move at most one known
                // frame interval in the current direction.
                applyFrameStability(ref proposedTime);

            if (FrameStablePlayback && proposedTime > referenceClock.CurrentTime && !AllowBackwardsSeeks)
            {
                if (lastBackwardsSeekLogTime == null || Math.Abs(Clock.CurrentTime - lastBackwardsSeekLogTime.Value) > 1000)
                {
                    lastBackwardsSeekLogTime = Clock.CurrentTime;
                    Logger.Log($"Denying backwards seek during gameplay (reference: {referenceClock.CurrentTime:N2} stable: {proposedTime:N2})");
                }

                state = PlaybackState.NotValid;
                return;
            }

            // if the proposed time is the same as the current time, assume that the clock will continue progressing in the same direction as previously.
            // this avoids spurious flips in direction from -1 to 1 during rewinds.
            if (state == PlaybackState.Valid && proposedTime != manualClock.CurrentTime)
                direction = proposedTime >= manualClock.CurrentTime ? 1 : -1;

            double timeBehind = Math.Abs(proposedTime - referenceClock.CurrentTime);

            isCatchingUp.Value = timeBehind > 200;
            waitingOnFrames.Value = false;

            manualClock.CurrentTime = proposedTime;
            manualClock.Rate = Math.Abs(referenceClock.Rate) * direction;
            manualClock.IsRunning = referenceClock.IsRunning;

            // determine whether catch-up is required.
            if (state == PlaybackState.Valid && timeBehind > 0)
                state = PlaybackState.RequiresCatchUp;

            // The manual clock time has changed in the above code. The framed clock now needs to be updated
            // to ensure that the its time is valid for our children before input is processed
            framedClock.ProcessFrame();

            if (framedClock.ElapsedFrameTime != 0)
                IsRewinding = framedClock.ElapsedFrameTime < 0;
        }

        /// <summary>
        /// Apply frame stability modifier to a time.
        /// </summary>
        /// <param name="proposedTime">The time which is to be displayed.</param>
        private void applyFrameStability(ref double proposedTime)
        {
            const double sixty_frame_time = 1000.0 / 60;

            if (firstConsumption)
            {
                // On the first update, frame-stability seeking would result in unexpected/unwanted behaviour.
                // Instead we perform an initial seek to the proposed time.

                // process frame (in addition to finally clause) to clear out ElapsedTime
                manualClock.CurrentTime = proposedTime;
                framedClock.ProcessFrame();

                firstConsumption = false;
                return;
            }

            if (manualClock.CurrentTime < GameplayStartTime)
                manualClock.CurrentTime = proposedTime = Math.Min(GameplayStartTime, proposedTime);
            else if (Math.Abs(manualClock.CurrentTime - proposedTime) > sixty_frame_time * 1.2f)
            {
                proposedTime = proposedTime > manualClock.CurrentTime
                    ? Math.Min(proposedTime, manualClock.CurrentTime + sixty_frame_time)
                    : Math.Max(proposedTime, manualClock.CurrentTime - sixty_frame_time);
            }
        }

        #region Delegation of IGameplayClock

        IBindable<bool> IFrameStableClock.IsCatchingUp => isCatchingUp;
        IBindable<bool> IFrameStableClock.WaitingOnFrames => waitingOnFrames;

        public IBindable<bool> IsPaused { get; } = new BindableBool();

        public bool IsRewinding { get; private set; }

        public double CurrentTime => framedClock.CurrentTime;

        public double Rate => framedClock.Rate;

        public bool IsRunning => framedClock.IsRunning;

        public void ProcessFrame() { }

        public double ElapsedFrameTime => framedClock.ElapsedFrameTime;

        public double FramesPerSecond => framedClock.FramesPerSecond;

        public double StartTime => parentGameplayClock?.StartTime ?? 0;

        private readonly AudioAdjustments gameplayAdjustments = new AudioAdjustments();

        #endregion

        private enum PlaybackState
        {
            /// <summary>
            /// Playback is not possible. Child hierarchy should not be processed.
            /// </summary>
            NotValid,

            /// <summary>
            /// Playback is running behind real-time. Catch-up will be attempted by processing more than once per
            /// game loop (limited to a sane maximum to avoid frame drops).
            /// </summary>
            RequiresCatchUp,

            /// <summary>
            /// In a valid state, progressing one child hierarchy loop per game loop.
            /// </summary>
            Valid
        }

        // private class FrameStabilityClock : GameplayClock, IFrameStableClock
        // {
        //     public readonly Bindable<bool> IsCatchingUp = new Bindable<bool>();
        //
        //     public readonly Bindable<bool> WaitingOnFrames = new Bindable<bool>();
        //
        //     IBindable<bool> IFrameStableClock.IsCatchingUp => IsCatchingUp;
        //
        //     IBindable<bool> IFrameStableClock.WaitingOnFrames => WaitingOnFrames;
        //
        //     public FrameStabilityClock(FramedClock underlyingClock)
        //         : base(underlyingClock)
        //     {
        //     }
        // }
    }
}
