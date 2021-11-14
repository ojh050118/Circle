using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;

namespace Circle.Game.IO
{
    public class TrackStore : IResourceStore<byte[]>
    {
        private readonly Storage storage;

        public TrackStore(Storage storage)
        {
            this.storage = storage?.GetStorageForDirectory("tracks");
        }

        public byte[] Get(string name)
        {
            using (Stream sr = GetStream(name))
            {
                byte[] buffer = new byte[sr.Length];
                sr.Read(buffer, 0, buffer.Length);

                return buffer;
            }
        }

        public Stream GetStream(string name)
        {
            FileStream fs;

            try
            {
                fs = File.OpenRead($"{storage.GetFullPath(string.Empty)}/{name}.mp3");
            }
            catch
            {
                fs = File.OpenRead($"{storage.GetFullPath(string.Empty)}/{name}.ogg");
            }

            return fs;
        }

        public Task<byte[]> GetAsync(string name) => null;

        public IEnumerable<string> GetAvailableResources() => Enumerable.Empty<string>();

        private bool isDisposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                isDisposed = true;
            }
        }

        ~TrackStore()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
