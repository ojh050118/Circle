using System.Collections.Generic;
using Circle.Game.Beatmaps;
using Circle.Game.Graphics.Containers;
using Circle.Game.Graphics.UserInterface;
using Circle.Game.Overlays;
using Circle.Game.Overlays.OSD;
using Circle.Game.Screens;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Screens;
using osuTK.Graphics;

namespace Circle.Game
{
    public class CircleGame : CircleGameBase
    {
        private DependencyContainer dependencies;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        private ScreenStack screenStack;
        private Background background;
        private ImportOverlay import;
        private DialogOverlay dialog;
        private readonly Toast toast = new Toast();

        public GameScreenContainer ScreenContainer;

        [BackgroundDependencyLoader]
        private void load()
        {
            dependencies.CacheAs(toast);

            Children = new Drawable[]
            {
                ScreenContainer = new GameScreenContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        background = new Background(textureName: "bg1"),
                        screenStack = new ScreenStack { RelativeSizeAxes = Axes.Both },
                    }
                },
                import = new ImportOverlay(ScreenContainer),
                dialog = new DialogOverlay(ScreenContainer),
                toast,
            };

            dependencies.CacheAs(ScreenContainer);
            dependencies.CacheAs(background);
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
