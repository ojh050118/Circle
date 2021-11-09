using System;
using Circle.Game.Graphics.UserInterface;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Screens
{
    public class ScreenHeader : CompositeDrawable
    {
        public const int MARGIN = 60;

        public float DrawnHeight { get; }

        public ScreenHeader(string name)
        {
            Margin = new MarginPadding { Top = MARGIN, Bottom = 10 };
            InternalChild = new FillFlowContainer
            {
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(10),
                AutoSizeAxes = Axes.Y,
                RelativeSizeAxes = Axes.X,
                Children = new Drawable[]
                {
                    new IconButton
                    {
                        Icon = FontAwesome.Solid.AngleLeft,
                        Size = new Vector2(30)
                    },
                    new SpriteText
                    {
                        Text = name,
                        Font = FontUsage.Default.With(size: 40)
                    }.WithEffect(new GlowEffect
                    {
                        PadExtent = true,
                        Colour = Color4.White,
                        BlurSigma = new Vector2(10)
                    })
                }
            };

            DrawnHeight = MARGIN + DrawHeight + 10;
        }
    }
}
