using Circle.Game.Beatmap;
using osu.Framework.Graphics;
using osuTK;

namespace Circle.Game.Rulesets.Objects
{
    public struct TileInfo : IHasTileInfo
    {
        public int Floor { get; set; }

        public EventType? EventType { get; set; }

        public TileType TileType { get; set; }

        public SpeedType? SpeedType { get; set; }

        public Easing Easing { get; set; }

        public bool Twirl { get; set; }

        public float Bpm { get; set; }

        public float BpmMultiplier { get; set; }

        public float Angle { get; set; }

        public Vector2 Position { get; set; }
    }
}
