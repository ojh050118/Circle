using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;

namespace Circle.Game.Rulesets.Objects
{
    public class ShortTile : Tile
    {
        public ShortTile()
            : base(TileType.Short)
        {
            Child = new CircularContainer
            {
                RelativeSizeAxes = Axes.Both,
                Width = 0.6f,
                Masking = true,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                    }
                },
                EdgeEffect = new EdgeEffectParameters
                {
                    Type = EdgeEffectType.Shadow,
                    Radius = 5,
                    Colour = Color4.Black.Opacity(0.5f),
                }
            };
        }
    }
}
