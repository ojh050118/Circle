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
                new EnumStepper<WindowMode>
                {
                    LabelText = "Screen mode",
                    Current = config.GetBindable<WindowMode>(FrameworkSetting.WindowMode)
                },
                new Stepper<FrameSync>
                {
                    LabelText = "Frame limiter",
                    Current = config.GetBindable<FrameSync>(FrameworkSetting.FrameSync),
                    Items = new[]
                    {
                        new StepperItem<FrameSync>("2x", FrameSync.Limit2x),
                        new StepperItem<FrameSync>("4x", FrameSync.Limit4x),
                        new StepperItem<FrameSync>("8x", FrameSync.Limit8x),
                        new StepperItem<FrameSync>(FrameSync.Unlimited),
                        new StepperItem<FrameSync>(FrameSync.VSync)
                    }
                },
                new EnumStepper<ExecutionMode>
                {
                    LabelText = "Threading mode",
                    Current = config.GetBindable<ExecutionMode>(FrameworkSetting.ExecutionMode)
                },
                new Stepper<bool>
                {
                    LabelText = "Frame overlay",
                    Current = localConfig.GetBindable<bool>(CircleSetting.FpsDisplay),
                    Items = new[]
                    {
                        new StepperItem<bool>("On", true),
                        new StepperItem<bool>("Off", false),
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
                new Stepper<bool>
                {
                    LabelText = "Overlay background blur",
                    Current = localConfig.GetBindable<bool>(CircleSetting.BlurVisibility),
                    Items = new[]
                    {
                        new StepperItem<bool>("Off", false),
                        new StepperItem<bool>("On", true),
                    }
                },
                new Stepper<bool>
                {
                    LabelText = "Background parallax",
                    Current = localConfig.GetBindable<bool>(CircleSetting.Parallax),
                    Items = new[]
                    {
                        new StepperItem<bool>("Off", false),
                        new StepperItem<bool>("On", true),
                    }
                }
            });
        }
    }
}
