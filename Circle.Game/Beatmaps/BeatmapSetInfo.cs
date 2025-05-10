using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Circle.Game.Extensions;

namespace Circle.Game.Beatmaps
{
    // TODO: 2개이상의 비트맵이 있으면 노래선택 화면에서 스테이지 선택을 제공해야함.
    /// <summary>
    /// 비트맵 세트에 대한 정보.
    /// </summary>
    public class BeatmapSetInfo : IEquatable<BeatmapSetInfo>, IBeatmapSetInfo
    {
        public Guid ID { get; set; }

        public BeatmapMetadata Metadata => Beatmaps.FirstOrDefault()?.Metadata ?? new BeatmapMetadata();

        public IList<BeatmapInfo> Beatmaps { get; } = null!;

        public List<FileInfo> Files => Beatmaps.Select(b => b.File).ToList();

        public DirectoryInfo? Directory { get; }

        public BeatmapSetInfo(DirectoryInfo? directory = null, IEnumerable<BeatmapInfo>? beatmaps = null)
        {
            ID = Guid.NewGuid();

            Directory = directory;

            if (beatmaps != null)
                Beatmaps.AddRange(beatmaps);
        }

        public bool Equals(BeatmapSetInfo? other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other == null) return false;

            return ID == other.ID;
        }
    }
}
