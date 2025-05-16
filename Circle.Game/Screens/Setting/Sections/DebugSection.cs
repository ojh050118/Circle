#nullable disable

using Circle.Game.Graphics.Sprites;
using Circle.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Platform;

namespace Circle.Game.Screens.Setting.Sections
{
    public partial class DebugSection : SettingsSection
    {
        public override string Header => "Debug";

        [BackgroundDependencyLoader]
        private void load(CircleGameBase gameBase, GameHost host)
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
            FlowContent.Add(new CircleSpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Text = $"running on osu!framework {gameBase.FrameworkVersion}"
            });
        }
    }
}
