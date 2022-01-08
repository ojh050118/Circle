using System;
using Circle.Game.Beatmap;
using Circle.Game.Configuration;
using Circle.Game.Input;
using Circle.Game.Overlays;
using Circle.Resources;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Configuration.Tracking;
using osu.Framework.Development;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Performance;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
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

        protected Bindable<BeatmapInfo> WorkingBeatmap { get; set; }

        protected override Container<Drawable> Content => ContentContainer;

        protected MusicController MusicController { get; private set; }

        private DependencyContainer dependencies;

        protected BeatmapResourcesManager BeatmapResourceStore { get; set; }

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
            var largeStore = new LargeTextureStore(Host.CreateTextureLoaderStore(new NamespacedResourceStore<byte[]>(Resources, @"Textures")));

            Resources.AddStore(new DllResourceStore(typeof(CircleResources).Assembly));
            largeStore.AddStore(Host.CreateTextureLoaderStore(new OnlineStore()));

            AddFont(Resources, @"Fonts/OpenSans-Regular");
            AddFont(Resources, @"Fonts/OpenSans-Light");
            AddFont(Resources, @"Fonts/OpenSans-Bold");
            AddFont(Resources, @"Fonts/OpenSans-SemiBold");
            AddFont(Resources, @"Fonts/Noto-Hangul");

            dependencies.CacheAs(largeStore);

            dependencies.CacheAs(new BeatmapStorage(files));
            dependencies.CacheAs(BeatmapResourceStore = new BeatmapResourcesManager(files, Audio, Host));

            dependencies.CacheAs(Storage);

            dependencies.CacheAs(LocalConfig);
            dependencies.CacheAs(TrackedSettings);

            dependencies.CacheAs(WorkingBeatmap = new Bindable<BeatmapInfo>());

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

            WorkingBeatmap.ValueChanged += value =>
            {
                var oldBeatmapName = $"[{value.OldValue.Settings.Author}] {value.OldValue.Settings.Artist} - {value.OldValue.Settings.Song}";
                var newBeatmapName = $"[{value.NewValue.Settings.Author}] {value.NewValue.Settings.Artist} - {value.NewValue.Settings.Song}";
                Logger.Log($"Beatmap changed: {oldBeatmapName} ¡æ {value.NewValue.Settings.Author} - {newBeatmapName}.");
            };
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
