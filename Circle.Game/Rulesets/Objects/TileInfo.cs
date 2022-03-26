using Circle.Game.Beatmaps;
using osuTK;

namespace Circle.Game.Rulesets.Objects
{
    public struct TileInfo : IHasTileInfo
    {
        public Actions[] Action { get; set; }

        public TileType TileType { get; set; }

        public float Angle { get; set; }

        public Vector2 Position { get; set; }
    }
}
