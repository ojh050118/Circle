using System.Runtime.InteropServices;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class AberrationFilter : CameraFilter, IHasIntensity
    {
        public float Intensity { get; set; }

        private IUniformBuffer<AberrationParameters>? parameters;

        public AberrationFilter()
            : base("aberration")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            parameters ??= renderer.CreateUniformBuffer<AberrationParameters>();

            parameters.Data = new AberrationParameters { Intensity = Intensity };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private record struct AberrationParameters
        {
            public UniformFloat Intensity;
            public UniformPadding12 Padding;
        }
    }
}
