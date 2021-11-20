using System.Globalization;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics;
using osuTK.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Effects;
using osu.Framework.Extensions.Color4Extensions;
using osuTK;

namespace Circle.Game.Rulesets.Objects
{
    public class CircularTile : Tile
    {
        public CircularTile(float angle)
            : base(angle)
        {
            Size = new Vector2(HEIGHT);
            Child = new CircularContainer
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                    },
                    new SpriteText
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Text = angle.ToString(CultureInfo.CurrentCulture),
                        Colour = Color4.Black
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
