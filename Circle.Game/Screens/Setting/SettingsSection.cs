#nullable disable

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Screens.Setting
{
    public abstract class SettingsSection : Container
    {
        private const int header_size = 30;
        private const int margin = 20;

        public const int CONTENT_MARGINS = 15;
        protected FillFlowContainer FlowContent;

        protected SettingsSection()
        {
            Margin = new MarginPadding { Bottom = margin };
            AutoSizeAxes = Axes.Y;
            RelativeSizeAxes = Axes.X;

            FlowContent = new FillFlowContainer
            {
                Margin = new MarginPadding { Top = header_size + margin },
                Padding = new MarginPadding { Horizontal = CONTENT_MARGINS },
                Direction = FillDirection.Vertical,
                AutoSizeAxes = Axes.Y,
                RelativeSizeAxes = Axes.X,
                Spacing = new Vector2(5)
            };
        }

        public abstract string Header { get; }

        [BackgroundDependencyLoader]
        private void load()
        {
            AddRangeInternal(new Drawable[]
            {
                new Box
                {
                    Colour = Color4.Black,
                    Height = 3,
                    RelativeSizeAxes = Axes.X,
                    Alpha = 0.5f,
                },
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Margin = new MarginPadding { Top = 20 },
                    Children = new Drawable[]
                    {
                        new SpriteText
                        {
                            Font = FontUsage.Default.With(size: header_size),
                            Text = Header,
                            Margin = new MarginPadding { Horizontal = CONTENT_MARGINS },
                        },
                        FlowContent
                    }
                }
            });
        }
    }
}
