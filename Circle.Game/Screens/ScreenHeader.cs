#nullable disable

using Circle.Game.Graphics;
using Circle.Game.Graphics.Sprites;
using Circle.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Screens
{
    public partial class ScreenHeader : CompositeDrawable
    {
        public const int MARGIN = 30;

        private readonly CircleScreen screen;

        public ScreenHeader(CircleScreen screen)
        {
            this.screen = screen;
        }

        public string Text { get; set; } = string.Empty;

        [BackgroundDependencyLoader]
        private void load()
        {
            Padding = new MarginPadding { Top = MARGIN, Horizontal = MARGIN, Bottom = 10 };
            AutoSizeAxes = Axes.Y;
            RelativeSizeAxes = Axes.X;
            InternalChild = new GridContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                RowDimensions = new[]
                {
                    new Dimension(GridSizeMode.AutoSize),
                    new Dimension(GridSizeMode.AutoSize),
                },
                ColumnDimensions = new[]
                {
                    new Dimension(GridSizeMode.AutoSize)
                },
                Content = new[]
                {
                    new Drawable[]
                    {
                        new IconButton
                        {
                            Icon = FontAwesome.Solid.AngleLeft,
                            Size = new Vector2(40),
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Action = screen.OnExit
                        },
                        new CircleSpriteText
                        {
                            Text = string.IsNullOrEmpty(Text) ? screen.Header : Text,
                            Font = CircleFont.Default.With(size: 40),
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Truncate = true,
                            RelativeSizeAxes = Axes.X,
                            Margin = new MarginPadding { Left = 10 }
                        }.WithEffect(new GlowEffect
                        {
                            PadExtent = true,
                            Colour = Color4.White,
                            BlurSigma = new Vector2(10)
                        })
                    }
                }
            };
        }
    }
}
