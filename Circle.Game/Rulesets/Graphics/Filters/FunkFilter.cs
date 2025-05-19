using System.Runtime.InteropServices;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class FunkFilter : CameraFilter, IHasTime
    {
        public float Time { get; set; }

        private IUniformBuffer<FunkParameters>? parameters;

        public FunkFilter()
            : base("funk")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            parameters ??= renderer.CreateUniformBuffer<FunkParameters>();

            parameters.Data = new FunkParameters { Time = Time };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private record struct FunkParameters
        {
            public UniformFloat Time;
            public UniformPadding12 Padding;
        }
    }
}
