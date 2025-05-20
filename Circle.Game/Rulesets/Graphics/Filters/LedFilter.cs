using Circle.Game.Rulesets.Graphics.Shaders;
using osu.Framework.Graphics.Rendering;
using osuTK;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class LedFilter : CameraFilter, IHasIntensity, IHasTime, IHasResolution
    {
        public float Intensity { get; set; }
        public float Time { get; set; }
        public Vector2 Resolution { get; set; }

        private IUniformBuffer<IntensityTimeResolutionParameters>? parameters;

        public LedFilter()
            : base("led")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            base.UpdateUniforms(renderer);

            parameters ??= renderer.CreateUniformBuffer<IntensityTimeResolutionParameters>();

            parameters.Data = parameters.Data with { Intensity = Intensity, Time = Time, Resolution = Resolution };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }
    }
}
