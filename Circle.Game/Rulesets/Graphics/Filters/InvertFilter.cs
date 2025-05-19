using osu.Framework.Graphics.Rendering;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class InvertFilter : CameraFilter
    {
        public InvertFilter()
            : base("invert")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
        }
    }
}
