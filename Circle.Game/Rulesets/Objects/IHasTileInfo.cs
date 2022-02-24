using Circle.Game.Beatmaps;
using osu.Framework.Graphics;

namespace Circle.Game.Rulesets.Objects
{
    public interface IHasTileInfo
    {
        int Floor { get; }

        EventType? EventType { get; }

        TileType TileType { get; }

        SpeedType? SpeedType { get; }

        Easing Easing { get; }

        bool Twirl { get; }

        float Bpm { get; }

        float BpmMultiplier { get; }

        float Angle { get; }
    }
}
