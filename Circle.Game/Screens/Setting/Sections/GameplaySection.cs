using Circle.Game.Configuration;
using Circle.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osuTK.Graphics;

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
                new SettingsDropdown<Color4>
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Text = "Red planet color"
                },
                new SettingsDropdown<Color4>
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Text = "Blue planet color"
                }
            });
        }
    }
}
