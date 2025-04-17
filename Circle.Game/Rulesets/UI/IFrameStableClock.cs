#nullable disable

using Circle.Game.Screens.Play;
using osu.Framework.Bindables;

namespace Circle.Game.Rulesets.UI
{
    public interface IFrameStableClock : IGameplayClock
    {
        IBindable<bool> IsCatchingUp { get; }

        IBindable<bool> WaitingOnFrames { get; }
    }
}
