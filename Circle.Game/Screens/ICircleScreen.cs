using osu.Framework.Screens;

namespace Circle.Game.Screens
{
    public interface ICircleScreen : IScreen
    {
        bool BlockExit { get; }

        string Header { get; }
    }
}
