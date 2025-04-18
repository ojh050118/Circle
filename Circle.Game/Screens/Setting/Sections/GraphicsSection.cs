#nullable disable

using Circle.Game.Configuration;
using Circle.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Platform;

namespace Circle.Game.Screens.Setting.Sections
{
    public partial class GraphicsSection : SettingsSection
    {
        public override string Header => "Graphics";

        [BackgroundDependencyLoader]
        private void load(CircleConfigManager localConfig, FrameworkConfigManager config)
        {
            FlowContent.AddRange(new Drawable[]
            {
                new CircleEnumStepperControl<WindowMode>
                {
                    LabelText = "Screen mode",
                    Current = config.GetBindable<WindowMode>(FrameworkSetting.WindowMode)
                },
                new CircleEnumStepperControl<FrameSync>
                {
                    LabelText = "Frame limiter",
                    Current = config.GetBindable<FrameSync>(FrameworkSetting.FrameSync)
                },
                new CircleEnumStepperControl<ExecutionMode>
                {
                    LabelText = "Threading mode",
                    Current = config.GetBindable<ExecutionMode>(FrameworkSetting.ExecutionMode)
                },
                new CircleStepperControl<bool>
                {
                    LabelText = "Frame overlay",
                    Current = localConfig.GetBindable<bool>(CircleSetting.FpsDisplay),
                    Items = new[]
                    {
                        true,
                        false
                    }
                },
                new SettingsSlider<float>
                {
                    Text = "UI scaling",
                    Current = localConfig.GetBindable<float>(CircleSetting.Scale),
                    LeftIcon = FontAwesome.Solid.Compress,
                    RightIcon = FontAwesome.Solid.Expand,
                    TransferValueOnCommit = true,
                },
                new CircleStepperControl<bool>
                {
                    LabelText = "Overlay background blur",
                    Current = localConfig.GetBindable<bool>(CircleSetting.BlurVisibility),
                    Items = new[]
                    {
                        false,
                        true,
                    }
                },
                new CircleStepperControl<bool>
                {
                    LabelText = "Background parallax",
                    Current = localConfig.GetBindable<bool>(CircleSetting.Parallax),
                    Items = new[]
                    {
                        false,
                        true
                    }
                }
            });
        }
    }
}
