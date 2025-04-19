#nullable disable

using System;
using System.Linq;
using Circle.Game.Configuration;
using Circle.Game.Graphics.UserInterface;
using osu.Framework;
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

        private SettingsEnumDropdown<RendererType> renderer;

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
                renderer = new SettingsEnumDropdown<RendererType>
                {
                    Text = "Renderer",
                    Current = config.GetBindable<RendererType>(FrameworkSetting.Renderer)
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

        protected override void LoadComplete()
        {
            var availableRenderers = Enum.GetValues<RendererType>().Except(new[] { RendererType.Vulkan, RendererType.Deferred_Vulkan });

            switch (RuntimeInfo.OS)
            {
                case RuntimeInfo.Platform.Windows:
                    availableRenderers = availableRenderers.Except(new[] { RendererType.Metal, RendererType.Deferred_Metal });
                    break;

                case RuntimeInfo.Platform.Linux:
                    availableRenderers = availableRenderers.Except(new[] { RendererType.Direct3D11, RendererType.Deferred_Direct3D11, RendererType.Metal, RendererType.Deferred_Metal });
                    break;

                case RuntimeInfo.Platform.macOS:
                    availableRenderers = availableRenderers.Except(new[] { RendererType.Direct3D11, RendererType.Deferred_Direct3D11 });
                    break;

                case RuntimeInfo.Platform.Android:
                    availableRenderers = availableRenderers.Except(new[] { RendererType.Direct3D11, RendererType.Deferred_Direct3D11, RendererType.Metal, RendererType.Deferred_Metal });
                    break;

                case RuntimeInfo.Platform.iOS:
                    availableRenderers = availableRenderers.Except(new[] { RendererType.Direct3D11, RendererType.Deferred_Direct3D11 });
                    break;
            }

            renderer.Items = availableRenderers;
        }
    }
}
