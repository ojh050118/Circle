using Circle.Game.Beatmaps;
using Circle.Game.Screens.Select;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;

namespace Circle.Game.Tests.Visual.SongSelect
{
    public class TestSceneBeatmapCarousel : CircleTestScene
    {
        [BackgroundDependencyLoader]
        private void load(BeatmapStorage beatmaps)
        {
            BeatmapCarousel carousel;

            Add(new Box
            {
                Colour = Color4.Black,
                Alpha = 0.7f,
                RelativeSizeAxes = Axes.Both,
            });
            Add(carousel = new BeatmapCarousel());
            AddLabel("Add carousel item");
            foreach (var bi in beatmaps.GetBeatmapInfos())
                AddStep($"Add item({bi})", () => carousel.AddItem(bi, null));

            AddLabel("Select beatmap(vertical direction)");
            AddRepeatStep("Select beatmap(down)", () => carousel.SelectBeatmap(VerticalDirection.Down), 5);
            AddRepeatStep("Select beatmap(up)", () => carousel.SelectBeatmap(VerticalDirection.Up), 5);
            AddLabel("Select beatmap(beatmap)");
            foreach (var bi in beatmaps.GetBeatmapInfos())
            {
                AddAssert($"Select beatmap({bi})", () =>
                {
                    carousel.SelectBeatmap(bi);
                    return carousel.SelectedItem.Value.BeatmapInfo.Equals(bi);
                });
            }
        }
    }
}
