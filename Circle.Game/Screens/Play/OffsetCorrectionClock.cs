using osu.Framework.Timing;

namespace Circle.Game.Screens.Play
{
    public class OffsetCorrectionClock : FramedOffsetClock
    {
        public double RateAdjustedOffset => base.Offset;

        public new double Offset
        {
            get => offset;
            set
            {
                if (value == offset)
                    return;

                offset = value;

                updateOffset();
            }
        }

        private double offset;

        public OffsetCorrectionClock(IClock source)
            : base(source)
        {
        }

        public override void ProcessFrame()
        {
            base.ProcessFrame();
            updateOffset();
        }

        private void updateOffset()
        {
            // we always want to apply the same real-time offset, so it should be adjusted by the difference in playback rate (from realtime) to achieve this.
            base.Offset = Offset * Rate;
        }
    }
}
