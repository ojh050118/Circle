#nullable disable

using Circle.Game.Screens.Setting;
using Circle.Game.Utils;
using osu.Framework.Graphics;

namespace Circle.Game.Tests.Visual.UserInterface
{
    public class TestSceneSettingsDropdown : CircleTestScene
    {
        public TestSceneSettingsDropdown()
        {
            Add(new SettingsEnumDropdown<Color4Enum>
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Text = "Red planet color",
            });
        }
    }
}
