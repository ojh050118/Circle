#nullable disable

using Circle.Game.Beatmaps;

namespace Circle.Game.Rulesets.Objects
{
    public interface IHasTileInfo
    {
        ActionEvents[] Action { get; }

        TileType TileType { get; }
    }
}
