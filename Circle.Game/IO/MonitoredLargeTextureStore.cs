using System.Threading.Tasks;
using osu.Framework.Graphics.Textures;
using osu.Framework.Platform;

namespace Circle.Game.IO
{
    public class MonitoredLargeTextureStore : MonitoredResourceStore
    {
        private LargeTextureStore largeTextureStore;

        public MonitoredLargeTextureStore(GameHost host, Storage underlyingStorage)
            : base(underlyingStorage)
        {
            largeTextureStore = new LargeTextureStore(host.CreateTextureLoaderStore(UnderlyingStore));
        }

        public new Texture Get(string name)
            => largeTextureStore.Get(name);

        public new Task<Texture> GetAsync(string name)
            => largeTextureStore.GetAsync(name);
    }
}
