using Circle.Game.Graphics.UserInterface;
using Circle.Game.Screens;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;
using osuTK;

namespace Circle.Game
{
    public class CircleGame : CircleGameBase
    {
        private DependencyContainer dependencies;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        private ScreenStack screenStack;
        public Background Background;

        [BackgroundDependencyLoader]
        private void load()
        {
            Children = new Drawable[]
            {
                Background = new Background(textureName: "Duelyst")
                {
                    Alpha = 0.3f,
                    BlurSigma = new Vector2(10)
                },
                screenStack = new ScreenStack { RelativeSizeAxes = Axes.Both }
            };

            dependencies.CacheAs(Background);
            dependencies.CacheAs(this);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            screenStack.Push(new Loader());
        }
    }
}
