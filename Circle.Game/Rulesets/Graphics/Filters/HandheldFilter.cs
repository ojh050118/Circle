using osu.Framework.Graphics.Rendering;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class HandheldFilter : CameraFilter
    {
        public HandheldFilter()
            : base("handheld")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
        }
    }
}
