using System.Runtime.InteropServices;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class SepiaFilter : CameraFilter, IHasIntensity
    {
        public float Intensity { get; set; }

        private IUniformBuffer<SepiaParameters>? parameters;

        public SepiaFilter()
            : base("sepia")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            parameters ??= renderer.CreateUniformBuffer<SepiaParameters>();
            parameters.Data = parameters.Data with { Intensity = Intensity };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private record struct SepiaParameters
        {
            public UniformFloat Intensity;
            public UniformPadding12 Padding;
        }
    }
}
