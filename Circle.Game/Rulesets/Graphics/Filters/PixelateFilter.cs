using Circle.Game.Rulesets.Graphics.Shaders;
using osu.Framework.Graphics.Rendering;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class PixelateFilter : CameraFilter, IHasIntensity
    {
        public override bool Enabled => base.Enabled && Intensity > 0;

        public float Intensity { get; set; }

        public float IntensityForShader => Intensity / 100f;

        private IUniformBuffer<IntensityParameters>? parameters;

        public PixelateFilter()
            : base("pixelate")
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
