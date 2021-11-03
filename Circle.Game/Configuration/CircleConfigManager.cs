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
    }

    public enum CircleSetting
    {
    }
}
