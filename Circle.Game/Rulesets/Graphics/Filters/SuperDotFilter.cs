using Circle.Game.Rulesets.Graphics.Shaders;
using osu.Framework.Graphics.Rendering;
using osuTK;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class SuperDotFilter : CameraFilter, IHasResolution
    {
        public Vector2 Resolution { get; set; }

        private IUniformBuffer<ResolutionParameters>? parameters;

        public SuperDotFilter()
            : base("superdot")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            base.UpdateUniforms(renderer);

            parameters ??= renderer.CreateUniformBuffer<ResolutionParameters>();

            parameters.Data = parameters.Data with { Resolution = Resolution };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }
    }
}
