using Circle.Game.Rulesets.Graphics.Shaders;
using osu.Framework.Graphics.Rendering;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class GlitchFilter : CameraFilter, IHasTime
    {
        public float Time { get; set; }

        private IUniformBuffer<TimeParameters>? parameters;

        public GlitchFilter()
            : base("glitch")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            base.UpdateUniforms(renderer);

            parameters ??= renderer.CreateUniformBuffer<TimeParameters>();

            parameters.Data = parameters.Data with { Time = Time };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }
    }
}
