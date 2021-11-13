using Circle.Game.Graphics.UserInterface;
using Circle.Game.Overlays;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Platform;
using osu.Framework.Testing;

namespace Circle.Game.Tests
{
    public class CircleTestBrowser : CircleGameBase
    {
        private DependencyContainer dependencies;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Background background;
            DialogOverlay dialog;

            AddRange(new Drawable[]
            {
                background = new Background(textureName: "Duelyst"),
                new TestBrowser("Circle"),
                dialog = new DialogOverlay(),
                new CursorContainer()
            });
            dependencies.CacheAs(background);
            dependencies.CacheAs(dialog);
        }

        public override void SetHost(GameHost host)
        {
            base.SetHost(host);
            host.Window.CursorState |= CursorState.Hidden;
        }
    }
}
