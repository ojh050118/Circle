using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Rulesets.Objects
{
    public class MidspinTile : Tile
    {
        public MidspinTile(float angle)
            : base(TileType.Midspin, angle)
        {
            Child = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Masking = true,
                Child = new Container
                {
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    Size = new Vector2(HEIGHT),
                    Masking = true,
                    Child = new EquilateralTriangle
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                        Rotation = 90,
                    },
                },
            };
        }
    }
}
