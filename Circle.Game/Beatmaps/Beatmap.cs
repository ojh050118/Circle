using osu.Framework.Graphics;

namespace Circle.Game.Beatmaps
{
    public class Beatmap
    {
        public float[] AngleData { get; set; }
        public Settings Settings { get; set; }
        public Actions[] Actions { get; set; }
    }

    public struct Settings
    {
        public string Artist { get; set; }
        public string Song { get; set; }
        public string SongFileName { get; set; }
        public string Author { get; set; }
        public bool SeparateCountdownTime { get; set; }
        public int PreviewSongStart { get; set; }
        public int PreviewSongDuration { get; set; }
        public string BeatmapDesc { get; set; }
        public int Difficulty { get; set; }
        public float Bpm { get; set; }
        public int Volume { get; set; }
        public int Offset { get; set; }
        public int Pitch { get; set; }
        public int CountdownTicks { get; set; }
        public string BgImage { get; set; }
        public Easing PlanetEasing { get; set; }

        public bool Equals(Settings settings) => Artist == settings.Artist &&
                                                 Song == settings.Song &&
                                                 SongFileName == settings.SongFileName &&
                                                 Author == settings.Author &&
                                                 SeparateCountdownTime == settings.SeparateCountdownTime &&
                                                 PreviewSongStart == settings.PreviewSongStart &&
                                                 PreviewSongDuration == settings.PreviewSongDuration &&
                                                 BeatmapDesc == settings.BeatmapDesc &&
                                                 Difficulty == settings.Difficulty &&
                                                 (int)Bpm == (int)settings.Bpm &&
                                                 Volume == settings.Volume &&
                                                 Offset == settings.Offset &&
                                                 Pitch == settings.Pitch &&
                                                 CountdownTicks == settings.CountdownTicks &&
                                                 BgImage == settings.BgImage &&
                                                 PlanetEasing == settings.PlanetEasing;
    }

    public struct Actions
    {
        public int Floor { get; set; }
        public EventType EventType { get; set; }
        public SpeedType? SpeedType { get; set; }
        public float BeatsPerMinute { get; set; }
        public float BpmMultiplier { get; set; }
        public Relativity? RelativeTo { get; set; }
        public Easing Ease { get; set; }
    }

    public enum EventType
    {
        Twirl,
        SetSpeed,
        MoveCamera,
        SetPlanetRotation,
        Other
    }

    public enum SpeedType
    {
        Multiplier,
        Bpm
    }

    public enum Relativity
    {
        Tile,
        LastPosition,
        Player,
        Global,
        RedPlanet,
        BluePlanet
    }
}
