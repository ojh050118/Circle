using System;
using Circle.Game.Beatmaps;

namespace Circle.Game.Converting.Adofai.Elements
{
    public class Settings
    {
        public int Version { get; set; } = 15;

        #region Song settings

        public string SongFilename { get; set; } = string.Empty;
        public float Bpm { get; set; } = 100;
        public int Volume { get; set; } = 100;
        public int Offset { get; set; } = 0;
        public int Pitch { get; set; } = 100;
        public string HitSound { get; set; } = "Kick";
        public int HitSoundVolume { get; set; } = 100;
        public int CountdownTicks { get; set; } = 4;

        #endregion

        #region Level settings

        public string Artist { get; set; } = string.Empty;
        public string SpecialArtistType { get; set; } = "None";
        public string ArtistPermission { get; set; } = string.Empty;

        public string Song { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;

        public bool SeparateCountdownTime { get; set; } = true;

        public string PreviewImage { get; set; } = string.Empty;
        public string PreviewIcon { get; set; } = string.Empty;
        public string PreviewIconColor { get; set; } = "003f52";

        public int PreviewSongStart { get; set; } = 0;
        public int PreviewSongDuration { get; set; } = 10;

        public bool SeizureWarning { get; set; }

        public string LevelDesc { get; set; } = string.Empty;
        public string LevelTags { get; set; } = string.Empty;
        public string ArtistLinks { get; set; } = string.Empty;

        public float SpeedTrialAim { get; set; }

        public int Difficulty { get; set; } = 1;

        public object[] RequiredMods { get; set; } = Array.Empty<object>();

        #endregion

        #region Track Settings

        public TileShape TileShape { get; set; } = TileShape.Long;
        public TrackColorType TrackColorType { get; set; } = TrackColorType.Single;
        public string TrackColor { get; set; } = "debb7bff";
        public string SecondaryTrackColor { get; set; } = "ffffff";
        public float TrackColorAnimDuration { get; set; } = 2;
        public TrackColorPulseType TrackColorPulse { get; set; } = TrackColorPulseType.None;
        public float TrackPulseLength { get; set; } = 10;
        public TrackStyle TrackStyle { get; set; } = TrackStyle.Standard;
        public string TrackTexture { get; set; } = string.Empty;
        public float TrackTextureScale { get; set; } = 1;
        public int TrackGlowIntensity { get; set; } = 100;
        public TrackAnimation TrackAnimation { get; set; } = TrackAnimation.None;
        public float BeatsAhead { get; set; } = 3;
        public TrackDisappearAnimation TrackDisappearAnimation { get; set; } = TrackDisappearAnimation.None;
        public float BeatsBehind { get; set; } = 4;

        #endregion

        #region Background settings

        public string BackgroundColor { get; set; } = "000000";
        public bool ShowDefaultBgIfNoImage { get; set; } = true;
        public bool ShowDefaultBgTile { get; set; } = true;
        public string DefaultBgTileColor { get; set; } = "101121";
        public DefaultBgShapeType DefaultBgShapeType { get; set; } = DefaultBgShapeType.Default;
        public string DefaultBgShapeColor { get; set; } = "ffffff";

        public string BgImage { get; set; } = string.Empty;
        public string BgImageColor { get; set; } = "ffffff";
        public float[] Parallax { get; set; } = { 100, 100 };
        public string BgDisplayMode { get; set; } = "FitToScreen";
        public bool ImageSmoothing { get; set; } = true;
        public bool LockRot { get; set; }
        public bool LoopBg { get; set; }
        public float ScaleRatio { get; set; } = 100;
        public float UnscaledSize { get; set; } = 100;

        #endregion

        #region Camera Settings

        public Relativity RelativeTo { get; set; } = Relativity.Player;

        // TODO: 나중에 타입을 Vector2로 전환
        public float[] Position { get; set; } = { 0, 0 };
        public float Rotation { get; set; } = 0;
        public float Zoom { get; set; } = 100;
        public bool PulseOnFloor { get; set; } = true;
        public bool StartCamLowVfx { get; set; } = false;

        #endregion

        #region Other setings

        public string BgVideo { get; set; } = string.Empty;
        public bool LoopVideo { get; set; }
        public float VidOffset { get; set; } = 0;

        public bool FloorIconOutlines { get; set; }
        public bool StickToFloors { get; set; }

        public Ease PlanetEase { get; set; } = Ease.Linear;
        public int PlanetEaseParts { get; set; } = 1;
        public PlanetEasePartBehavior PlanetEasePartBehavior { get; set; } = PlanetEasePartBehavior.Mirror;

        public string DefaultTextColor { get; set; } = "ffffff";
        public string DefaultTextShadowColor { get; set; } = "00000050";
        public string CongratsText { get; set; } = string.Empty;
        public string PerfectText { get; set; } = string.Empty;

        #endregion

        #region Legacy settings

        public bool LegacyFlash { get; set; } = false;
        public bool LegacyCamRelativeTo { get; set; } = true;
        public bool LegacySpriteTiles { get; set; } = false;
        public bool LegacyTween { get; set; } = true;
        public bool DisableV15Features { get; set; } = true;

        #endregion
    }
}
