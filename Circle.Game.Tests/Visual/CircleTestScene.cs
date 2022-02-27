using Circle.Game.Graphics.UserInterface;
using Circle.Game.Overlays;
using Circle.Game.Overlays.OSD;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Framework.Testing;

namespace Circle.Game.Tests.Visual
{
    public class CircleTestScene : TestScene
    {
        protected override ITestSceneTestRunner CreateRunner() => new CircleTestSceneTestRunner();

        private class CircleTestSceneTestRunner : CircleGameBase, ITestSceneTestRunner
        {
            private TestSceneTestRunner.TestRunner runner;

            protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
            {
                var dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
                dependencies.CacheAs(new Toast());
                dependencies.CacheAs(new Background(textureName: "bg1"));
                dependencies.CacheAs(new ImportOverlay(new BufferedContainer()));
                dependencies.CacheAs(new DialogOverlay(new BufferedContainer()));

                return dependencies;
            }

            protected override void LoadAsyncComplete()
            {
                base.LoadAsyncComplete();
                Add(runner = new TestSceneTestRunner.TestRunner());
            }

            public void RunTestBlocking(TestScene test) => runner.RunTestBlocking(test);
        }
    }
}
