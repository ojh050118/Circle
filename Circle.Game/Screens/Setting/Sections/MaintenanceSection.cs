using System.Threading.Tasks;
using Circle.Game.Beatmaps;
using Circle.Game.Graphics.UserInterface;
using Circle.Game.Overlays;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace Circle.Game.Screens.Setting.Sections
{
    public class MaintenanceSection : SettingsSection
    {
        public override string Header => "Maintenance";

        [BackgroundDependencyLoader]
        private void load(ImportOverlay import, BeatmapManager beatmap)
        {
            FlowContent.AddRange(new Drawable[]
            {
                new BoxButton
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Text = "Migrate to a new storage method",
                    Action = () => Task.Factory.StartNew(beatmap.Migrate, TaskCreationOptions.LongRunning)
                },
                new BoxButton
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Text = "Import beatmap",
                    Action = import.Show
                },
                new BoxButton
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Text = "Reload beatmaps",
                    Action = () => Task.Factory.StartNew(beatmap.ReloadBeatmaps, TaskCreationOptions.LongRunning)
                },
            });
        }
    }
}
