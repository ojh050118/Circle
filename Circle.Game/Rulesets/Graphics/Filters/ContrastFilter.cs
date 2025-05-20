using Circle.Game.Rulesets.Graphics.Shaders;
using osu.Framework.Graphics.Rendering;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class ContrastFilter : CameraFilter, IHasIntensity
    {
        public float Intensity { get; set; }

        private IUniformBuffer<IntensityParameters>? parameters;

        public ContrastFilter()
            : base("contrast")
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
