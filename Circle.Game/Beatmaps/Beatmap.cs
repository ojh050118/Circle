#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using Circle.Game.Rulesets.Extensions;
using Circle.Game.Rulesets.Objects;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using osu.Framework.Utils;

namespace Circle.Game.Beatmaps
{
    public class Beatmap : IEquatable<Beatmap>
    {
        public float[] AngleData { get; set; }

        [JsonProperty("Settings")]
        public BeatmapMetadata Metadata
        {
            get => BeatmapInfo.Metadata;
            private set => BeatmapInfo.Metadata = value;
        }

        public ActionEvents[] Actions { get; set; }
        private TileInfo[] tilesInfo;

        [JsonIgnore]
        public BeatmapInfo BeatmapInfo { get; set; }

        public Beatmap()
        {
            BeatmapInfo = new BeatmapInfo
            {
                Metadata = new BeatmapMetadata
                {
                    Author = "Unknown Creator",
                    Song = "Unknown"
                }
            };
        }

        [JsonIgnore]
        public TileInfo[] TilesInfo => tilesInfo ??= CalculationExtensions.GetTilesInfo(this);

        [JsonIgnore]
        public IReadOnlyList<double> TileStartTime => tileStartTime ??= CalculationExtensions.GetTileStartTime(this, Metadata.Offset, 60000 / Metadata.Bpm * Metadata.CountdownTicks);

        private IReadOnlyList<double> tileStartTime;

        public bool Equals(Beatmap other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other == null) return false;

            return Metadata.Equals(other.Metadata) && Actions.SequenceEqual(other.Actions);
        }
    }

    /// <summary>
    /// 행성이 특정 타일에 도달했을 때 동작할 정보를 담고있습니다. 이벤트라고도 불립니다.
    /// </summary>
    public struct ActionEvents : IEquatable<ActionEvents>
    {
        /// <summary>
        /// 이 액션의 위치.
        /// </summary>
        public int Floor { get; set; }

        /// <summary>
        /// 이벤트의 종류.
        /// </summary>
        public EventType? EventType { get; set; }

        /// <summary>
        /// 속도 유형. 이벤트 타입이 SetSpeed일때만 사용가능합니다.
        /// </summary>
        public SpeedType? SpeedType { get; set; }

        /// <summary>
        /// 분당 비트 횟수.
        /// </summary>
        public float BeatsPerMinute { get; set; }

        /// <summary>
        /// bpm 승수.
        /// </summary>
        public float BpmMultiplier { get; set; }

        /// <summary>
        /// 카메라의 기준좌표. 이벤트 타입이 MoveCamera일때만 사용가능 합니다.
        /// </summary>
        public Relativity? RelativeTo { get; set; }

        /// <summary>
        /// 가감속.
        /// </summary>
        public Easing Ease { get; set; }

        /// <summary>
        /// 이벤트의 지속시간. 비트단위입니다.
        /// </summary>
        public double Duration { get; set; }

        /// <summary>
        /// 회전. null이면 회전하지 않음을 의미합니다.
        /// </summary>
        public float? Rotation { get; set; }

        /// <summary>
        /// 이벤트를 실행하기 위해 행성이 도달해야하는 각도.
        /// </summary>
        public float AngleOffset { get; set; }

        /// <summary>
        /// 타일 단위의 좌표. 현재 카메라 이동에만 사용됩니다.
        /// </summary>
        public float?[] Position { get; set; }

        /// <summary>
        /// 카메라의 확대/축소 비율.
        /// </summary>
        public int? Zoom { get; set; }

        /// <summary>
        /// 이벤트 반복 횟수. 원래 이벤트에 추가할 실행할 횟수를 결정합니다.
        /// </summary>
        public int Repetitions { get; set; }

        /// <summary>
        /// 실행 간격. 비트단위입니다.
        /// </summary>
        public double Interval { get; set; }

        /// <summary>
        /// 이벤트의 식별명. '이벤트 반복'과 '조건부 이벤트'에 사용됩니다.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// 이벤트의 식별명. '이벤트 반복'과 '조건부 이벤트'에 사용됩니다.
        /// </summary>
        public string EventTag { get; set; }

        public override string ToString()
        {
            return $"Floor: {Floor} | Event type: {EventType}";
        }

        public bool Equals(ActionEvents actionEvents) => Floor == actionEvents.Floor &&
                                                         EventType == actionEvents.EventType &&
                                                         SpeedType == actionEvents.SpeedType &&
                                                         Precision.AlmostEquals(BeatsPerMinute, actionEvents.BeatsPerMinute) &&
                                                         Precision.AlmostEquals(BpmMultiplier, actionEvents.BpmMultiplier) &&
                                                         RelativeTo == actionEvents.RelativeTo &&
                                                         Ease == actionEvents.Ease &&
                                                         Duration == actionEvents.Duration &&
                                                         Rotation == actionEvents.Rotation &&
                                                         AngleOffset == actionEvents.AngleOffset &&
                                                         Zoom == actionEvents.Zoom &&
                                                         Repetitions == actionEvents.Repetitions &&
                                                         Precision.AlmostEquals(Interval, actionEvents.Interval) &&
                                                         Tag == actionEvents.Tag;
    }

    public enum EventType
    {
        Twirl,
        SetSpeed,
        MoveCamera,
        SetPlanetRotation,
        RepeatEvents,
        Other,
        Pause
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
