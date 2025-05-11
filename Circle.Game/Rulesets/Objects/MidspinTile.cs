#nullable disable

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace Circle.Game.Rulesets.Objects
{
    public partial class MidspinTile : DrawableTile
    {
        public MidspinTile()
        {
            Size = new Vector2(50);
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
