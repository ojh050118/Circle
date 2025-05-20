using Circle.Game.Rulesets.Graphics.Shaders;
using osu.Framework.Graphics.Rendering;
using osuTK;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class OilPaintFilter : CameraFilter, IHasIntensity, IHasResolution
    {
        public float Intensity { get; set; }
        public Vector2 Resolution { get; set; }

        private IUniformBuffer<IntensityResolutionParameters>? parameters;

        public OilPaintFilter()
            : base("oilpaint")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            base.UpdateUniforms(renderer);

            parameters ??= renderer.CreateUniformBuffer<IntensityResolutionParameters>();

            parameters.Data = parameters.Data with { Intensity = Intensity, Resolution = Resolution };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }
    }
}
