using System;
using Circle.Game.Screens.Setting;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osuTK.Graphics;

namespace Circle.Game.Tests.Visual.UserInterface
{
    public class TestSceneSettingsDropdown : CircleTestScene
    {
        public TestSceneSettingsDropdown()
        {
            SettingsDropdown<Color4> dropdown;

            Add(dropdown = new SettingsDropdown<Color4>
            {
                Text = "Red planet color",
            });

            foreach (Color4 color in Enum.GetValues(typeof(Color4)))
                dropdown.AddDropdownItem(color);
        }
    }
}
