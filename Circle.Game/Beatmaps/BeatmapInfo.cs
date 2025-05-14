#nullable disable

using System;
using System.IO;
using Newtonsoft.Json;
using osu.Framework.Logging;

namespace Circle.Game.Beatmaps
{
    /// <summary>
    /// 비트맵 파일에 대한 기본적인 정보를 제공합니다.
    /// 리소스에 대한 경로 정보도 포함하고 있습니다.
    /// </summary>
    public class BeatmapInfo : IEquatable<BeatmapInfo>
    {
        public Guid ID { get; set; }

        public BeatmapSetInfo BeatmapSet { get; set; }

        public BeatmapMetadata Metadata
        {
            get
            {
                if (metadata == null && File != null)
                {
                    using (var stream = File!.OpenRead())
                    {
                        try
                        {
                            SimpleBeatmap beatmap;

                            using (var reader = new StreamReader(stream))
                                beatmap = JsonConvert.DeserializeObject<SimpleBeatmap>(reader.ReadToEnd());

                            return metadata = beatmap.Metadata;
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e, "Failed to parsing beatmap metadata from file.");
                            return metadata ??= new BeatmapMetadata();
                        }
                    }
                }

                return metadata ??= new BeatmapMetadata();
            }

            set => metadata = value;
        }

        private BeatmapMetadata metadata;

        // TODO: 총 플레이시간 계산
        public double Length { get; set; }

        public readonly FileInfo File;

        public BeatmapInfo(FileInfo file = null, BeatmapMetadata metadata = null)
        {
            ID = Guid.NewGuid();

            File = file;
            Metadata = metadata;
        }

        public bool Equals(BeatmapInfo other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other == null) return false;

            // TODO: Metadata를 호출하면 동기적으로 비트맵 파싱이 발생할 수 있습니다. 빠른 비교를 위해 GUID 대신 해시, 또는 캐시를 사용해야 할 듯 합니다.
            return Metadata.Equals(other.Metadata);
        }

        /// <summary>
        /// <see cref="BeatmapMetadata"/>만 구문분석하여 오버헤드를 줄입니다.
        /// </summary>
        private class SimpleBeatmap
        {
            [JsonProperty("Settings")]
            public BeatmapMetadata Metadata { get; set; }
        }
    }
}
