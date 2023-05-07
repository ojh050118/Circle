using Circle.Game.Beatmaps;
using osu.Framework.Graphics;
using osuTK;

namespace Circle.Game.Rulesets
{
    public class CameraTransform
    {
        public double StartTime { get; set; }

        public double Duration { get; set; }

        public Vector2? Position { get; set; }

        public float? Rotation { get; set; }

        public Relativity? RelativeTo { get; set; }

        public float? Zoom { get; set; }

        public Easing Easing { get; set; }

        public override string ToString()
        {
            string position = "null";

            if (Position.HasValue)
                position = $"({Position.Value.X}, {Position.Value.Y})";

            return $"RelativeTo: {RelativeTo.ToString()} | Position: {position} | Rotation: {Rotation} | Zoom: {Zoom}";
        }
    }
}
