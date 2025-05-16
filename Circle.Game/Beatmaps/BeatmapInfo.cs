#nullable disable

using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
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

        private static readonly JsonSerializerOptions serializer_options = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            IncludeFields = true,
            Converters = { new JsonStringEnumConverter() }
        };

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
                            var beatmap = JsonSerializer.Deserialize<SimpleBeatmap>(stream, serializer_options);

                            return metadata = beatmap.Metadata;
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e, $"Failed to parse beatmap metadata from file {File.FullName}.");
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

#pragma warning disable CA1812

        /// <summary>
        /// <see cref="BeatmapMetadata"/>만 구문분석하여 오버헤드를 줄입니다.
        /// </summary>
        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        private class SimpleBeatmap
        {
            [JsonPropertyName("Settings")]
            public BeatmapMetadata Metadata { get; init; }
        }

#pragma warning restore CA1812
    }
}
