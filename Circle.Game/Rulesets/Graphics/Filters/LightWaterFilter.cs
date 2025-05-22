using Circle.Game.Rulesets.Graphics.Shaders;
using osu.Framework.Graphics.Rendering;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class LightWaterFilter : CameraFilter, IHasIntensity, IHasTime
    {
        public float Intensity { get; set; }

        public float IntensityForShader => Intensity / 100f * 2.4f;

        public float Time { get; set; }

        private IUniformBuffer<IntensityTimeParameters>? parameters;

        public LightWaterFilter()
            : base("lightwater")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            base.UpdateUniforms(renderer);

            parameters ??= renderer.CreateUniformBuffer<IntensityTimeParameters>();

            parameters.Data = parameters.Data with { Intensity = Intensity, Time = Time };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }
    }
}
