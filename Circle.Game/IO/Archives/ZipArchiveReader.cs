#nullable disable

using System.Collections.Generic;
using System.IO;
using System.Linq;
using osu.Framework.IO.Stores;
using SharpCompress.Archives.Zip;

namespace Circle.Game.IO.Archives
{
    public sealed class ZipArchiveReader : ArchiveReader
    {
        public readonly ZipArchive Archive;
        private readonly Stream archiveStream;

        public ZipArchiveReader(Stream archiveStream, string name = null)
            : base(name)
        {
            this.archiveStream = archiveStream;
            Archive = ZipArchive.Open(archiveStream);
        }

        public override IEnumerable<string> Filenames => Archive.Entries.Select(e => e.Key).ExcludeSystemFileNames();

        public override Stream GetStream(string name)
        {
            ZipArchiveEntry entry = Archive.Entries.SingleOrDefault(e => e.Key == name);
            if (entry == null)
                throw new FileNotFoundException();

            MemoryStream copy = new MemoryStream();

            using (Stream s = entry.OpenEntryStream())
                s.CopyTo(copy);

            copy.Position = 0;

            return copy;
        }

        public override void Dispose()
        {
            Archive.Dispose();
            archiveStream.Dispose();
        }
    }
}
