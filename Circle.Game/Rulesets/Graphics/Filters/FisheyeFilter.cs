using System.Runtime.InteropServices;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class FisheyeFilter : CameraFilter, IHasIntensity
    {
        public float Intensity { get; set; }

        private IUniformBuffer<FisheyeParameters>? parameters;

        public FisheyeFilter()
            : base("fisheye")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            parameters ??= renderer.CreateUniformBuffer<FisheyeParameters>();
            parameters.Data = parameters.Data with { Intensity = Intensity };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private record struct FisheyeParameters
        {
            public UniformFloat Intensity;
            public UniformPadding12 Padding;
        }
    }
}
