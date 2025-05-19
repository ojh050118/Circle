using System.Runtime.InteropServices;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class GlitchFilter : CameraFilter, IHasTime
    {
        public float Time { get; set; }

        private IUniformBuffer<GlitchParameters>? parameters;

        public GlitchFilter()
            : base("glitch")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            parameters ??= renderer.CreateUniformBuffer<GlitchParameters>();

            parameters.Data = new GlitchParameters { Time = Time };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private record struct GlitchParameters
        {
            public UniformFloat Time;
            public UniformPadding12 Padding;
        }
    }
}
