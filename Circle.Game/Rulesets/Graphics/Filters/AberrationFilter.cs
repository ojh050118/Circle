using Circle.Game.Rulesets.Graphics.Shaders;
using osu.Framework.Graphics.Rendering;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class AberrationFilter : CameraFilter, IHasIntensity
    {
        public float Intensity { get; set; }

        private IUniformBuffer<IntensityParameters>? parameters;

        public AberrationFilter()
            : base("aberration")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            parameters ??= renderer.CreateUniformBuffer<IntensityParameters>();

            parameters.Data = new IntensityParameters { Intensity = Intensity };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }
    }
}
