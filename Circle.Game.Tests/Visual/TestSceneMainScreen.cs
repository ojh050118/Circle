#nullable disable

using Circle.Game.Screens;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace Circle.Game.Tests.Visual
{
    public partial class TestSceneMainScreen : CircleTestScene
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            Add(new ScreenStack(new MainScreen()) { RelativeSizeAxes = Axes.Both });
        }
    }
}
