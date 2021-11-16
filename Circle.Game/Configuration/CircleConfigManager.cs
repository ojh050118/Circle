using System.Collections.Generic;
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
            };
        }
    }

    public enum CircleSetting
    {
        Scale,
        FpsDisplay,
        Offset,
    }
}
