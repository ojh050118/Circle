namespace Circle.Game.Converting.Adofai.Elements
{
    public class Settings
    {
        public int Version { get; set; } = 6;
        public string Artist { get; set; } = "Artist";
        public string SpecialArtistType { get; set; } = "None";
        public string ArtistPermission { get; set; } = string.Empty;
        public string Song { get; set; } = "Song";
        public string Author { get; set; } = "Author";
        public string SeparateCountdownTime { get; set; } = "Enabled";
        public string PreviewImage { get; set; } = string.Empty;
        public string PreviewIcon { get; set; } = string.Empty;
        public string PreviewIconColor { get; set; } = "003f52";
        public int PreviewSongStart { get; set; } = 0;
        public int PreviewSongDuration { get; set; } = 10;
        public string SeizureWarning { get; set; } = "Disabled";
        public string LevelDesc { get; set; } = string.Empty;
        public string LevelTags { get; set; } = string.Empty;
        public string ArtistLinks { get; set; } = string.Empty;
        public int Difficulty { get; set; } = 1;
        public string SongFilename { get; set; } = string.Empty;
        public float Bpm { get; set; } = 100;
        public int Volume { get; set; } = 100;
        public int Offset { get; set; } = 0;
        public int Pitch { get; set; } = 100;
        public string HitSound { get; set; } = "kick";
        public int HitSoundVolume { get; set; } = 100;
        public int CountdownTicks { get; set; } = 4;
        public string TrackColorType { get; set; } = "Single";
        public string TrackColor { get; set; } = "debb7b";
        public string SecondaryTrackColor { get; set; } = "ffffff";
        public float TrackColorAnimDuration { get; set; } = 2;
        public string TrackColorPulse { get; set; } = "None";
        public float TrackPulseLength { get; set; } = 10;
        public string TrackStyle { get; set; } = "Standard";
        public string TrackAnimation { get; set; } = "None";
        public float BeatsAhead { get; set; } = 3;
        public string TrackDisappearAnimation { get; set; } = "None";
        public float BeatsBehind { get; set; } = 4;
        public string BackgroundColor { get; set; } = "000000";
        public string BgImage { get; set; } = string.Empty;
        public string BgImageColor { get; set; } = "ffffff";
        public float[] Parallax { get; set; } = { 100, 100 };
        public string BgDisplayMode { get; set; } = "FitToScreen";
        public string LockRot { get; set; } = "Disabled";
        public string LoopBg { get; set; } = "Disabled";
        public float UnscaledSize { get; set; } = 100;
        public string RelativeTo { get; set; } = "Player";
        public float[] Position { get; set; } = { 0, 0 };
        public float Rotation { get; set; } = 0;
        public float Zoom { get; set; } = 100;
        public string BgVideo { get; set; } = string.Empty;
        public string LoopVideo { get; set; } = "Disabled";
        public float VidOffset { get; set; } = 0;
        public string FloorIconOutlines { get; set; } = "Disabled";
        public string StickToFloors { get; set; } = "Disabled";
        public string PlanetEase { get; set; } = "Linear";
        public int PlanetEaseParts { get; set; } = 1;
        public bool LegacyFlash { get; set; } = false;
        public bool LegacySpriteTiles { get; set; } = false;
    }
}
