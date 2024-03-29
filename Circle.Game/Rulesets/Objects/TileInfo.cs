#nullable disable

using Circle.Game.Beatmaps;
using osuTK;

namespace Circle.Game.Rulesets.Objects
{
    public class TileInfo : IHasTileInfo
    {
        public float Angle { get; set; }

        public Vector2 Position { get; set; }

        public bool Clockwise { get; set; }

        public float Bpm { get; set; }

        public bool PreviousClockwise { get; set; }

        public float PreviousAngle { get; set; }

        public float PreviousBpm { get; set; }
        public Actions[] Action { get; set; }

        public TileType TileType { get; set; }

        public override string ToString()
        {
            return $"Tile type: {TileType} | Action: {Action.Length}";
        }
    }
}
