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
        private void load(CircleGameBase gameBase, GameHost host, CircleConfigManager config)
        {
            FlowContent.AddRange(new Drawable[]
            {
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