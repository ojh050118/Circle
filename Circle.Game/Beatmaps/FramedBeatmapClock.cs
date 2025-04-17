using System.Diagnostics;
using Circle.Game.Configuration;
using Circle.Game.Screens.Play;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Timing;

namespace Circle.Game.Beatmaps
{
    public partial class FramedBeatmapClock : Component, IFrameBasedClock, IAdjustableClock, ISourceChangeableClock
    {
        public double TotalAppliedOffset
        {
            get
            {
                if (!applyOffsets)
                    return 0;

                Debug.Assert(userGlobalOffsetClock != null);
                Debug.Assert(userBeatmapOffsetClock != null);

                return userGlobalOffsetClock.RateAdjustedOffset + userBeatmapOffsetClock.Offset;
            }
        }

        public bool IsRewinding { get; private set; }
        private readonly bool applyOffsets;

        private readonly OffsetCorrectionClock? userGlobalOffsetClock;
        private readonly FramedOffsetClock? userBeatmapOffsetClock;

        private readonly IFrameBasedClock finalClockSource;

        private readonly DecouplingFramedClock decoupledTrack;
        private readonly InterpolatingFramedClock interpolatedTrack;

        private Bindable<double>? userAudioOffset;

        [Resolved]
        private CircleConfigManager config { get; set; } = null!;

        public FramedBeatmapClock(bool applyOffsets, bool requireDecoupling, IClock? source = null)
        {
            this.applyOffsets = applyOffsets;

            decoupledTrack = new DecouplingFramedClock(source) { AllowDecoupling = requireDecoupling };

            // An interpolating clock is used to ensure precise time values even when the host audio subsystem is not reporting
            // high precision times (on windows there's generally only 5-10ms reporting intervals, as an example).
            interpolatedTrack = new InterpolatingFramedClock(decoupledTrack);

            if (applyOffsets)
            {
                // User global offset (set in settings) should also be applied.
                userGlobalOffsetClock = new OffsetCorrectionClock(interpolatedTrack);

                // User per-beatmap offset will be applied to this final clock.
                finalClockSource = userBeatmapOffsetClock = new FramedOffsetClock(userGlobalOffsetClock);
            }
            else
            {
                finalClockSource = interpolatedTrack;
            }
        }

        public string GetSnapshot()
        {
            return
                $"originalSource: {output(Source)}\n" +
                $"userGlobalOffsetClock: {output(userGlobalOffsetClock)}\n" +
                $"userBeatmapOffsetClock: {output(userBeatmapOffsetClock)}\n" +
                $"interpolatedTrack: {output(interpolatedTrack)}\n" +
                $"decoupledTrack: {output(decoupledTrack)}\n" +
                $"finalClockSource: {output(finalClockSource)}\n";

            string output(IClock? clock)
            {
                if (clock == null)
                    return "null";

                if (clock is IFrameBasedClock framed)
                    return $"current: {clock.CurrentTime:N2} running: {clock.IsRunning} rate: {clock.Rate} elapsed: {framed.ElapsedFrameTime:N2}";

                return $"current: {clock.CurrentTime:N2} running: {clock.IsRunning} rate: {clock.Rate}";
            }
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            if (applyOffsets)
            {
                Debug.Assert(userBeatmapOffsetClock != null);
                Debug.Assert(userGlobalOffsetClock != null);

                userAudioOffset = config.GetBindable<double>(CircleSetting.Offset);
                userAudioOffset.BindValueChanged(offset => userGlobalOffsetClock.Offset = offset.NewValue, true);
            }
        }

        protected override void Update()
        {
            base.Update();

            finalClockSource.ProcessFrame();

            if (Clock.ElapsedFrameTime != 0)
                IsRewinding = Clock.ElapsedFrameTime < 0;
        }

        #region Delegation of IAdjustableClock / ISourceChangeableClock to decoupled clock.

        public void ChangeSource(IClock? source) => decoupledTrack.ChangeSource(source);

        public IClock Source => decoupledTrack.Source;

        public void Reset()
        {
            decoupledTrack.Reset();
            finalClockSource.ProcessFrame();
        }

        public void Start()
        {
            decoupledTrack.Start();
            finalClockSource.ProcessFrame();
        }

        public void Stop()
        {
            decoupledTrack.Stop();
            finalClockSource.ProcessFrame();
        }

        public bool Seek(double position)
        {
            bool success = decoupledTrack.Seek(position - TotalAppliedOffset);
            finalClockSource.ProcessFrame();

            return success;
        }

        public void ResetSpeedAdjustments() => decoupledTrack.ResetSpeedAdjustments();

        public double Rate
        {
            get => decoupledTrack.Rate;
            set => decoupledTrack.Rate = value;
        }

        #endregion

        #region Delegation of IFrameBasedClock to clock with all offsets applied

        public double CurrentTime => finalClockSource.CurrentTime;

        public bool IsRunning => finalClockSource.IsRunning;

        public void ProcessFrame()
        {
            // Noop to ensure an external consumer doesn't process the internal clock an extra time.
        }

        public double ElapsedFrameTime => finalClockSource.ElapsedFrameTime;

        public double FramesPerSecond => finalClockSource.FramesPerSecond;

        #endregion
    }
}
