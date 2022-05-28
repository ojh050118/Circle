using Circle.Game.Beatmaps;
using Circle.Game.Overlays;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;

namespace Circle.Game.Tests.Visual.Overlays
{
    public class TestSceneMusicController : CircleTestScene
    {
        [Resolved]
        private MusicController music { get; set; }

        private SpriteText text;

        [BackgroundDependencyLoader]
        private void load(BeatmapStorage beatmaps, MusicController music)
        {
            Add(text = new SpriteText
            {
                Text = $"Current time: {music.CurrentTrack?.CurrentTime}"
            });
            AddStep("Play", () => music.Play());
            AddStep("Stop", music.Stop);
            AddStep("Restart", music.RestartTrack);
            AddLabel("tracks");

            foreach (var bi in beatmaps.GetBeatmapInfos())
            {
                if (!string.IsNullOrEmpty(bi.SongPath))
                    AddStep($"{bi.Beatmap.Settings.SongFileName}", () => music.ChangeTrack(bi));
            }
        }

        protected override void Update()
        {
            base.Update();

            text.Text = $"Current time: {music.CurrentTrack.CurrentTime}";
        }
    }
}
