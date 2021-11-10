using Circle.Game.Graphics.UserInterface;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace Circle.Game.Tests.Visual.UserInterface
{
    public class TestSceneCircleSliderBar : CircleTestScene
    {
        public TestSceneCircleSliderBar()
        {
            var value = new BindableNumber<float>
            {
                MinValue = 0,
                MaxValue = 100,
            };

            Add(new CircleSliderBar<float>
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.X,
                KeyboardStep = 5,
                Current = value
            });
        }
    }
}
