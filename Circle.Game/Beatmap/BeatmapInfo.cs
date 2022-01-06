using osu.Framework.Graphics;

namespace Circle.Game.Beatmap
{
    public struct BeatmapInfo
    {
        public float[] AngleData;
        public Settings Settings;
        public Actions[] Actions;

        public bool Equals(BeatmapInfo info) => AngleData?.Length == info.AngleData?.Length && Settings.Equals(info.Settings) && Actions.Length == info.Actions.Length;
    }

    public struct Settings
    {
        public string Artist;
        public string Song;
        public string SongFileName;
        public string Author;
        public bool SeparateCountdownTime;
        public int PreviewSongStart;
        public int PreviewSongDuration;
        public string BeatmapDesc;
        public int Difficulty;
        public float Bpm;
        public int Volume;
        public int Offset;
        public int Pitch;
        public int CountdownTicks;
        public string BgImage;
        public Easing PlanetEasing;

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
        public int Floor;
        public EventType EventType;
        public SpeedType? SpeedType;
        public float BeatsPerMinute;
        public float BpmMultiplier;
        public Relativity? RelativeTo;
    }

    public enum EventType
    {
        Twirl,
        SetSpeed,
        MoveCamera,
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
