using Circle.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace Circle.Game.Tests.Visual.UserInterface
{
    public class TestSceneRollingControl : CircleTestScene
    {
        private RollingControl<string> control;

        [BackgroundDependencyLoader]
        private void load()
        {
            Add(new RollingControl<int>
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Text = "Rolling Control",
                Item = new[]
                {
                    new RollingItem<int>(1),
                    new RollingItem<int>(2),
                    new RollingItem<int>(3),
                }
            });
            Add(control = new RollingControl<string>
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Text = "Rolling Control",
                Item = new[]
                {
                    new RollingItem<string>("value1"),
                    new RollingItem<string>("value2"),
                    new RollingItem<string>("value3"),
                }
            });
            Add(new RollingControl<TestEnum>
            {
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Text = "Rolling Control",
                Item = new[]
                {
                    new RollingItem<TestEnum>(TestEnum.Enum1),
                    new RollingItem<TestEnum>(TestEnum.Enum2),
                    new RollingItem<TestEnum>(TestEnum.Enum3),
                }
            });
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            control.SetCurrent("value1");
        }

        private enum TestEnum
        {
            Enum1,
            Enum2,
            Enum3
        }
    }
}
