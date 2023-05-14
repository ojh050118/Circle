#nullable disable

using Circle.Game.Screens.Setting;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;

namespace Circle.Game.Tests.Visual.UserInterface
{
    public class TestSceneSettingsSlider : CircleTestScene
    {
        public TestSceneSettingsSlider()
        {
            var value = new BindableNumber<float>
            {
                MinValue = 0,
                MaxValue = 100,
            };

            Add(new SettingsSlider<float>
            {
                LeftIcon = FontAwesome.Solid.VolumeDown,
                RightIcon = FontAwesome.Solid.VolumeUp,
                Text = "volume",
                Current = value,
                KeyboardStep = 5
            });
        }
    }
}
