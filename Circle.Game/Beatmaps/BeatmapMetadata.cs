#nullable disable

using System;
using System.Text.Json.Serialization;
using Circle.Game.Converting.Json;
using osu.Framework.Graphics;
using osu.Framework.Utils;

namespace Circle.Game.Beatmaps
{
    public class BeatmapMetadata : IEquatable<BeatmapMetadata>
    {
        /// <summary>
        /// 아티스트 이름.
        /// </summary>
        public string Artist { get; set; } = string.Empty;

        /// <summary>
        /// 표시할 음악 이름.
        /// </summary>
        public string Song { get; set; } = string.Empty;

        /// <summary>
        /// 확장자가 포함된 음악 파일 이름.
        /// </summary>
        public string SongFileName { get; set; } = string.Empty;

        /// <summary>
        /// 비트맵 작성자.
        /// </summary>
        public string Author { get; set; } = string.Empty;

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
        public string BeatmapDesc { get; set; } = string.Empty;

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
        public string BgImage { get; set; } = string.Empty;

        /// <summary>
        /// 배경 비디오 파일.
        /// </summary>
        public string BgVideo { get; set; } = string.Empty;

        /// <summary>
        /// 카메라 기준좌표.
        /// </summary>
        public Relativity RelativeTo { get; set; }

        /// <summary>
        /// 카메라 기준좌표으로부터 오프셋 좌표.
        /// </summary>
        [JsonConverter(typeof(ExcludeIndentConverter<float[]>))]
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

        public bool Equals(BeatmapMetadata other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other == null) return false;

            return Artist == other.Artist &&
                   Song == other.Song &&
                   SongFileName == other.SongFileName &&
                   Author == other.Author &&
                   SeparateCountdownTime == other.SeparateCountdownTime &&
                   PreviewSongStart == other.PreviewSongStart &&
                   PreviewSongDuration == other.PreviewSongDuration &&
                   BeatmapDesc == other.BeatmapDesc &&
                   Difficulty == other.Difficulty &&
                   Precision.AlmostEquals(Bpm, other.Bpm) &&
                   Volume == other.Volume &&
                   Offset == other.Offset &&
                   Precision.AlmostEquals(VidOffset, other.VidOffset) &&
                   Pitch == other.Pitch &&
                   CountdownTicks == other.CountdownTicks &&
                   BgImage == other.BgImage &&
                   BgVideo == other.BgVideo &&
                   RelativeTo == other.RelativeTo &&
                   Precision.AlmostEquals(Rotation, other.Rotation) &&
                   PlanetEasing == other.PlanetEasing;
        }
    }
}
