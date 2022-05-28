using Circle.Game.Beatmaps;
using osuTK;

namespace Circle.Game.Rulesets
{
    public class CameraTransform
    {
        public Actions Action { get; set; }

        public double StartTime { get; set; }

        public double Duration { get; set; }

        public Vector2? Position { get; set; }

        public float? Rotation { get; set; }

        public Relativity? RelativeTo { get; set; }

        public float? Zoom { get; set; }
    }
}
