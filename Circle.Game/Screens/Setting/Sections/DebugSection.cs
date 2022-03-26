using Circle.Game.Beatmaps;
using Circle.Game.Configuration;
using Circle.Game.Graphics.UserInterface;
using Circle.Game.Overlays;
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
        private void load(CircleGameBase gameBase, GameHost host, CircleConfigManager config, BeatmapManager beatmap, ImportOverlay import)
        {
            FlowContent.AddRange(new Drawable[]
            {
                new Stepper<bool>(config.Get<bool>(CircleSetting.LoadBeatmapsOnStartup))
                {
                    Text = "Load beatmaps on startup",
                    Items = new[]
                    {
                        new StepperItem<bool>("On", true),
                        new StepperItem<bool>("Off", false)
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
            FlowContent.Add(new SpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Text = $"running on osu!framework {gameBase.FrameworkVersion}"
            });
        }
    }
}
