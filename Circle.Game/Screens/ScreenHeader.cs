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
    public class ScreenHeader : CompositeDrawable
    {
        public const int MARGIN = 30;

        public string Text { get; set; } = string.Empty;

        private readonly CircleScreen screen;

        public ScreenHeader(CircleScreen screen)
        {
            this.screen = screen;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Margin = new MarginPadding { Top = MARGIN, Left = 30, Bottom = 10 };
            AutoSizeAxes = Axes.Both;
            InternalChild = new FillFlowContainer
            {
                Direction = FillDirection.Horizontal,
                AutoSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new IconButton
                    {
                        Icon = FontAwesome.Solid.AngleLeft,
                        Size = new Vector2(30),
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Action = screen.OnExit
                    },
                    new SpriteText
                    {
                        Text = string.IsNullOrEmpty(Text) ? screen.Header : Text,
                        Font = FontUsage.Default.With(size: 40),
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft
                    }.WithEffect(new GlowEffect
                    {
                        PadExtent = true,
                        Colour = Color4.White,
                        BlurSigma = new Vector2(10)
                    })
                }
            };
        }
    }
}
