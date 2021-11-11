using Circle.Game.Graphics.UserInterface;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK.Graphics;

namespace Circle.Game.Tests.Visual.UserInterface
{
    public class TestSceneIconButton : CircleTestScene
    {
        public TestSceneIconButton()
        {
            Add(new IconButton
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Icon = FontAwesome.Solid.Circle
            });
            Add(new IconButton(false)
            {
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Icon = FontAwesome.Solid.CircleNotch
            });
        }
    }
}
