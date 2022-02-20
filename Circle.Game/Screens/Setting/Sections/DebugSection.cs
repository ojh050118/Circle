using Circle.Game.Beatmap;
using Circle.Game.Configuration;
using Circle.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Platform;

namespace Circle.Game.Screens.Setting.Sections
{
    public class DebugSection : SettingsSection
    {
        public override string Header => "Debug";

        [BackgroundDependencyLoader]
        private void load(CircleGameBase gameBase, GameHost host, CircleConfigManager config, BeatmapManager beatmap)
        {
            FlowContent.AddRange(new Drawable[]
            {
                new Stepper<bool>
                {
                    Text = "Load beatmaps on startup",
                    Current = config.GetBindable<bool>(CircleSetting.LoadBeatmapsOnStartup),
                    Item = new[]
                    {
                        new StepperItem<bool>(CircleSetting.LoadBeatmapsOnStartup, true, "On"),
                        new StepperItem<bool>(CircleSetting.LoadBeatmapsOnStartup, false, "Off")
                    }
                },
                new BoxButton
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Text = "Reload beatmaps",
                    Action = beatmap.ReloadBeatmaps
                },
                new BoxButton
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Text = "Clear all caches",
                    Action = host.Collect
                }
            });
            FlowContent.Add(new SpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Text = $"running on osu!framework {gameBase.FrameworkVersion}"
            });
        }
    }
}
