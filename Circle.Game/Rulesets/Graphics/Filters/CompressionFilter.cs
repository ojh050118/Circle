using Circle.Game.Rulesets.Graphics.Shaders;
using osu.Framework.Graphics.Rendering;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class CompressionFilter : CameraFilter, IHasIntensity, IHasTime
    {
        public float Intensity { get; set; }

        public float IntensityForShader => Intensity / 100f;

        public float Time { get; set; }

        private IUniformBuffer<IntensityTimeParameters>? parameters;

        public CompressionFilter()
            : base("compression")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            base.UpdateUniforms(renderer);

            parameters ??= renderer.CreateUniformBuffer<IntensityTimeParameters>();

            parameters.Data = new IntensityTimeParameters { Intensity = IntensityForShader, Time = Time };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }
    }
}
