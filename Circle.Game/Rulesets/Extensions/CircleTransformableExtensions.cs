#nullable disable

using Circle.Game.Rulesets.Objects;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Transforms;

namespace Circle.Game.Rulesets.Extensions
{
    public static class CircleTransformableExtensions
    {
        public static TransformSequence<T> ExpandTo<T>(this T planet, float value, double duration = 0, Easing easing = Easing.None)
            where T : Planet
        {
            return planet.TransformTo(nameof(planet.Expansion), value, duration, easing);
        }
    }
}
