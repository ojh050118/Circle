using Circle.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Circle.Game.Tests.Visual.UserInterface
{
    public class TestSceneStepper : CircleTestScene
    {
        private Stepper<int> intStepper;
        private Stepper<string> stringStepper;
        private Stepper<TestEnum> enumStepper;

        [BackgroundDependencyLoader]
        private void load()
        {
            Add(new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                RowDimensions = new[]
                {
                    new Dimension(GridSizeMode.AutoSize)
                },
                Content = new[]
                {
                    new Drawable[]
                    {
                        intStepper = new Stepper<int>
                        {
                            Anchor = Anchor.TopLeft,
                            Origin = Anchor.TopLeft,
                            Text = "int stepper(no initial value)",
                            Items = new[]
                            {
                                new StepperItem<int>(1),
                                new StepperItem<int>(2),
                                new StepperItem<int>(3),
                            }
                        },
                        stringStepper = new Stepper<string>("string2")
                        {
                            Anchor = Anchor.TopLeft,
                            Origin = Anchor.TopLeft,
                            Text = "string stepper",
                            Items = new[]
                            {
                                new StepperItem<string>("string1"),
                                new StepperItem<string>("string2"),
                                new StepperItem<string>("string3"),
                            }
                        },
                        enumStepper = new Stepper<TestEnum>
                        {
                            Anchor = Anchor.TopLeft,
                            Origin = Anchor.TopLeft,
                            Text = "enum stepper",
                            Items = new[]
                            {
                                new StepperItem<TestEnum>(TestEnum.Enum1),
                                new StepperItem<TestEnum>(TestEnum.Enum2),
                                new StepperItem<TestEnum>(TestEnum.Enum3),
                            }
                        }
                    }
                }
            });
            AddLabel("int stepper");
            AddAssert("Select 1", () =>
            {
                intStepper.Select(1);
                return intStepper.Selected == 1;
            });
            AddRepeatStep("Select next", intStepper.SelectNext, 5);
            AddRepeatStep("Select previous", intStepper.SelectPrevious, 5);

            AddLabel("string stepper");
            AddAssert("Select \"string1\"", () =>
            {
                stringStepper.Select("string1");
                return stringStepper.Selected == "string1";
            });
            AddRepeatStep("Select next", stringStepper.SelectNext, 5);
            AddRepeatStep("Select previous", stringStepper.SelectPrevious, 5);

            AddLabel("enum stepper");
            AddAssert("Select Enum1", () =>
            {
                enumStepper.Select(TestEnum.Enum1);
                return enumStepper.Selected == TestEnum.Enum1;
            });
            AddRepeatStep("Select next", enumStepper.SelectNext, 5);
            AddRepeatStep("Select previous", enumStepper.SelectPrevious, 5);
        }

        private enum TestEnum
        {
            Enum1,
            Enum2,
            Enum3
        }
    }
}
