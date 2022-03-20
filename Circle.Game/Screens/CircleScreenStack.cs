using Circle.Game.Graphics.Containers;
using Circle.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace Circle.Game.Screens
{
    public class CircleScreenStack : ScreenStack
    {
        private readonly Background background;

        /// <summary>
        /// 화면 스택 개수.
        /// </summary>
        public int Count { get; private set; }

        public CircleScreenStack()
        {
            InternalChild = new ParallaxContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = background = new Background()
            };

            ScreenPushed += (prev, next) => Count++;
            ScreenExited += (prev, next) => Count--;
        }

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            var dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

            dependencies.CacheAs(background);

            return dependencies;
        }
    }
}
