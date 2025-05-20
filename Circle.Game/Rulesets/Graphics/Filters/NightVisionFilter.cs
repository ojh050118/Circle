using Circle.Game.Rulesets.Graphics.Shaders;
using osu.Framework.Graphics.Rendering;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class NightVisionFilter : CameraFilter, IHasTime
    {
        public float Time { get; set; }

        private IUniformBuffer<TimeParameters>? parameters;

        public NightVisionFilter()
            : base("nightvision")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            parameters ??= renderer.CreateUniformBuffer<TimeParameters>();

            parameters.Data = parameters.Data with { Time = Time };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }
    }
}
