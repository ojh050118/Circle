#nullable disable

using osu.Framework.Bindables;
using osu.Framework.Timing;

namespace Circle.Game.Screens.Play
{
    public class GameplayClock : IFrameBasedClock
    {
        public readonly BindableBool IsPaused = new BindableBool();
        internal readonly IFrameBasedClock UnderlyingClock;

        public GameplayClock(IFrameBasedClock underlyingClock)
        {
            UnderlyingClock = underlyingClock;
        }

        public double CurrentTime => UnderlyingClock.CurrentTime;

        public double Rate => UnderlyingClock.Rate;

        public double ElapsedFrameTime => UnderlyingClock.ElapsedFrameTime;

        public double FramesPerSecond => UnderlyingClock.FramesPerSecond;

        public FrameTimeInfo TimeInfo => UnderlyingClock.TimeInfo;

        public bool IsRunning => UnderlyingClock.IsRunning;

        public void ProcessFrame()
        {
        }
    }
}
