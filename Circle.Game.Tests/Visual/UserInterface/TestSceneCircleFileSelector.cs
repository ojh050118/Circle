#nullable disable

using Circle.Game.Graphics.UserInterface;
using NUnit.Framework;
using osu.Framework.Graphics;

namespace Circle.Game.Tests.Visual.UserInterface
{
    public class TestSceneCircleFileSelector : CircleTestScene
    {
        [Test]
        public void TestAllFiles()
        {
            AddStep("Create", () => Child = new CircleFileSelector { RelativeSizeAxes = Axes.Both });
        }

        [Test]
        public void TestJpgFilesOnly()
        {
            AddStep("Create", () => Child = new CircleFileSelector(validFileExtensions: new[] { ".jpg" }) { RelativeSizeAxes = Axes.Both });
        }
    }
}
