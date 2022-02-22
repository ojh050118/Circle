using Circle.Game.Configuration;
using Circle.Game.Graphics.UserInterface;
using Circle.Game.Overlays;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Platform;
using osu.Framework.Testing;

namespace Circle.Game.Tests
{
    public class CircleTestBrowser : CircleGameBase
    {
        private Background background;
        private DialogOverlay dialog;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            var dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
            dependencies.CacheAs(background = new Background(textureName: "bg1"));
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
                dialog,
                new CursorContainer()
            });
        }

        public override void SetHost(GameHost host)
        {
            base.SetHost(host);
            host.Window.CursorState |= CursorState.Hidden;
        }
    }
}
