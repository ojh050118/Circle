using Circle.Game.Beatmaps;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace Circle.Game.Screens.Select.Carousel
{
    public class PanelContent : Container
    {
        private readonly BeatmapInfo beatmapInfo;

        public PanelContent(BeatmapInfo info)
        {
            beatmapInfo = info;
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
                        new SpriteText
                        {
                            Text = beatmapInfo.Beatmap.Settings.Song,
                            Font = FontUsage.Default.With("OpenSans-Bold", size: 30)
                        },
                        new SpriteText
                        {
                            Text = beatmapInfo.Beatmap.Settings.Author,
                            Font = FontUsage.Default.With(size: 24)
                        }
                    }
                }
            };
        }
    }
}
