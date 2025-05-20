using Circle.Game.Rulesets.Graphics.Shaders;
using osu.Framework.Graphics.Rendering;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class PosterizeFilter : CameraFilter, IHasIntensity
    {
        public float Intensity { get; set; }

        private IUniformBuffer<IntensityParameters>? parameters;

        public PosterizeFilter()
            : base("posterize")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            base.UpdateUniforms(renderer);

            parameters ??= renderer.CreateUniformBuffer<IntensityParameters>();

            parameters.Data = parameters.Data with { Intensity = Intensity };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }
    }
}
