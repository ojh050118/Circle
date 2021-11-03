using osu.Framework.Testing;

namespace Circle.Game.Tests.Visual
{
    public class CircleTestScene : TestScene
    {
        protected override ITestSceneTestRunner CreateRunner() => new CircleTestSceneTestRunner();

        private class CircleTestSceneTestRunner : CircleGameBase, ITestSceneTestRunner
        {
            private TestSceneTestRunner.TestRunner runner;

            protected override void LoadAsyncComplete()
            {
                base.LoadAsyncComplete();
                Add(runner = new TestSceneTestRunner.TestRunner());
            }

            public void RunTestBlocking(TestScene test) => runner.RunTestBlocking(test);
        }
    }
}
