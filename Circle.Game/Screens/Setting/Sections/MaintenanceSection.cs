﻿#nullable disable

using Circle.Game.Beatmaps;
using Circle.Game.Graphics.UserInterface;
using Circle.Game.Overlays;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace Circle.Game.Screens.Setting.Sections
{
    public partial class MaintenanceSection : SettingsSection
    {
        public override string Header => "Maintenance";

        [BackgroundDependencyLoader]
        private void load(ImportOverlay import, ConvertOverlay convert, BeatmapManager beatmap)
        {
            FlowContent.AddRange(new Drawable[]
            {
                new BoxButton
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Text = "Convert Beatmaps",
                    Action = convert.Show
                },
                new BoxButton
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Text = "Import beatmap",
                    Action = import.Show
                }
            });
        }
    }
}
