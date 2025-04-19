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
                    Current = config.GetBindable<FrameSync>(FrameworkSetting.FrameSync),
                    Items = new[]
                    {
                        new StepperControlItem<FrameSync>("2x", FrameSync.Limit2x),
                        new StepperControlItem<FrameSync>("4x", FrameSync.Limit4x),
                        new StepperControlItem<FrameSync>("8x", FrameSync.Limit8x),
                        new StepperControlItem<FrameSync>(FrameSync.Unlimited),
                        new StepperControlItem<FrameSync>(FrameSync.VSync)
                    }
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
                        new StepperControlItem<bool>("Off", false),
                        new StepperControlItem<bool>("On", true)
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
                        new StepperControlItem<bool>("Off", false),
                        new StepperControlItem<bool>("On", true)
                    }
                },
                new CircleStepperControl<bool>
                {
                    LabelText = "Background parallax",
                    Current = localConfig.GetBindable<bool>(CircleSetting.Parallax),
                    Items = new[]
                    {
                        new StepperControlItem<bool>("Off", false),
                        new StepperControlItem<bool>("On", true)
                    }
                }
            });
        }
    }
}
