using System.Runtime.InteropServices;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class PixelSnowFilter : CameraFilter, IHasIntensity, IHasTime
    {
        public float Intensity { get; set; }
        public float Time { get; set; }

        private IUniformBuffer<PixelSnowParameters>? parameters;

        public PixelSnowFilter()
            : base("pixelsnow")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            parameters ??= renderer.CreateUniformBuffer<PixelSnowParameters>();

            parameters.Data = new PixelSnowParameters { Intensity = Intensity, Time = Time };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private record struct PixelSnowParameters
        {
            public UniformFloat Intensity;
            public UniformFloat Time;
            public UniformPadding8 Padding;
        }
    }
}
