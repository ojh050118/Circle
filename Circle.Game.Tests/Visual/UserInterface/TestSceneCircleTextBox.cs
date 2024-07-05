using Circle.Game.Graphics.UserInterface;
using osu.Framework.Graphics;
using osuTK;

namespace Circle.Game.Tests.Visual.UserInterface
{
    public partial class TestSceneCircleTextBox : CircleTestScene
    {
        public TestSceneCircleTextBox()
        {
            Add(new CircleTextBox
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(400, 40),
                PlaceholderText = "Placeholder"
            });
        }
    }
}
