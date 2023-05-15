#nullable disable

using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace Circle.Game.Screens.Setting.Sections
{
    public partial class AudioSection : SettingsSection
    {
        public override string Header => "Audio";

        [BackgroundDependencyLoader]
        private void load(FrameworkConfigManager config)
        {
            FlowContent.AddRange(new Drawable[]
            {
                new SettingsSlider<double>
                {
                    Text = "Master",
                    Current = config.GetBindable<double>(FrameworkSetting.VolumeUniversal),
                    LeftIcon = FontAwesome.Solid.VolumeDown,
                    RightIcon = FontAwesome.Solid.VolumeUp,
                },
                new SettingsSlider<double>
                {
                    Text = "Effect",
                    Current = config.GetBindable<double>(FrameworkSetting.VolumeEffect),
                    LeftIcon = FontAwesome.Solid.VolumeDown,
                    RightIcon = FontAwesome.Solid.VolumeUp,
                },
                new SettingsSlider<double>
                {
                    Text = "Music",
                    Current = config.GetBindable<double>(FrameworkSetting.VolumeMusic),
                    LeftIcon = FontAwesome.Solid.VolumeDown,
                    RightIcon = FontAwesome.Solid.VolumeUp,
                },
            });
        }
    }
}
