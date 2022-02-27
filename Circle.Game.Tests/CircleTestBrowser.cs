using System.Collections.Generic;
using Circle.Game.Beatmaps;
using Circle.Game.Configuration;
using Circle.Game.Graphics.UserInterface;
using Circle.Game.Overlays;
using Circle.Game.Overlays.OSD;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Platform;
using osu.Framework.Testing;
using osuTK.Graphics;

namespace Circle.Game.Tests
{
    public class CircleTestBrowser : CircleGameBase
    {
        private Background background;
        private ImportOverlay import;
        private DialogOverlay dialog;
        private Toast toast;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            var dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
            dependencies.CacheAs(toast = new Toast());
            dependencies.CacheAs(background = new Background(textureName: "bg1"));
            dependencies.CacheAs(import = new ImportOverlay(new BufferedContainer()));
            dependencies.CacheAs(dialog = new DialogOverlay(new BufferedContainer()));

            return dependencies;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            if (LocalConfig.Get<bool>(CircleSetting.LoadBeatmapsOnStartup))
                BeatmapManager.ReloadBeatmaps();

            AddRange(new Drawable[]
            {
                background,
                new TestBrowser("Circle"),
                import,
                dialog,
                toast,
                new CursorContainer()
            });

            BeatmapManager.OnLoadedBeatmaps += loadedBeatmaps;
            BeatmapManager.OnImported += imported;
        }

        public override void SetHost(GameHost host)
        {
            base.SetHost(host);
            host.Window.CursorState |= CursorState.Hidden;
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
