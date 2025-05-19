using System.Runtime.InteropServices;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class CompressionFilter : CameraFilter, IHasIntensity, IHasTime
    {
        public float Intensity { get; set; }

        public float Time { get; set; }

        private IUniformBuffer<CompressionParameters>? parameters;

        public CompressionFilter()
            : base("compression")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            parameters ??= renderer.CreateUniformBuffer<CompressionParameters>();
            parameters.Data = new CompressionParameters { Intensity = Intensity, Time = Time };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private record struct CompressionParameters
        {
            public UniformFloat Intensity;
            public UniformFloat Time;
            public UniformPadding8 Padding;
        }
    }
}
