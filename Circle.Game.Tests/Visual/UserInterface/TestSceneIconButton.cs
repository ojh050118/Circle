#nullable disable

using Circle.Game.Graphics.UserInterface;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace Circle.Game.Tests.Visual.UserInterface
{
    public partial class TestSceneIconButton : CircleTestScene
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
