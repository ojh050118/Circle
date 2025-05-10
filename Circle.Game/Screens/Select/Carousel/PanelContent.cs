#nullable disable

using Circle.Game.Beatmaps;
using Circle.Game.Graphics;
using Circle.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace Circle.Game.Screens.Select.Carousel
{
    public partial class PanelContent : Container
    {
        private readonly BeatmapInfo info;

        public PanelContent(BeatmapInfo info)
        {
            this.info = info;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;
            Children = new Drawable[]
            {
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Padding = new MarginPadding { Horizontal = 20, Vertical = 10 },
                    Spacing = new Vector2(5),
                    Children = new Drawable[]
                    {
                        new CircleSpriteText
                        {
                            Text = info.Metadata.Song,
                            Font = CircleFont.Default.With(weight: FontWeight.Bold, size: 30)
                        },
                        new CircleSpriteText
                        {
                            Text = info.Metadata.Author,
                            Font = CircleFont.Default.With(size: 24)
                        }
                    }
                }
            };
        }
    }
}
