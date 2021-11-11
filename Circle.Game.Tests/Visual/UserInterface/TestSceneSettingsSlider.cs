using Circle.Game.Screens.Setting;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK.Graphics;

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
