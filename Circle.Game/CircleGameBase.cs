using Circle.Resources;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osuTK;

namespace Circle.Game
{
    public class CircleGameBase : osu.Framework.Game
    {
        protected override Container<Drawable> Content { get; }

        private DependencyContainer dependencies;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        protected CircleGameBase()
        {
            base.Content.Add(Content = new DrawSizePreservingFillContainer
            {
                TargetDrawSize = new Vector2(1366, 768)
            });
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            var largeStore = new LargeTextureStore(Host.CreateTextureLoaderStore(new NamespacedResourceStore<byte[]>(Resources, @"Textures")));

            Resources.AddStore(new DllResourceStore(typeof(CircleResources).Assembly));
            largeStore.AddStore(Host.CreateTextureLoaderStore(new OnlineStore()));

            dependencies.CacheAs(largeStore);
            dependencies.CacheAs(this);
        }
    }
}
