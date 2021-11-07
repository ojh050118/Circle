using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace Circle.Game.Tests.Visual
{
    public class TestSceneMainScreen : CircleTestScene
    {
        public TestSceneMainScreen()
        {
            Add(new ScreenStack(new MainScreen()) { RelativeSizeAxes = Axes.Both });
        }
    }
}
