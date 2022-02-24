using Circle.Game.Graphics.Containers;
using Circle.Game.Graphics.UserInterface;
using Circle.Game.Overlays;
using Circle.Game.Screens;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

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

        public GameScreenContainer ScreenContainer;

        [BackgroundDependencyLoader]
        private void load()
        {
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
                dialog = new DialogOverlay(ScreenContainer)
            };

            dependencies.CacheAs(background);
            dependencies.CacheAs(import);
            dependencies.CacheAs(dialog);
            dependencies.CacheAs(this);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            screenStack.Push(new Loader());
        }
    }
}
