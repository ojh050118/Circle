using Circle.Game.Beatmap;
using Circle.Game.Screens.Select;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace Circle.Game.Tests.Visual.SongSelect
{
    public class TestSceneBeatmapDetails : CircleTestScene
    {
        [BackgroundDependencyLoader]
        private void load(BeatmapStorage beatmaps)
        {
            BeatmapDetails details;

            Add(details = new BeatmapDetails { Padding = new MarginPadding(10) });
            AddLabel("Beatmaps");
            foreach (var beatmap in beatmaps.GetBeatmaps())
                AddStep($"Change to {beatmap.Settings.SongFileName}", () => details.ChangeBeatmap(beatmap));
        }
    }
}
