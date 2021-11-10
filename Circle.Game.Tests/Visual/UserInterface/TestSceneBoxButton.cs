using Circle.Game.Graphics.UserInterface;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;

namespace Circle.Game.Tests.Visual.UserInterface
{
    public class TestSceneBoxButton : CircleTestScene
    {
        public TestSceneBoxButton()
        {
            Add(new Box { RelativeSizeAxes = Axes.Both, Colour = Color4.DarkGray });
            Add(new BoxButton
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Text = "Hover this!",
            });
            Add(new BoxButton(false)
            {
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Text = "Click this!"
            });
        }
    }
}
