using Circle.Game.Beatmaps;
using osu.Framework.Graphics;

namespace Circle.Game.Rulesets
{
    public class FilterTransform
    {
        public double StartTime { get; set; }

        public FilterType FilterType { get; set; }

        public bool Enabled { get; set; }

        public float? Intensity { get; set; }

        public double Duration { get; set; }

        public bool DisableOthers { get; set; }

        public Easing Easing { get; set; }

        public override string ToString()
        {
            return $"Filter Type: {FilterType} | Enabled: {Enabled} | Intensity: {Intensity}";
        }
    }
}
