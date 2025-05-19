using osu.Framework.Graphics.Rendering;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class EmptyFilter : CameraFilter
    {
        public EmptyFilter()
            : base("empty")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
        }
    }
}
