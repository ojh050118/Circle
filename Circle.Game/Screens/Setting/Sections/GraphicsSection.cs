using Circle.Game.Configuration;
using Circle.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Platform;

namespace Circle.Game.Screens.Setting.Sections
{
    public class GraphicsSection : SettingsSection
    {
        public override string Header => "Graphics";

        [BackgroundDependencyLoader]
        private void load(CircleConfigManager localConfig, FrameworkConfigManager config)
        {
            FlowContent.AddRange(new Drawable[]
            {
                new Stepper<WindowMode>(config.Get<WindowMode>(FrameworkSetting.WindowMode))
                {
                    Text = "Screen mode",
                    Items = new[]
                    {
                        new StepperItem<WindowMode>(FrameworkSetting.WindowMode, WindowMode.Windowed),
                        new StepperItem<WindowMode>(FrameworkSetting.WindowMode, WindowMode.Borderless),
                        new StepperItem<WindowMode>(FrameworkSetting.WindowMode, WindowMode.Fullscreen)
                    }
                },
                new Stepper<FrameSync>(config.Get<FrameSync>(FrameworkSetting.FrameSync))
                {
                    Text = "Frame limiter",
                    Items = new[]
                    {
                        new StepperItem<FrameSync>(FrameworkSetting.FrameSync, FrameSync.Limit2x, "2x"),
                        new StepperItem<FrameSync>(FrameworkSetting.FrameSync, FrameSync.Limit4x, "4x"),
                        new StepperItem<FrameSync>(FrameworkSetting.FrameSync, FrameSync.Limit8x, "8x"),
                        new StepperItem<FrameSync>(FrameworkSetting.FrameSync, FrameSync.Unlimited),
                        new StepperItem<FrameSync>(FrameworkSetting.FrameSync, FrameSync.VSync)
                    }
                },
                new Stepper<ExecutionMode>(config.Get<ExecutionMode>(FrameworkSetting.ExecutionMode))
                {
                    Text = "Threading mode",
                    Items = new[]
                    {
                        new StepperItem<ExecutionMode>(FrameworkSetting.ExecutionMode, ExecutionMode.MultiThreaded),
                        new StepperItem<ExecutionMode>(FrameworkSetting.ExecutionMode, ExecutionMode.SingleThread)
                    }
                },
                new Stepper<bool>(localConfig.Get<bool>(CircleSetting.FpsDisplay))
                {
                    Text = "Frame overlay",
                    Items = new[]
                    {
                        new StepperItem<bool>(CircleSetting.FpsDisplay, true, "On"),
                        new StepperItem<bool>(CircleSetting.FpsDisplay, false, "Off"),
                    }
                },
                new SettingsSlider<float>
                {
                    Text = "UI scaling",
                    Current = localConfig.GetBindable<float>(CircleSetting.Scale),
                    LeftIcon = FontAwesome.Solid.Compress,
                    RightIcon = FontAwesome.Solid.Expand,
                    TransferValueOnCommit = true,
                }
            });
        }
    }
}
