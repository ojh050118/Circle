using Circle.Game.Configuration;
using Circle.Game.IO;
using Circle.Resources;
using osu.Framework.Allocation;
using osu.Framework.Development;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Performance;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osuTK;

namespace Circle.Game
{
    public class CircleGameBase : osu.Framework.Game
    {
        public Container<Drawable> ContentContainer;

        public bool IsDevelopmentBuild { get; }

        protected CircleConfigManager LocalConfig { get; private set; }

        protected Storage Storage { get; set; }

        protected override Container<Drawable> Content => ContentContainer;

        private DependencyContainer dependencies;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        public string FrameworkVersion
        {
            get
            {
                var version = typeof(osu.Framework.Game).Assembly.GetName().Version;

                return $"{version.Major}.{version.Minor}.{version.Build}";
            }
        }

        protected CircleGameBase()
        {
            IsDevelopmentBuild = DebugUtils.IsDebugBuild;
            Name = $"Circle {(IsDevelopmentBuild ? "(Development Mode)" : string.Empty)}";
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            var files = Storage.GetStorageForDirectory("files");
            var tracks = new ResourceStore<byte[]>();
            var largeStore = new LargeTextureStore(Host.CreateTextureLoaderStore(new NamespacedResourceStore<byte[]>(Resources, @"Textures")));
            var monitoredLargeStore = new MonitoredLargeTextureStore(Host, files.GetStorageForDirectory("backgrounds"));

            Resources.AddStore(new DllResourceStore(typeof(CircleResources).Assembly));
            largeStore.AddStore(Host.CreateTextureLoaderStore(new OnlineStore()));
            tracks.AddStore(new TrackStore(files));

            dependencies.CacheAs(largeStore);
            dependencies.CacheAs(this);

            var externalAudioManager = new ExternalAudioManager(Host.AudioThread, tracks, new ResourceStore<byte[]>());
            dependencies.CacheAs(externalAudioManager);
            dependencies.CacheAs(monitoredLargeStore);

            dependencies.CacheAs(Storage);

            dependencies.CacheAs(LocalConfig);

            ContentContainer = new DrawSizePreservingFillContainer
            {
                TargetDrawSize = new Vector2(1366, 768),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            };

            base.Content.Add(ContentContainer);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            LocalConfig.GetBindable<bool>(CircleSetting.FpsDisplay).ValueChanged += value => FrameStatistics.Value = value.NewValue ? FrameStatisticsMode.Minimal : FrameStatisticsMode.None;
            LocalConfig.GetBindable<float>(CircleSetting.Scale).ValueChanged += value => ContentContainer.Size = new Vector2(X * value.NewValue, Y * value.NewValue);
        }

        public override void SetHost(GameHost host)
        {
            base.SetHost(host);

            Storage ??= host.Storage;
            LocalConfig ??= new CircleConfigManager(Storage);
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            LocalConfig?.Dispose();
        }
    }
}
