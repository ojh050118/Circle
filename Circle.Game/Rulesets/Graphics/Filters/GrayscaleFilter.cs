using System.Runtime.InteropServices;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class GrayscaleFilter : CameraFilter, IHasIntensity
    {
        public float Intensity { get; set; }

        private IUniformBuffer<GrayscaleParameters>? parameters;

        public GrayscaleFilter()
            : base("grayscale")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            parameters ??= renderer.CreateUniformBuffer<GrayscaleParameters>();
            parameters.Data = parameters.Data with { Intensity = Intensity };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private record struct GrayscaleParameters
        {
            public UniformFloat Intensity;
            public UniformPadding12 Padding;
        }
    }
}
