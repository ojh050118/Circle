using Circle.Game.Rulesets.Graphics.Shaders;
using osu.Framework.Graphics.Rendering;
using osuTK;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class EightiesTvFilter : CameraFilter, IHasTime, IHasResolution
    {
        public float Time { get; set; }
        public Vector2 Resolution { get; set; }

        private IUniformBuffer<TimeResolutionParameters>? parameters;

        public EightiesTvFilter()
            : base("eightiestv")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            base.UpdateUniforms(renderer);

            parameters ??= renderer.CreateUniformBuffer<TimeResolutionParameters>();

            parameters.Data = new TimeResolutionParameters { Time = Time, Resolution = Resolution };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }
    }
}
