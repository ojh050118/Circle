using Circle.Game.Rulesets.Graphics.Shaders;
using osu.Framework.Graphics.Rendering;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class RainFilter : CameraFilter, IHasIntensity, IHasTime
    {
        public float Intensity { get; set; }

        public float IntensityForShader => Intensity / 100f * 0.5f;

        public float Time { get; set; }

        private IUniformBuffer<IntensityTimeTextureRectParameters>? parameters;

        public RainFilter()
            : base("rain", 1, "Rain")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            base.UpdateUniforms(renderer);

            parameters ??= renderer.CreateUniformBuffer<IntensityTimeTextureRectParameters>();

            parameters.Data = parameters.Data with { Intensity = IntensityForShader, Time = Time, TextureRect = TextureRects![0] };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }
    }
}
