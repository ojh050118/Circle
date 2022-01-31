using osu.Framework.Bindables;
using osu.Framework.Timing;

namespace Circle.Game.Rulesets.UI
{
    public interface IFrameStableClock : IFrameBasedClock
    {
        IBindable<bool> IsCatchingUp { get; }

        IBindable<bool> WaitingOnFrames { get; }
    }
}
