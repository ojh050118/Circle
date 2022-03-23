using System;
using Circle.Game.Beatmaps;
using Circle.Game.Configuration;
using Circle.Game.Input;
using Circle.Game.Overlays;
using Circle.Resources;
using osu.Framework.Allocation;
using osu.Framework.Configuration.Tracking;
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

        protected TrackedSettings TrackedSettings { get; set; }

        protected Storage Storage { get; set; }

        protected override Container<Drawable> Content => ContentContainer;

        protected MusicController MusicController { get; private set; }

        private DependencyContainer dependencies;

        protected BeatmapStorage BeatmapStorage { get; set; }

        protected BeatmapManager BeatmapManager { get; set; }

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
            Name = $"Circle{(IsDevelopmentBuild ? " (Development mode)" : string.Empty)}";
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            var files = Storage.GetStorageForDirectory("files");
            var largeStore = new LargeTextureStore(Host.CreateTextureLoaderStore(new NamespacedResourceStore<byte[]>(Resources, @"Textures")));

            Resources.AddStore(new DllResourceStore(typeof(CircleResources).Assembly));

            AddFont(Resources, @"Fonts/OpenSans-Regular");
            AddFont(Resources, @"Fonts/OpenSans-Light");
            AddFont(Resources, @"Fonts/OpenSans-Bold");
            AddFont(Resources, @"Fonts/OpenSans-SemiBold");
            AddFont(Resources, @"Fonts/Noto-Basic");
            AddFont(Resources, @"Fonts/Noto-Hangul");
            AddFont(Resources, @"Fonts/Noto-CJK-Basic");
            AddFont(Resources, @"Fonts/Noto-CJK-Compatibility");

            dependencies.CacheAs(largeStore);

            dependencies.CacheAs(BeatmapStorage = new BeatmapStorage(files, Audio, new NamespacedResourceStore<byte[]>(Resources, @"Beatmaps"), Host));
            dependencies.CacheAs(BeatmapManager = new BeatmapManager(BeatmapStorage));

            dependencies.CacheAs(Storage);

            dependencies.CacheAs(LocalConfig);
            dependencies.CacheAs(TrackedSettings);

            dependencies.CacheAs(MusicController = new MusicController());

            dependencies.CacheAs(this);

            ContentContainer = new DrawSizePreservingFillContainer
            {
                TargetDrawSize = new Vector2(1366, 768),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            };

            AddInternal(MusicController);
            base.Content.Add(ContentContainer);
            base.Content.Add(new CircleKeyBindingContainer(this));
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            applySettings();
        }

        public override void SetHost(GameHost host)
        {
            base.SetHost(host);

            Storage ??= host.Storage;
            LocalConfig ??= new CircleConfigManager(Storage);
            TrackedSettings = LocalConfig.CreateTrackedSettings();
            LocalConfig.LoadInto(TrackedSettings);

            TrackedSettings.SettingChanged += setTrackedSettingChange;
        }

        public void GracefullyExit()
        {
            if (!OnExiting())
                Exit();
            else
                Scheduler.AddDelayed(GracefullyExit, 2000);
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            LocalConfig?.Dispose();
        }

        private void applySettings()
        {
            var scale = LocalConfig.Get<float>(CircleSetting.Scale);
            var fpsDisplay = LocalConfig.Get<bool>(CircleSetting.FpsDisplay);

            ContentContainer.ScaleTo(scale);
            ContentContainer.ResizeTo(new Vector2(scale / (scale * scale)));
            FrameStatistics.Value = fpsDisplay ? FrameStatisticsMode.Minimal : FrameStatisticsMode.None;
        }

        private void setTrackedSettingChange(SettingDescription description)
        {
            switch (description.Name.ToString())
            {
                case "Scale":
                    ContentContainer.ScaleTo((float)description.RawValue, 1000, Easing.OutPow10);
                    ContentContainer.ResizeTo(new Vector2((float)((float)description.RawValue / Math.Pow((float)description.RawValue, 2))), 1000, Easing.OutPow10);
                    break;

                case "FpsDisplay":
                    FrameStatistics.Value = (bool)description.RawValue ? FrameStatisticsMode.Minimal : FrameStatisticsMode.None;
                    break;
            }
        }
    }
}
