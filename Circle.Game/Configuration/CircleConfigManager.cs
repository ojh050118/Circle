using System.Collections.Generic;
using Circle.Game.Utils;
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
            SetDefault(CircleSetting.Scale, 1f, 0.8f, 1.6f);
            SetDefault(CircleSetting.FpsDisplay, false);
            SetDefault(CircleSetting.Offset, 0);
            SetDefault(CircleSetting.LoadBeatmapsOnStartup, true);
            SetDefault(CircleSetting.BlurVisibility, true);
            SetDefault(CircleSetting.Parallax, true);
            SetDefault(CircleSetting.PlanetRed, Color4Enum.Red);
            SetDefault(CircleSetting.PlanetBlue, Color4Enum.DeepSkyBlue);
            SetDefault(CircleSetting.TileFrontDistance, 8, 0, 500);
            SetDefault(CircleSetting.TileBackDistance, 4, 0, 500);
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
        PlanetBlue,
        TileFrontDistance,
        TileBackDistance
    }
}
