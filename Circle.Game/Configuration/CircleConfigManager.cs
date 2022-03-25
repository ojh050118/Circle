using System.Collections.Generic;
using Circle.Game.Utils;
using osu.Framework.Configuration;
using osu.Framework.Configuration.Tracking;
using osu.Framework.Platform;

namespace Circle.Game.Configuration
{
    public class CircleConfigManager : IniConfigManager<CircleSetting>
    {
        protected override string Filename => @"Circle.ini";

        public CircleConfigManager(Storage storage, IDictionary<CircleSetting, object> defaultOverrides = null)
            : base(storage, defaultOverrides)
        {
        }

        protected override void InitialiseDefaults()
        {
            SetDefault(CircleSetting.Scale, 1f, 0.8f, 1.6f);
            SetDefault(CircleSetting.FpsDisplay, false);
            SetDefault(CircleSetting.Offset, 0);
            SetDefault(CircleSetting.LoadBeatmapsOnStartup, true);
            SetDefault(CircleSetting.BlurVisibility, true);
            SetDefault(CircleSetting.Parallax, true);
            SetDefault(CircleSetting.PlanetRed, Color4Enum.Red);
            SetDefault(CircleSetting.PlanetBlue, Color4Enum.DeepSkyBlue);
        }

        public override TrackedSettings CreateTrackedSettings()
        {
            return new TrackedSettings
            {
                new TrackedSetting<float>(CircleSetting.Scale, scale => new SettingDescription(
                        rawValue: scale,
                        name: "Scale",
                        value: $"{scale}x"
                    )
                ),
                new TrackedSetting<bool>(CircleSetting.FpsDisplay, visibility => new SettingDescription(
                        rawValue: visibility,
                        name: "FpsDisplay",
                        value: $"{(visibility ? "On" : "Off")}"
                    )
                ),
                new TrackedSetting<int>(CircleSetting.Offset, offset => new SettingDescription(
                        rawValue: offset,
                        name: "Offset",
                        value: $"{offset}"
                    )
                ),
                new TrackedSetting<bool>(CircleSetting.LoadBeatmapsOnStartup, loadBeatmap => new SettingDescription(
                        rawValue: loadBeatmap,
                        name: "LoadBeatmapsOnStartup",
                        value: $"{loadBeatmap}"
                    )
                ),
                new TrackedSetting<bool>(CircleSetting.BlurVisibility, blur => new SettingDescription(
                        rawValue: blur,
                        name: "BlurVisibility",
                        value: $"{blur}"
                    )
                ),
                new TrackedSetting<bool>(CircleSetting.Parallax, papallax => new SettingDescription(
                        rawValue: papallax,
                        name: "Parallax",
                        value: $"{papallax}"
                    )
                )
            };
        }
    }

    public enum CircleSetting
    {
        Scale,
        FpsDisplay,
        Offset,
        LoadBeatmapsOnStartup,
        BlurVisibility,
        Parallax,
        PlanetRed,
        PlanetBlue
    }
}
