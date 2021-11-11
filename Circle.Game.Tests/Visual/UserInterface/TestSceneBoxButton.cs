using Circle.Game.Graphics.UserInterface;
using osu.Framework.Graphics;

namespace Circle.Game.Tests.Visual.UserInterface
{
    public class TestSceneBoxButton : CircleTestScene
    {
        public TestSceneBoxButton()
        {
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
