using Circle.Game.Rulesets.Graphics.Shaders;
using osu.Framework.Graphics.Rendering;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class FunkFilter : CameraFilter, IHasTime
    {
        public float Time { get; set; }

        private IUniformBuffer<TimeParameters>? parameters;

        public FunkFilter()
            : base("funk")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            parameters ??= renderer.CreateUniformBuffer<TimeParameters>();

            parameters.Data = new TimeParameters { Time = Time };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }
    }
}
