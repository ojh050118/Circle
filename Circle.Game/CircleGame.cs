using System.Collections.Generic;
using Circle.Game.Beatmaps;
using Circle.Game.Configuration;
using Circle.Game.Graphics.Containers;
using Circle.Game.Graphics.UserInterface;
using Circle.Game.Overlays;
using Circle.Game.Overlays.OSD;
using Circle.Game.Overlays.Volume;
using Circle.Game.Screens;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osuTK.Graphics;

namespace Circle.Game
{
    public class CircleGame : CircleGameBase
    {
        private DependencyContainer dependencies;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        private CircleScreenStack screenStack;
        private VolumeOverlay volume;
        private ImportOverlay import;
        private DialogOverlay dialog;
        private readonly Toast toast = new Toast();

        public GameScreenContainer ScreenContainer;

        [BackgroundDependencyLoader]
        private void load()
        {
            dependencies.CacheAs(toast);

            addGameScreen();

            dependencies.CacheAs(volume);
            dependencies.CacheAs(import);
            dependencies.CacheAs(dialog);
            dependencies.CacheAs(this);

            BeatmapManager.OnLoadedBeatmaps += loadedBeatmaps;
            BeatmapManager.OnImported += imported;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            screenStack.Push(new Loader());
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            BeatmapManager.OnLoadedBeatmaps -= loadedBeatmaps;
            BeatmapManager.OnImported -= imported;
        }

        private void addGameScreen()
        {
            ScreenContainer = new GameScreenContainer
            {
                RelativeSizeAxes = Axes.Both,
                RedrawOnScale = false,
                Depth = 6,
                Children = new Drawable[]
                {
                    screenStack = new CircleScreenStack { RelativeSizeAxes = Axes.Both },
                }
            };
            dependencies.CacheAs(ScreenContainer);

            Children = new Drawable[]
            {
                new VolumeControlReceptor
                {
                    RelativeSizeAxes = Axes.Both,
                    Depth = 7,
                    ActionRequested = action => volume.Adjust(action),
                    ScrollActionRequested = (action, amount, _) => volume.Adjust(action, amount),
                },
                volume = new VolumeOverlay(),
                import = new ImportOverlay(),
                dialog = new DialogOverlay(),
                toast,
            };

            if (LocalConfig.Get<bool>(CircleSetting.BlurVisibility))
            {
                Add(ScreenContainer);
            }
            else
            {
                ScreenContainer.Dispose();
                AddRange(new Drawable[]
                {
                    new Background(textureName: "bg1") { Depth = 6 },
                    screenStack = new CircleScreenStack { RelativeSizeAxes = Axes.Both, Depth = 5 },
                });
            }
        }

        private void loadedBeatmaps(IList<BeatmapInfo> beatmaps)
        {
            toast.Push(new ToastInfo
            {
                Description = $"Loaded {beatmaps.Count} beatmaps!",
                Icon = FontAwesome.Solid.Check,
                IconColour = Color4.LightGreen
            });
        }

        private void imported(string name)
        {
            toast.Push(new ToastInfo
            {
                Description = "Imported successfully!",
                SubDescription = name,
                Icon = FontAwesome.Solid.Check,
                IconColour = Color4.LightGreen
            });
        }
    }
}
