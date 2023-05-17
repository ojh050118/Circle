#nullable disable

using System;
using osu.Framework.Graphics;

namespace Circle.Game.Beatmaps
{
    public class DummyBeatmap : Beatmap
    {
        public DummyBeatmap()
        {
            AngleData = Array.Empty<float>();
            Settings = new Settings
            {
                Artist = "please load a beatmap!",
                Song = "no beatmaps available!",
                SongFileName = string.Empty,
                Author = string.Empty,
                SeparateCountdownTime = false,
                PreviewSongStart = 0,
                PreviewSongDuration = 0,
                BeatmapDesc = string.Empty,
                Difficulty = 0,
                Bpm = 0,
                Volume = 0,
                Offset = 0,
                VidOffset = 0,
                Pitch = 0,
                CountdownTicks = 0,
                BgImage = string.Empty,
                BgVideo = string.Empty,
                RelativeTo = Relativity.Player,
                Position = Array.Empty<float>(),
                Rotation = 0,
                Zoom = 0,
                PlanetEasing = Easing.None
            };
            Actions = Array.Empty<Actions>();
        }
    }
}
