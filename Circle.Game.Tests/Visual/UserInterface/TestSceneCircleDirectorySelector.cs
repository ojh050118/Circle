using Circle.Game.Graphics.UserInterface;
using osu.Framework.Graphics;

namespace Circle.Game.Tests.Visual.UserInterface
{
    public class TestSceneCircleDirectorySelector : CircleTestScene
    {
        public TestSceneCircleDirectorySelector()
        {
            Add(new CircleDirectorySelector { RelativeSizeAxes = Axes.Both });
        }
    }
}
