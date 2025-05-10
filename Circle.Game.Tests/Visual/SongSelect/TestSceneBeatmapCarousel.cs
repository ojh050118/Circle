#nullable disable

using Circle.Game.Beatmaps;
using Circle.Game.Screens.Select;
using osu.Framework.Allocation;

namespace Circle.Game.Tests.Visual.SongSelect
{
    public partial class TestSceneBeatmapCarousel : CircleTestScene
    {
        [BackgroundDependencyLoader]
        private void load(BeatmapManager beatmapManager)
        {
            BeatmapCarousel carousel;

            Add(carousel = new BeatmapCarousel());
            AddLabel("Add carousel item");
            foreach (var bi in beatmapManager.GetAvailableBeatmaps())
                AddStep($"Add item({bi})", () => carousel.Add(bi, null));

            AddLabel("Select beatmap(vertical direction)");
            AddRepeatStep("Select beatmap(down)", carousel.SelectNext, 5);
            AddRepeatStep("Select beatmap(up)", carousel.SelectPrevious, 5);
            AddLabel("Select beatmap(beatmap)");

            foreach (var bi in beatmapManager.GetAvailableBeatmaps())
            {
                AddAssert($"Select beatmap({bi})", () =>
                {
                    carousel.Select(bi);
                    return carousel.SelectedItem.Value.BeatmapInfo.Equals(bi);
                });
            }
        }
    }
}
