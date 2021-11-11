using System;
using System.Collections.Generic;
using System.Text;
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
                new RollingControl<WindowMode>
                {
                    Text = "Screen mode",
                    Current = config.GetBindable<WindowMode>(FrameworkSetting.WindowMode),
                    Item = new[]
                    {
                        new RollingItem<WindowMode>(FrameworkSetting.WindowMode, WindowMode.Windowed),
                        new RollingItem<WindowMode>(FrameworkSetting.WindowMode, WindowMode.Borderless),
                        new RollingItem<WindowMode>(FrameworkSetting.WindowMode, WindowMode.Fullscreen)
                    }
                },
                new RollingControl<FrameSync>
                {
                    Text = "Frame limiter",
                    Current = config.GetBindable<FrameSync>(FrameworkSetting.FrameSync),
                    Item = new[]
                    {
                        new RollingItem<FrameSync>(FrameworkSetting.FrameSync, FrameSync.Limit2x, "2x"),
                        new RollingItem<FrameSync>(FrameworkSetting.FrameSync, FrameSync.Limit4x, "4x"),
                        new RollingItem<FrameSync>(FrameworkSetting.FrameSync, FrameSync.Limit8x, "8x"),
                        new RollingItem<FrameSync>(FrameworkSetting.FrameSync, FrameSync.Unlimited),
                        new RollingItem<FrameSync>(FrameworkSetting.FrameSync, FrameSync.VSync)
                    }
                },
                new RollingControl<ExecutionMode>
                {
                    Text = "Threading mode",
                    Current = config.GetBindable<ExecutionMode>(FrameworkSetting.ExecutionMode),
                    Item = new[]
                    {
                        new RollingItem<ExecutionMode>(FrameworkSetting.ExecutionMode, ExecutionMode.MultiThreaded),
                        new RollingItem<ExecutionMode>(FrameworkSetting.ExecutionMode, ExecutionMode.SingleThread)
                    }
                },
                new RollingControl<bool>
                {
                    Text = "Frame overlay",
                    Current = localConfig.GetBindable<bool>(CircleSetting.FpsDisplay),
                    Item = new[]
                    {
                        new RollingItem<bool>(CircleSetting.FpsDisplay, true, "On"),
                        new RollingItem<bool>(CircleSetting.FpsDisplay, false, "Off"),
                    }
                },
                new SettingsSlider<float>
                {
                    Text = "Scale",
                    Current = localConfig.GetBindable<float>(CircleSetting.Scale),
                    LeftIcon = FontAwesome.Solid.Compress,
                    RightIcon = FontAwesome.Solid.Expand,
                    TransferValueOnCommit = true,
                }
            });
        }
    }
}
