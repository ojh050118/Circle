#nullable disable

using Circle.Game.Graphics.UserInterface;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace Circle.Game.Tests.Visual.UserInterface
{
    public partial class TestSceneCircleSliderBar : CircleTestScene
    {
        public TestSceneCircleSliderBar()
        {
            CircleSliderBar<float> slider;
            var value = new BindableNumber<float>
            {
                MinValue = 0,
                MaxValue = 100,
            };

            Add(slider = new CircleSliderBar<float>
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.X,
                KeyboardStep = 5,
                Current = value
            });
            AddSliderStep<float>("Current", 0, 100, 0, v => slider.Current.Value = v);
            AddStep("Current to 0", () => slider.Current.Value = 0);
            AddStep("Current to 25", () => slider.Current.Value = 25);
            AddStep("Current to 50", () => slider.Current.Value = 50);
            AddStep("Current to 75", () => slider.Current.Value = 75);
            AddStep("Current to 100", () => slider.Current.Value = 100);
        }
    }
}
