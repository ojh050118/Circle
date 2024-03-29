#nullable disable

using Circle.Game.Rulesets.Objects;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;

namespace Circle.Game.Tests.Visual.Objects
{
    public partial class TestSceneMidspinTile : CircleTestScene
    {
        public TestSceneMidspinTile()
        {
            Add(new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Color4.Black
            });
            Add(new MidspinTile
            {
                Anchor = Anchor.Centre,
            });
        }
    }
}
