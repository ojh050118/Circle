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
        public Settings Settings { get; set; }
        public Actions[] Actions { get; set; }

        [JsonIgnore]
        public TileInfo[] TilesInfo => tilesInfo ??= CalculationExtensions.GetTilesInfo(this);

        private TileInfo[] tilesInfo;

        [JsonIgnore]
        public IReadOnlyList<double> TileStartTime => tileStartTime ??= CalculationExtensions.GetTileStartTime(this, Settings.Offset, 60000 / Settings.Bpm * Settings.CountdownTicks);

        private IReadOnlyList<double> tileStartTime;

        public bool Equals(Beatmap beatmap) => beatmap != null && Settings.Equals(beatmap.Settings) && Actions.SequenceEqual(beatmap.Actions);
    }

    public struct Settings : IEquatable<Settings>
    {
        /// <summary>
        /// 아티스트 이름.
        /// </summary>
        public string Artist { get; set; }

        /// <summary>
        /// 음악 이름.
        /// </summary>
        public string Song { get; set; }

        /// <summary>
        /// 확장자가 포함된 음악 파일 이름.
        /// </summary>
        public string SongFileName { get; set; }

        /// <summary>
        /// 비트맵 작성자.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// 시작 전 카운트다운 여부.
        /// </summary>
        public bool SeparateCountdownTime { get; set; }

        /// <summary>
        /// 미리듣기 재생 시작 구간.
        /// </summary>
        public int PreviewSongStart { get; set; }

        /// <summary>
        /// 미리듣기 지속시간.
        /// </summary>
        public int PreviewSongDuration { get; set; }

        /// <summary>
        /// 비트맵 설명.
        /// </summary>
        public string BeatmapDesc { get; set; }

        /// <summary>
        /// 난이도.
        /// </summary>
        public int Difficulty { get; set; }

        /// <summary>
        /// 분당 비트 수.
        /// </summary>
        public float Bpm { get; set; }

        /// <summary>
        /// 음악의 볼륨.
        /// </summary>
        public int Volume { get; set; }

        /// <summary>
        /// 음악 오프셋.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// 비디오 오프셋.
        /// </summary>
        public float VidOffset { get; set; }

        /// <summary>
        /// 음악의 피치.
        /// </summary>
        public int Pitch { get; set; }

        /// <summary>
        /// 카운트다운 틱 횟수.
        /// </summary>
        public int CountdownTicks { get; set; }

        /// <summary>
        /// 배경 이미지 파일.
        /// </summary>
        public string BgImage { get; set; }

        /// <summary>
        /// 배경 비디오 파일.
        /// </summary>
        public string BgVideo { get; set; }

        /// <summary>
        /// 카메라 기준좌표.
        /// </summary>
        public Relativity RelativeTo { get; set; }

        /// <summary>
        /// 카메라 기준좌표으로부터 오프셋 좌표.
        /// </summary>
        public float[] Position { get; set; }

        /// <summary>
        /// 카메라 회전.
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// 카메라 확대율.
        /// </summary>
        public float Zoom { get; set; }

        /// <summary>
        /// 행성 가감속.
        /// </summary>
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
                                                 Precision.AlmostEquals(Bpm, settings.Bpm) &&
                                                 Volume == settings.Volume &&
                                                 Offset == settings.Offset &&
                                                 Precision.AlmostEquals(VidOffset, settings.VidOffset) &&
                                                 Pitch == settings.Pitch &&
                                                 CountdownTicks == settings.CountdownTicks &&
                                                 BgImage == settings.BgImage &&
                                                 BgVideo == settings.BgVideo &&
                                                 RelativeTo == settings.RelativeTo &&
                                                 Precision.AlmostEquals(Rotation, settings.Rotation) &&
                                                 PlanetEasing == settings.PlanetEasing;
    }

    /// <summary>
    /// 행성이 특정 타일에 도달했을 때 동작할 정보를 담고있습니다. 이벤트라고도 불립니다.
    /// </summary>
    public struct Actions : IEquatable<Actions>
    {
        /// <summary>
        /// 이 액션의 위치.
        /// </summary>
        public int Floor { get; set; }

        /// <summary>
        /// 이벤트의 종류.
        /// </summary>
        public EventType EventType { get; set; }

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

        public bool Equals(Actions actions) => Floor == actions.Floor &&
                                               EventType == actions.EventType &&
                                               SpeedType == actions.SpeedType &&
                                               Precision.AlmostEquals(BeatsPerMinute, actions.BeatsPerMinute) &&
                                               Precision.AlmostEquals(BpmMultiplier, actions.BpmMultiplier) &&
                                               RelativeTo == actions.RelativeTo &&
                                               Ease == actions.Ease &&
                                               Duration == actions.Duration &&
                                               Rotation == actions.Rotation &&
                                               AngleOffset == actions.AngleOffset &&
                                               Zoom == actions.Zoom &&
                                               Repetitions == actions.Repetitions &&
                                               Precision.AlmostEquals(Interval, actions.Interval) &&
                                               Tag == actions.Tag;
    }

    public enum EventType
    {
        Twirl,
        SetSpeed,
        MoveCamera,
        SetPlanetRotation,
        RepeatEvents,
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
