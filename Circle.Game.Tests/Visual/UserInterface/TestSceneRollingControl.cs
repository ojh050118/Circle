using Circle.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;

namespace Circle.Game.Tests.Visual.UserInterface
{
    public class TestSceneRollingControl : CircleTestScene
    {
        private RollingControl<string> control;

        [BackgroundDependencyLoader]
        private void load()
        {
            Add(new Box { RelativeSizeAxes = Axes.Both, Colour = Color4.DarkGray });
            Add(new RollingControl<int>
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Text = "Rolling Control",
                Item = new RollingItem<int>[]
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
                Item = new RollingItem<string>[]
                {
                    new RollingItem<string>("value1"),
                    new RollingItem<string>("value2"),
                    new RollingItem<string>("value3"),
                }
            });
            Add(new RollingControl<testEnum>
            {
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Text = "Rolling Control",
                Item = new RollingItem<testEnum>[]
                {
                    new RollingItem<testEnum>(testEnum.Enum1),
                    new RollingItem<testEnum>(testEnum.Enum2),
                    new RollingItem<testEnum>(testEnum.Enum3),
                }
            });
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            control.SetCurrent("value1");
        }

        private enum testEnum
        {
            Enum1,
            Enum2,
            Enum3
        }
    }
}