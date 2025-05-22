using Circle.Game.Rulesets.Graphics.Shaders;
using osu.Framework.Graphics.Rendering;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class AberrationFilter : CameraFilter, IHasIntensity
    {
        public float Intensity { get; set; }

        public float IntensityForShader => (float)(Intensity / 100f * 0.039999999105930328 - 0.019999999552965164);

        private IUniformBuffer<IntensityParameters>? parameters;

        public AberrationFilter()
            : base("aberration")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            base.UpdateUniforms(renderer);

            parameters ??= renderer.CreateUniformBuffer<IntensityParameters>();

            parameters.Data = new IntensityParameters { Intensity = IntensityForShader };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }
    }
}
