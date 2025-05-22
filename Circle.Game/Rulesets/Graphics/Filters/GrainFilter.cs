using Circle.Game.Rulesets.Graphics.Shaders;
using osu.Framework.Graphics.Rendering;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class GrainFilter : CameraFilter, IHasIntensity, IHasTime
    {
        public float Intensity { get; set; }

        public float IntensityForShader => (float)(Intensity / 100f * 0.039999999105930328 - 0.019999999552965164);

        public float Time { get; set; }

        private IUniformBuffer<IntensityTimeParameters>? parameters;

        public GrainFilter()
            : base("grain")
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
