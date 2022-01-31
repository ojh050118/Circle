using Circle.Game.Beatmap;
using Circle.Game.Overlays;
using osu.Framework.Allocation;

namespace Circle.Game.Tests.Visual.Overlays
{
    public class TestSceneMusicController : CircleTestScene
    {
        [Resolved]
        private MusicController music { get; set; }

        public TestSceneMusicController()
        {
            music = new MusicController();
            AddStep("Play", () => music.Play());
            AddStep("Stop", () => music.Stop());
            AddStep("Restart", () => music.RestartTrack());
            AddLabel("tracks");
        }

        [BackgroundDependencyLoader]
        private void load(BeatmapStorage beatmaps, MusicController music)
        {
            foreach (var beatmap in beatmaps.GetBeatmaps())
            {
                AddStep($"{beatmap.Settings.SongFileName}", () => music.ChangeTrack(beatmap));
            }
        }
    }
}
