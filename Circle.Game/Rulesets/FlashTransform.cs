using Circle.Game.Beatmaps;
using osuTK.Graphics;

namespace Circle.Game.Rulesets
{
    public class FlashTransform
    {
        public double StartTime { get; set; }

        public FlashPlane Plane { get; set; }

        public Color4 StartColor { get; set; }

        public float StartOpacity { get; set; }

        public Color4 EndColor { get; set; }

        public float EndOpacity { get; set; }

        public double Duration { get; set; }

        public override string ToString()
        {
            return $"Plane Type: {Plane}";
        }
    }
}
