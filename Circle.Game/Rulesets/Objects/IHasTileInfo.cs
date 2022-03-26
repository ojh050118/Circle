using Circle.Game.Beatmaps;

namespace Circle.Game.Rulesets.Objects
{
    public interface IHasTileInfo
    {
        Actions[] Action { get; }

        TileType TileType { get; }
    }
}
