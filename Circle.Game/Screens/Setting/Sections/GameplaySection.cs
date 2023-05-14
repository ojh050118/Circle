#nullable disable

using Circle.Game.Configuration;
using Circle.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace Circle.Game.Screens.Setting.Sections
{
    public partial class GameplaySection : SettingsSection
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
                },
                new SettingsSlider<int>
                {
                    Text = "Tile front visibility distance",
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Current = localConfig.GetBindable<int>(CircleSetting.TileFrontDistance),
                    LeftIcon = FontAwesome.Solid.ArrowLeft,
                    RightIcon = FontAwesome.Solid.ArrowRight
                },
                new SettingsSlider<int>
                {
                    Text = "Tile back visibility distance",
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Current = localConfig.GetBindable<int>(CircleSetting.TileBackDistance),
                    LeftIcon = FontAwesome.Solid.ArrowLeft,
                    RightIcon = FontAwesome.Solid.ArrowRight
                },
            });
        }
    }
}
