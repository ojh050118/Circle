#nullable disable

using Circle.Game.Beatmaps;
using Circle.Game.Configuration;
using Circle.Game.Graphics;
using Circle.Game.Graphics.Containers;
using Circle.Game.Input;
using Circle.Game.Overlays;
using Circle.Resources;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Development;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Performance;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;

namespace Circle.Game
{
    public partial class CircleGameBase : osu.Framework.Game
    {
        public ScalingContainer ContentContainer;

        private DependencyContainer dependencies;

        protected CircleGameBase()
        {
            IsDevelopmentBuild = DebugUtils.IsDebugBuild;
            Name = $"Circle{(IsDevelopmentBuild ? " (Development mode)" : string.Empty)}";
        }

        public bool IsDevelopmentBuild { get; }

        protected CircleConfigManager LocalConfig { get; private set; }

        protected Storage Storage { get; set; }

        protected override Container<Drawable> Content => ContentContainer;

        protected MusicController MusicController { get; private set; }

        protected Bindable<WorkingBeatmap> Beatmap { get; private set; }

        protected BeatmapManager BeatmapManager { get; set; }

        public string FrameworkVersion
        {
            get
            {
                var version = typeof(osu.Framework.Game).Assembly.GetName().Version;

                if (version == null)
                    return "Unknown version";

                return $"{version.Major}.{version.Minor}.{version.Build}";
            }
        }

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        [BackgroundDependencyLoader]
        private void load()
        {
            var files = Storage.GetStorageForDirectory("files");
            var largeStore = new LargeTextureStore(Host.Renderer, Host.CreateTextureLoaderStore(new NamespacedResourceStore<byte[]>(Resources, @"Textures")));

            Resources.AddStore(new DllResourceStore(typeof(CircleResources).Assembly));

            var defaultBeatmap = new DummyWorkingBeatmap(Audio, Textures);
            Beatmap = new NonNullableBindable<WorkingBeatmap>(defaultBeatmap);

            AddFont(Resources, @"Fonts/OpenSans-Regular");
            AddFont(Resources, @"Fonts/OpenSans-Light");
            AddFont(Resources, @"Fonts/OpenSans-Bold");
            AddFont(Resources, @"Fonts/OpenSans-SemiBold");
            AddFont(Resources, @"Fonts/Noto-Basic");
            AddFont(Resources, @"Fonts/Noto-Hangul");
            AddFont(Resources, @"Fonts/Noto-CJK-Basic");
            AddFont(Resources, @"Fonts/Noto-CJK-Compatibility");

            dependencies.CacheAs(largeStore);

            dependencies.CacheAs(BeatmapManager = new BeatmapManager(files, Audio, Resources, Host, defaultBeatmap));
            dependencies.CacheAs(Beatmap);

            dependencies.CacheAs(Storage);
            dependencies.CacheAs(LocalConfig);

            dependencies.CacheAs(new CircleColour());
            dependencies.CacheAs(MusicController = new MusicController());
            dependencies.CacheAs(this);

            ContentContainer = new ScalingContainer
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both
            };

            base.Content.Add(MusicController);
            base.Content.Add(ContentContainer);
            base.Content.Add(new CircleKeyBindingContainer(this));
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            var fpsDisplay = LocalConfig.GetBindable<bool>(CircleSetting.FpsDisplay);
            fpsDisplay.BindValueChanged(fpsDisplayChanged, true);
            FrameStatistics.ValueChanged += e => fpsDisplay.Value = e.NewValue != FrameStatisticsMode.None;
        }

        public override void SetHost(GameHost host)
        {
            base.SetHost(host);

            Storage ??= host.Storage;
            LocalConfig ??= new CircleConfigManager(Storage);
        }

        private void fpsDisplayChanged(ValueChangedEvent<bool> e)
        {
            FrameStatistics.Value = e.NewValue ? FrameStatisticsMode.Minimal : FrameStatisticsMode.None;
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
    }
}
