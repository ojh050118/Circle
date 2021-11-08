using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;

namespace Circle.Game.IO
{
    public class MonitoredResourceStore : MonitoredStorage, IResourceStore<byte[]>
    {
        protected IResourceStore<byte[]> UnderlyingStore;

        private string[] filters;

        public MonitoredResourceStore(Storage underlyingStorage, string filters = null)
            : base(underlyingStorage, filters)
        {
            UnderlyingStore = new StorageBackedResourceStore(underlyingStorage);
        }

        public byte[] Get(string name)
            => UnderlyingStore.Get(name);

        public Task<byte[]> GetAsync(string name)
            => UnderlyingStore.GetAsync(name);

        public IEnumerable<string> GetAvailableResources()
            => UnderlyingStorage
                .GetDirectories(string.Empty)
                .Append(string.Empty)
                .SelectMany(d => UnderlyingStorage.GetFiles(d))
                .Where(p => filters?.Contains(Path.GetExtension(p)) ?? false);

        public Stream GetStream(string name)
            => UnderlyingStore.GetStream(name);
    }
}
