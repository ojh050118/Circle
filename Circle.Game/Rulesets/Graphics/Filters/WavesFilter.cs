using System.Runtime.InteropServices;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class WavesFilter : CameraFilter, IHasIntensity, IHasTime
    {
        public float Intensity { get; set; }

        public float Time { get; set; }

        private IUniformBuffer<WavesParameters>? parameters;

        public WavesFilter()
            : base("waves")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            parameters ??= renderer.CreateUniformBuffer<WavesParameters>();
            parameters.Data = new WavesParameters { Intensity = Intensity, Time = Time };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private record struct WavesParameters
        {
            public UniformFloat Intensity;
            public UniformFloat Time;
            public UniformPadding8 Padding;
        }
    }
}
