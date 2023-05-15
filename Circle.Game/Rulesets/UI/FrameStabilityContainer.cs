#nullable disable

using System;
using System.Diagnostics;
using Circle.Game.Screens.Play;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Timing;

namespace Circle.Game.Rulesets.UI
{
    public partial class FrameStabilityContainer : Container
    {
        public IFrameStableClock FrameStableClock => frameStableClock;

        public int MaxCatchUpFrames { get; set; } = 5;

        internal bool FrameStablePlayback = true;

        protected override bool RequiresChildrenUpdate => base.RequiresChildrenUpdate && state != PlaybackState.NotValid;
        private const double sixty_frame_time = 1000.0 / 60;

        private readonly FramedClock framedClock;

        [Cached(typeof(GameplayClock))]
        private readonly FrameStabilityClock frameStableClock;

        private readonly double gameplayStartTime;

        private readonly ManualClock manualClock;

        /// <summary>
        /// The current direction of playback to be exposed to frame stable children.
        /// </summary>
        /// <remarks>
        /// Initially it is presumed that playback will proceed in the forward direction.
        /// </remarks>
        private int direction = 1;

        private bool firstConsumption = true;

        private IFrameBasedClock parentGameplayClock;

        private PlaybackState state;

        public FrameStabilityContainer(double gameplayStartTime = double.MinValue)
        {
            RelativeSizeAxes = Axes.Both;

            frameStableClock = new FrameStabilityClock(framedClock = new FramedClock(manualClock = new ManualClock()));

            this.gameplayStartTime = gameplayStartTime;
        }

        public override bool UpdateSubTree()
        {
            int loops = MaxCatchUpFrames;

            do
            {
                // update clock is always trying to approach the aim time.
                // it should be provided as the original value each loop.
                updateClock();

                if (state == PlaybackState.NotValid)
                    break;

                base.UpdateSubTree();
                UpdateSubTreeMasking(this, ScreenSpaceDrawQuad.AABBFloat);
            } while (state == PlaybackState.RequiresCatchUp && loops-- > 0);

            return true;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            setClock();
        }

        [BackgroundDependencyLoader(true)]
        private void load(GameplayClock clock)
        {
            if (clock != null)
            {
                parentGameplayClock = clock;
                frameStableClock.IsPaused.BindTo(clock.IsPaused);
            }
        }

        private void updateClock()
        {
            if (frameStableClock.WaitingOnFrames.Value)
            {
                // if waiting on frames, run one update loop to determine if frames have arrived.
                state = PlaybackState.Valid;
            }
            else if (frameStableClock.IsPaused.Value)
            {
                // time should not advance while paused, nor should anything run.
                state = PlaybackState.NotValid;
                return;
            }
            else
            {
                state = PlaybackState.Valid;
            }

            if (parentGameplayClock == null)
                setClock(); // LoadComplete may not be run yet, but we still want the clock.

            Debug.Assert(parentGameplayClock != null);

            double proposedTime = parentGameplayClock.CurrentTime;

            if (FrameStablePlayback)
                // if we require frame stability, the proposed time will be adjusted to move at most one known
                // frame interval in the current direction.
                applyFrameStability(ref proposedTime);

            // if the proposed time is the same as the current time, assume that the clock will continue progressing in the same direction as previously.
            // this avoids spurious flips in direction from -1 to 1 during rewinds.
            if (state == PlaybackState.Valid && proposedTime != manualClock.CurrentTime)
                direction = proposedTime >= manualClock.CurrentTime ? 1 : -1;

            double timeBehind = Math.Abs(proposedTime - parentGameplayClock.CurrentTime);

            frameStableClock.IsCatchingUp.Value = timeBehind > 200;
            frameStableClock.WaitingOnFrames.Value = state == PlaybackState.NotValid;

            manualClock.CurrentTime = proposedTime;
            manualClock.Rate = Math.Abs(parentGameplayClock.Rate) * direction;
            manualClock.IsRunning = parentGameplayClock.IsRunning;

            // determine whether catch-up is required.
            if (state == PlaybackState.Valid && timeBehind > 0)
                state = PlaybackState.RequiresCatchUp;

            // The manual clock time has changed in the above code. The framed clock now needs to be updated
            // to ensure that the its time is valid for our children before input is processed
            framedClock.ProcessFrame();
        }

        /// <summary>
        /// Apply frame stability modifier to a time.
        /// </summary>
        /// <param name="proposedTime">The time which is to be displayed.</param>
        private void applyFrameStability(ref double proposedTime)
        {
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

            if (manualClock.CurrentTime < gameplayStartTime)
                manualClock.CurrentTime = proposedTime = Math.Min(gameplayStartTime, proposedTime);
            else if (Math.Abs(manualClock.CurrentTime - proposedTime) > sixty_frame_time * 1.2f)
            {
                proposedTime = proposedTime > manualClock.CurrentTime
                    ? Math.Min(proposedTime, manualClock.CurrentTime + sixty_frame_time)
                    : Math.Max(proposedTime, manualClock.CurrentTime - sixty_frame_time);
            }
        }

        private void setClock()
        {
            if (parentGameplayClock == null)
            {
                // in case a parent gameplay clock isn't available, just use the parent clock.
                parentGameplayClock ??= Clock;
            }
            else
            {
                Clock = frameStableClock;
            }
        }

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

        private class FrameStabilityClock : GameplayClock, IFrameStableClock
        {
            public readonly Bindable<bool> IsCatchingUp = new Bindable<bool>();

            public readonly Bindable<bool> WaitingOnFrames = new Bindable<bool>();

            IBindable<bool> IFrameStableClock.IsCatchingUp => IsCatchingUp;

            IBindable<bool> IFrameStableClock.WaitingOnFrames => WaitingOnFrames;

            public FrameStabilityClock(FramedClock underlyingClock)
                : base(underlyingClock)
            {
            }
        }
    }
}
