using System;
using System.Collections.Generic;
using System.IO;

namespace Circle.Game.Beatmaps
{
    public interface IBeatmapSetInfo
    {
        Guid ID { get; }

        BeatmapMetadata Metadata { get; }

        IList<BeatmapInfo> Beatmaps { get; }
        List<FileInfo> Files { get; }
    }
}
