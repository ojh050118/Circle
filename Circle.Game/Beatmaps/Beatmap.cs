#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Circle.Game.Converting.Json;
using Circle.Game.Rulesets.Extensions;
using Circle.Game.Rulesets.Objects;

namespace Circle.Game.Beatmaps
{
    public class Beatmap : IEquatable<Beatmap>
    {
        [JsonConverter(typeof(ExcludeIndentConverter<float[]>))]
        public float[] AngleData { get; set; }

        [JsonPropertyName("Settings")]
        public BeatmapMetadata Metadata
        {
            get => BeatmapInfo.Metadata;
            private set => BeatmapInfo.Metadata = value;
        }

        public ActionEvents[] Actions { get; set; }

        [JsonIgnore]
        public BeatmapInfo BeatmapInfo { get; set; }

        [JsonIgnore]
        public IEnumerable<Tile> Tiles => tiles ??= TileExtensions.GetTiles(this);

        private IEnumerable<Tile> tiles;

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

        public bool Equals(Beatmap other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other == null) return false;

            return Metadata.Equals(other.Metadata) && Actions.SequenceEqual(other.Actions);
        }

        public override string ToString() => $"[{Metadata.Author}] {Metadata.Artist} - {Metadata.Song}";
    }
}
