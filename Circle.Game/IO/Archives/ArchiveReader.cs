using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Extensions;
using osu.Framework.IO.Stores;

namespace Circle.Game.IO.Archives
{
    public abstract class ArchiveReader : IResourceStore<byte[]>
    {
        public abstract Stream GetStream(string name);

        public IEnumerable<string> GetAvailableResources() => Filenames;

        public abstract void Dispose();

        public readonly string Name;

        protected ArchiveReader(string name)
        {
            Name = name;
        }

        public abstract IEnumerable<string> Filenames { get; }

        public virtual byte[] Get(string name)
        {
            using (Stream input = GetStream(name))
                return input?.ReadAllBytesToArray();
        }

        public async Task<byte[]> GetAsync(string name, CancellationToken cancellationToken = default)
        {
            using (Stream input = GetStream(name))
            {
                if (input == null)
                    return null;

                return await input.ReadAllBytesToArrayAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
