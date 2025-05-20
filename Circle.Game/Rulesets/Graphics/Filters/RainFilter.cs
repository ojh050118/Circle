using Circle.Game.Rulesets.Graphics.Shaders;
using osu.Framework.Graphics.Rendering;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class RainFilter : CameraFilter, IHasIntensity, IHasTime
    {
        public float Intensity { get; set; }
        public float Time { get; set; }

        private IUniformBuffer<IntensityTimeTextureRectParameters>? parameters;

        public RainFilter()
            : base("rain", 1, "Rain")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            parameters ??= renderer.CreateUniformBuffer<IntensityTimeTextureRectParameters>();

            parameters.Data = new IntensityTimeTextureRectParameters { Intensity = Intensity, Time = Time };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }
    }
}
