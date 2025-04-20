#nullable disable

using Circle.Game.Beatmaps;
using Circle.Game.Screens.Select;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace Circle.Game.Tests.Visual.SongSelect
{
    public partial class TestSceneBeatmapDetails : CircleTestScene
    {
        [BackgroundDependencyLoader]
        private void load(BeatmapManager beatmapManager)
        {
            BeatmapDetails details;

            Add(details = new BeatmapDetails { Padding = new MarginPadding(10) });
            AddLabel("Beatmaps");
            foreach (var beatmap in beatmapManager.GetAvailableBeatmaps())
                AddStep($"Change to {beatmap.Metadata.SongFileName}", () => details.ChangeBeatmap(beatmap));
        }
    }
}
