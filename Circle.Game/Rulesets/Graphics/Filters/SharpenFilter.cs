using Circle.Game.Rulesets.Graphics.Shaders;
using osu.Framework.Graphics.Rendering;
using osuTK;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class SharpenFilter : CameraFilter, IHasIntensity, IHasResolution
    {
        public float Intensity { get; set; } = 100f;

        public float IntensityForShader => Intensity / 100f;

        public Vector2 Resolution { get; set; }

        private IUniformBuffer<IntensityResolutionParameters>? parameters;

        public SharpenFilter()
            : base("sharpen")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            base.UpdateUniforms(renderer);

            parameters ??= renderer.CreateUniformBuffer<IntensityResolutionParameters>();

            parameters.Data = parameters.Data with { Intensity = IntensityForShader, Resolution = Resolution };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }
    }
}
