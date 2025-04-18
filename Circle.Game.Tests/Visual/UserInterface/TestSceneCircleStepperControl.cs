using System.Linq;
using Circle.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;

namespace Circle.Game.Tests.Visual.UserInterface
{
    public partial class TestSceneCircleStepperControl : CircleTestScene
    {
        private CircleStepperControl<int> intStepper = null!;
        private CircleStepperControl<string> stringStepper = null!;
        private CircleEnumStepperControl<TestEnum> enumStepper = null!;
        private CircleStepperControl<float> emptyStepper = null!;

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
                        intStepper = new CircleStepperControl<int>
                        {
                            Anchor = Anchor.TopLeft,
                            Origin = Anchor.TopLeft,
                            LabelText = "int stepper(no initial value)",
                            Items = new[] { 1, 2, 3 }
                        },
                        stringStepper = new CircleStepperControl<string>
                        {
                            Anchor = Anchor.TopLeft,
                            Origin = Anchor.TopLeft,
                            LabelText = "string stepper",
                            Current = new Bindable<string>("String2"),
                            Items = new[] { "string1", "string2", "string3", "string4" }
                        },
                        enumStepper = new CircleEnumStepperControl<TestEnum>
                        {
                            Anchor = Anchor.TopLeft,
                            Origin = Anchor.TopLeft,
                            LabelText = "enum stepper",
                        }
                    },
                    new Drawable[]
                    {
                        emptyStepper = new CircleStepperControl<float>
                        {
                            Anchor = Anchor.TopLeft,
                            Origin = Anchor.TopLeft,
                            LabelText = "empty float stepper",
                        }
                    }
                }
            });

            AddLabel("int stepper");
            AddAssert("Disallow value cycling", () =>
            {
                intStepper.AllowValueCycling = false;
                return intStepper.AllowValueCycling == false;
            });
            AddAssert("Select 2", () =>
            {
                intStepper.Select(2);
                return intStepper.Current.Value == 2;
            });
            AddAssert("Select 1", () =>
            {
                intStepper.Select(1);
                return intStepper.Current.Value == 1;
            });
            AddRepeatStep("Select next", () => intStepper.MoveNext(), 5);
            AddAssert("Ensure current value is 3", () => intStepper.Current.Value == 3);
            AddRepeatStep("Select previous", intStepper.MovePrevious, 5);
            AddAssert("Ensure current value is 1", () => intStepper.Current.Value == 1);
            AddStep("Allow value cycling", () => intStepper.AllowValueCycling = true);
            AddRepeatStep("Select previous", intStepper.MovePrevious, 5);
            AddAssert("Ensure current value is 2", () => intStepper.Current.Value == 2);

            AddLabel("string stepper");
            AddAssert("Disallow value cycling", () =>
            {
                stringStepper.AllowValueCycling = false;
                return stringStepper.AllowValueCycling == false;
            });
            AddAssert("Select \"string2\"", () =>
            {
                stringStepper.Select("string2");
                return stringStepper.Current.Value == "string2";
            });
            AddAssert("Select \"string1\"", () =>
            {
                stringStepper.Select("string1");
                return stringStepper.Current.Value == "string1";
            });
            AddRepeatStep("Select next", () => stringStepper.MoveNext(), 5);
            AddAssert("Ensure current value is \"string4\"", () => stringStepper.Current.Value == "string4");
            AddRepeatStep("Select previous", stringStepper.MovePrevious, 5);
            AddAssert("Ensure current value is \"string1\"", () => stringStepper.Current.Value == "string1");
            AddStep("Allow value cycling", () => stringStepper.AllowValueCycling = true);
            AddRepeatStep("Select previous", stringStepper.MovePrevious, 6);
            AddAssert("Ensure current value is \"string3\"", () => stringStepper.Current.Value == "string3");

            AddLabel("enum stepper");
            AddAssert("Disallow value cycling", () =>
            {
                enumStepper.AllowValueCycling = false;
                return enumStepper.AllowValueCycling == false;
            });
            AddAssert("Select Enum3", () =>
            {
                enumStepper.Select(TestEnum.Enum3);
                return enumStepper.Current.Value == TestEnum.Enum3;
            });
            AddAssert("Select Enum1", () =>
            {
                enumStepper.Select(TestEnum.Enum1);
                return enumStepper.Current.Value == TestEnum.Enum1;
            });
            AddRepeatStep("Select next", () => enumStepper.MoveNext(), 5);
            AddAssert("Ensure current value is Enum3", () => enumStepper.Current.Value == TestEnum.Enum3);
            AddRepeatStep("Select previous", enumStepper.MovePrevious, 5);
            AddAssert("Ensure current value is Enum1", () => enumStepper.Current.Value == TestEnum.Enum1);
            AddStep("Allow value cycling", () => enumStepper.AllowValueCycling = true);
            AddRepeatStep("Select previous", enumStepper.MovePrevious, 5);
            AddAssert("Ensure current value is Enum2", () => enumStepper.Current.Value == TestEnum.Enum2);

            AddLabel("empty float stepper");
            AddAssert("Disallow value cycling", () =>
            {
                enumStepper.AllowValueCycling = false;
                return enumStepper.AllowValueCycling == false;
            });
            AddAssert("Ensure no items", () =>
            {
                emptyStepper.ClearItems();
                return !emptyStepper.Items.Any();
            });
            AddStep("Add value 0.1", () => emptyStepper.AddItem(0.1f));
            AddAssert("Select 0.1", () =>
            {
                emptyStepper.Select(0.1f);
                return Precision.AlmostEquals(emptyStepper.Current.Value, 0.1f);
            });
            AddStep("Add value 0.9", () => emptyStepper.AddItem(0.9f));
            AddRepeatStep("Select next", () => emptyStepper.MoveNext(), 5);
            AddAssert("Ensure current value is 0.9", () => Precision.AlmostEquals(emptyStepper.Current.Value, 0.9f));
            AddRepeatStep("Select previous", emptyStepper.MovePrevious, 5);
            AddAssert("Ensure current value is 0.1", () => Precision.AlmostEquals(emptyStepper.Current.Value, 0.1f));
            AddStep("Allow value cycling", () => emptyStepper.AllowValueCycling = true);
            AddRepeatStep("Select previous", emptyStepper.MovePrevious, 4);
            AddAssert("Ensure current value is 0.1", () => Precision.AlmostEquals(emptyStepper.Current.Value, 0.1f));
            AddStep("Remove value 0.9", () => emptyStepper.RemoveItem(0.9f));
            AddStep("Remove value 0.1", () => emptyStepper.RemoveItem(0.1f));
        }

        private enum TestEnum
        {
            Enum1,
            Enum2,
            Enum3
        }
    }
}
