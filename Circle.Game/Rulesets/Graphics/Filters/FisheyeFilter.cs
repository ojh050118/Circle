using Circle.Game.Rulesets.Graphics.Shaders;
using osu.Framework.Graphics.Rendering;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class FisheyeFilter : CameraFilter, IHasIntensity
    {
        public float Intensity { get; set; } = 50f;

        public float IntensityForShader => Intensity / 100f;

        private IUniformBuffer<IntensityParameters>? parameters;

        public FisheyeFilter()
            : base("fisheye")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            base.UpdateUniforms(renderer);

            parameters ??= renderer.CreateUniformBuffer<IntensityParameters>();

            parameters.Data = parameters.Data with { Intensity = IntensityForShader };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }
    }
}
