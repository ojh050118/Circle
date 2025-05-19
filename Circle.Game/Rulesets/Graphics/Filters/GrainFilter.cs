using System.Runtime.InteropServices;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class GrainFilter : CameraFilter, IHasIntensity, IHasTime
    {
        public float Intensity { get; set; }

        public float Time { get; set; }

        private IUniformBuffer<GrainParameters>? parameters;

        public GrainFilter()
            : base("grain")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            parameters ??= renderer.CreateUniformBuffer<GrainParameters>();
            parameters.Data = new GrainParameters { Intensity = Intensity, Time = Time };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private record struct GrainParameters
        {
            public UniformFloat Intensity;
            public UniformFloat Time;
            public UniformPadding8 Padding;
        }
    }
}
