using Circle.Game.Configuration;
using Circle.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace Circle.Game.Screens.Setting.Sections
{
    public class GameplaySection : SettingsSection
    {
        public override string Header => "Gameplay";

        [BackgroundDependencyLoader]
        private void load(CircleConfigManager localConfig)
        {
            FlowContent.AddRange(new Drawable[]
            {
                new SettingsEnumDropdown<Color4Enum>
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Text = "Red planet color",
                    Current = localConfig.GetBindable<Color4Enum>(CircleSetting.PlanetRed)
                },
                new SettingsEnumDropdown<Color4Enum>
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Text = "Blue planet color",
                    Current = localConfig.GetBindable<Color4Enum>(CircleSetting.PlanetBlue)
                }
            });
        }
    }
}
