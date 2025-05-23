#nullable disable

using System;
using System.Text.Json;
using Circle.Game.Beatmaps;

namespace Circle.Game.Converting.Adofai.Elements
{
    public class ActionEvent
    {
        public int Floor { get; set; }
        public EventType EventType { get; set; }
        public bool Enabled { get; set; } = true;
        public string TrackColor { get; set; }
        public string SecondaryTrackColor { get; set; }
        public TrackColorType TrackColorType { get; set; }
        public LegacyTrackStyle TrackStyle { get; set; }
        public ColorPulseType TrackColorPulse { get; set; }
        public int TrackPulseLength { get; set; }
        public double TrackColorAnimDuration { get; set; }
        public SpeedType? SpeedType { get; set; }
        public double BeatsPerMinute { get; set; }
        public double BPMMultiplier { get; set; }
        public float HitsoundVolume { get; set; }
        public string BgImage { get; set; }
        public double Duration { get; set; }
        public FlashPlane Plane { get; set; }
        public string StartColor { get; set; }
        public string EndColor { get; set; }
        public float StartOpacity { get; set; }
        public float EndOpacity { get; set; }
        public int? Zoom { get; set; }
        public Ease Ease { get; set; }
        public DisplayMode BgDisplayMode { get; set; }
        public string ImageColor { get; set; }
        public int UnscaledSize { get; set; }
        public float? Rotation { get; set; }
        public float AngleOffset { get; set; }

        /// <summary>
        /// <see cref="EventType"/>에 따라 타입이 바뀝니다.
        /// <see cref="EventType.MoveCamera"/>일때, PositionRelativity이며,
        /// <see cref="EventType.PositionTrack"/>일때, int와 TileRelativity로 구성된 튜플입니다.
        /// </summary>
        public object RelativeTo { get; set; }

        public float?[] Position { get; set; }
        public object Tile { get; set; }
        public string EventTag { get; set; }
        public string Tag { get; set; }
        public int Repetitions { get; set; }
        public double Interval { get; set; }
        public bool LoopBg { get; set; }
        public int StartTime { get; set; }
        public int Index { get; set; }
        public int Depth { get; set; }
        public string DecorationImage { get; set; }
        public string DecText { get; set; }
        public string Color { get; set; }
        public object Scale { get; set; }
        public object RotationOffset { get; set; }
        public bool EditorOnly { get; set; }
        public bool LockRot { get; set; }
        public object Parallax { get; set; }
        public object PositionOffset { get; set; }
        public float[] PivotOffset { get; set; }
        public object[] StartTile { get; set; }
        public object[] EndTile { get; set; }
        public float? Opacity { get; set; }
        public string PerfectTag { get; set; }
        public string HitTag { get; set; }
        public string BarelyTag { get; set; }
        public string MissTag { get; set; }
        public string LossTag { get; set; }
        public double BeatsAhead { get; set; }
        public double BeatsBehind { get; set; }
        public int EaseParts { get; set; }
        public int Strength { get; set; }
        public string Filter { get; set; }
        public int Intensity { get; set; }
        public bool DisableOthers { get; set; }
        public bool FadeOut { get; set; }
        public object Components { get; set; }
        public string TargetType { get; set; }
        public string TargetTag { get; set; }
        public string FilterProperties { get; set; }
        public string PlanetTailColor { get; set; }

        public Relativity? GetCameraMoveRelativeTo()
        {
            switch (RelativeTo)
            {
                case JsonElement r:
                    Relativity? result = null;

                    switch (r.ValueKind)
                    {
                        case JsonValueKind.String:
                            if (Enum.TryParse(r.GetString(), out Relativity parsed))
                                result = parsed;

                            break;

                        case JsonValueKind.Number:
                            result = (Relativity)r.GetInt32();
                            break;
                    }

                    return result;

                case Relativity r:
                    return r;

                case string r:
                    if (Enum.TryParse(r, out Relativity relativity))
                        return relativity;

                    return null;

                case int r:
                    return (Relativity)r;

                default:
                    return null;
            }
        }

        // TODO: MoveTrack, PositionTrack등 트랙에 관한 이벤트의 기준좌표 변환 구현
        public object GetTrackRelativeTo() => new object();
    }
}
