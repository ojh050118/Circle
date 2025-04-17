using System;
using System.Diagnostics;
using Circle.Game.Screens.Play;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Timing;

namespace Circle.Game.Rulesets.UI
{
    [Cached(typeof(IGameplayClock))]
    [Cached(typeof(IFrameStableClock))]
    public partial class FrameStabilityContainer : Container, IFrameStableClock
    {
        private const double max_catchup_milliseconds = 10;

        public bool AllowBackwardsSeeks { get; set; }

        internal bool FrameStablePlayback { get; set; } = true;

        private readonly Bindable<bool> isCatchingUp = new Bindable<bool>();
        private readonly Bindable<bool> waitingOnFrames = new Bindable<bool>();

        public double GameplayStartTime { get; }

        private IGameplayClock? parentGameplayClock;
        private IClock referenceClock = null!;

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

        [BackgroundDependencyLoader(true)]
        private void load(IGameplayClock? gameplayClock)
        {
            if (gameplayClock != null)
            {
                parentGameplayClock = gameplayClock;
                IsPaused.BindTo(parentGameplayClock.IsPaused);
            }

            referenceClock = gameplayClock ?? Clock;
            Clock = this;
        }

        public override bool UpdateSubTree()
        {
            stopwatch.Restart();

            do
            {
                // update clock is always trying to approach the aim time.
                // it should be provided as the original value each loop.
                updateClock();

                // 게임을 시작하기 전에는 일시정지 상태로 업데이트가 차단되어 드로어블이 보이지 않습니다.
                // 완전히 로드 되기 전에 한번 업데이트를 하여 드로어블이 씬 그래프에 포함되도록 합니다.
                if (state == PlaybackState.NotValid && LoadState > LoadState.Ready)
                    break;

                base.UpdateSubTree();
                UpdateSubTreeMasking();
            } while (state == PlaybackState.RequiresCatchUp && stopwatch.ElapsedMilliseconds <= max_catchup_milliseconds);

            return true;
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
    }
}
