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

        public MonitoredResourceStore(Storage underlyingStorage)
            : base(underlyingStorage)
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
               .SelectMany(d => UnderlyingStorage.GetFiles(d));

        public Stream GetStream(string name)
            => UnderlyingStore.GetStream(name);
    }
}
