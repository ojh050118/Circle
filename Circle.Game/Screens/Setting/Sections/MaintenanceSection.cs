using System.Threading.Tasks;
using Circle.Game.Beatmaps;
using Circle.Game.Graphics.UserInterface;
using Circle.Game.Overlays;
using Circle.Game.Overlays.OSD;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osuTK.Graphics;

namespace Circle.Game.Screens.Setting.Sections
{
    public class MaintenanceSection : SettingsSection
    {
        public override string Header => "Maintenance";

        [BackgroundDependencyLoader]
        private void load(ImportOverlay import, BeatmapManager beatmap, Toast toast)
        {
            FlowContent.AddRange(new Drawable[]
            {
                new BoxButton
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Text = "Migrate to a new storage method",
                    Action = () => Task.Factory.StartNew(() =>
                    {
                        toast.Push(new ToastInfo
                        {
                            Description = "Migration",
                            SubDescription = "Migrating to a new storage method...",
                            Icon = FontAwesome.Solid.Plane,
                            IconColour = Color4.DeepSkyBlue
                        });
                        beatmap.Migrate();
                        toast.Push(new ToastInfo
                        {
                            Description = "Migration",
                            SubDescription = "Migration successful! Check out what's new!",
                            Icon = FontAwesome.Solid.Check,
                            IconColour = Color4.LightGreen
                        });
                        beatmap.ReloadBeatmaps();
                    }, TaskCreationOptions.LongRunning)
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
