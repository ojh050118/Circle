using System.Collections.Generic;
using osu.Framework.Configuration;
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
            SetDefault(CircleSetting.Scale, 1f, 0.8f, 2f);
            SetDefault(CircleSetting.FpsDisplay, false);
            SetDefault(CircleSetting.Offset, 0);
        }
    }

    public enum CircleSetting
    {
        Scale,
        FpsDisplay,
        Offset,
    }
}
