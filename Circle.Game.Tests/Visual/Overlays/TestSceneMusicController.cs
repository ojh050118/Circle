﻿using Circle.Game.Beatmap;
using Circle.Game.Overlays;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;

namespace Circle.Game.Tests.Visual.Overlays
{
    public class TestSceneMusicController : CircleTestScene
    {
        [Resolved]
        private MusicController music { get; set; }

        private readonly SpriteText text;

        public TestSceneMusicController()
        {
            music = new MusicController();
            Add(text = new SpriteText
            {
                Text = $"Current time: {music.CurrentTrack.CurrentTime}"
            });
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

        protected override void Update()
        {
            base.Update();

            text.Text = $"Current time: {music.CurrentTrack.CurrentTime}";
        }
    }
}
