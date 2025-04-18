﻿#nullable disable

using Circle.Game.Beatmaps;
using Circle.Game.Configuration;
using Circle.Game.Graphics.Sprites;
using Circle.Game.Graphics.UserInterface;
using Circle.Game.Overlays;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Platform;

namespace Circle.Game.Screens.Setting.Sections
{
    public partial class DebugSection : SettingsSection
    {
        public override string Header => "Debug";

        [BackgroundDependencyLoader]
        private void load(CircleGameBase gameBase, GameHost host, CircleConfigManager config, BeatmapManager beatmap, ImportOverlay import)
        {
            FlowContent.AddRange(new Drawable[]
            {
                new CircleStepperControl<bool>
                {
                    LabelText = "Load beatmaps on startup",
                    Current = config.GetBindable<bool>(CircleSetting.LoadBeatmapsOnStartup),
                    Items = new[]
                    {
                        new StepperControlItem<bool>("Off", false),
                        new StepperControlItem<bool>("On", true)
                    }
                },
                new BoxButton
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Text = "Clear all caches",
                    Action = host.Collect
                }
            });
            FlowContent.Add(new CircleSpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Text = $"running on osu!framework {gameBase.FrameworkVersion}"
            });
        }
    }
}
