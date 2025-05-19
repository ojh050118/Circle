using System.Runtime.InteropServices;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;
using osuTK;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class EightiesTvFilter : CameraFilter, IHasTime, IHasResolution
    {
        public float Time { get; set; }
        public Vector2 Resolution { get; set; }

        private IUniformBuffer<EightiesTvParameters>? parameters;

        public EightiesTvFilter()
            : base("eightiestv")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            parameters ??= renderer.CreateUniformBuffer<EightiesTvParameters>();

            parameters.Data = new EightiesTvParameters { Time = Time, Resolution = Resolution };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private record struct EightiesTvParameters
        {
            public UniformFloat Time;
            public UniformVector2 Resolution;
            public UniformPadding4 Padding;
        }
    }
}
