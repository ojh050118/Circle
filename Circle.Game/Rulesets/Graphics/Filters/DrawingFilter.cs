using Circle.Game.Rulesets.Graphics.Shaders;
using osu.Framework.Graphics.Rendering;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class DrawingFilter : CameraFilter, IHasIntensity, IHasTime
    {
        public float Intensity { get; set; }
        public float Time { get; set; }

        private IUniformBuffer<IntensityTimeTextureRectParameters>? parameters;

        public DrawingFilter()
            : base("drawing", 1, "Drawing")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            base.UpdateUniforms(renderer);

            parameters ??= renderer.CreateUniformBuffer<IntensityTimeTextureRectParameters>();

            parameters.Data = parameters.Data with { Intensity = Intensity, Time = Time, TextureRect = TextureRects![0] };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }
    }
}
