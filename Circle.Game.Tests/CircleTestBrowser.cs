#nullable disable

using System.Collections.Generic;
using Circle.Game.Beatmaps;
using Circle.Game.Configuration;
using Circle.Game.Graphics.UserInterface;
using Circle.Game.Overlays;
using Circle.Game.Overlays.OSD;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Platform;
using osu.Framework.Testing;
using osuTK.Graphics;

namespace Circle.Game.Tests
{
    public partial class CircleTestBrowser : CircleGameBase
    {
        private Background background;
        private DialogOverlay dialog;
        private ImportOverlay import;
        private ConvertOverlay convert;
        private CarouselItemOverlay item;
        private Toast toast;

        #region Disposal

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            BeatmapManager.OnLoadedBeatmaps -= loadedBeatmaps;
            BeatmapManager.OnImported -= imported;
        }

        #endregion

        public override void SetHost(GameHost host)
        {
            base.SetHost(host);
            host.Window.CursorState |= CursorState.Hidden;
        }

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            var dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
            dependencies.CacheAs(toast = new Toast());
            dependencies.CacheAs(background = new Background(textureName: "bg1"));
            dependencies.CacheAs(import = new ImportOverlay());
            dependencies.CacheAs(dialog = new DialogOverlay());
            dependencies.CacheAs(convert = new ConvertOverlay());
            dependencies.CacheAs(item = new CarouselItemOverlay());

            return dependencies;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            if (LocalConfig.Get<bool>(CircleSetting.LoadBeatmapsOnStartup))
                BeatmapManager.LoadBeatmaps();

            AddRange(new Drawable[]
            {
                background,
                new TestBrowser("Circle"),
                import,
                dialog,
                convert,
                item,
                toast,
                new CursorContainer()
            });

            BeatmapManager.OnLoadedBeatmaps += loadedBeatmaps;
            BeatmapManager.OnImported += imported;
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
