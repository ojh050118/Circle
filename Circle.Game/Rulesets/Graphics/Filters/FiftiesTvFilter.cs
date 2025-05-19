using System.Runtime.InteropServices;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;
using osuTK;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class FiftiesTvFilter : CameraFilter, IHasTime, IHasResolution
    {
        public float Time { get; set; }
        public Vector2 Resolution { get; set; }

        private IUniformBuffer<FiftiesTvParameters>? parameters;

        public FiftiesTvFilter()
            : base("fiftiestv")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            parameters ??= renderer.CreateUniformBuffer<FiftiesTvParameters>();

            parameters.Data = new FiftiesTvParameters { Time = Time, Resolution = Resolution };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private record struct FiftiesTvParameters
        {
            public UniformFloat Time;
            public UniformVector2 Resolution;
            public UniformPadding4 Padding;
        }
    }
}
