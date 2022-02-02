using Circle.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace Circle.Game.Tests.Visual.UserInterface
{
    public class TestSceneStepper : CircleTestScene
    {
        private Stepper<string> stepper;

        [BackgroundDependencyLoader]
        private void load()
        {
            Add(new Stepper<int>
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Text = "Stepper",
                Item = new[]
                {
                    new StepperItem<int>(1),
                    new StepperItem<int>(2),
                    new StepperItem<int>(3),
                }
            });
            Add(stepper = new Stepper<string>
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Text = "Stepper",
                Item = new[]
                {
                    new StepperItem<string>("value1"),
                    new StepperItem<string>("value2"),
                    new StepperItem<string>("value3"),
                }
            });
            Add(new Stepper<TestEnum>
            {
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Text = "Stepper",
                Item = new[]
                {
                    new StepperItem<TestEnum>(TestEnum.Enum1),
                    new StepperItem<TestEnum>(TestEnum.Enum2),
                    new StepperItem<TestEnum>(TestEnum.Enum3),
                }
            });
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            stepper.SetCurrent("value1");
        }

        private enum TestEnum
        {
            Enum1,
            Enum2,
            Enum3
        }
    }
}
