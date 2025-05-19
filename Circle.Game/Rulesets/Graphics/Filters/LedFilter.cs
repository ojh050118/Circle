using System.Runtime.InteropServices;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;
using osuTK;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class LedFilter : CameraFilter, IHasIntensity, IHasTime, IHasResolution
    {
        public float Intensity { get; set; }
        public float Time { get; set; }
        public Vector2 Resolution { get; set; }

        private IUniformBuffer<LedParameters>? parameters;

        public LedFilter()
            : base("led")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            parameters ??= renderer.CreateUniformBuffer<LedParameters>();

            parameters.Data = new LedParameters { Intensity = Intensity, Time = Time, Resolution = Resolution };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private record struct LedParameters
        {
            public UniformFloat Intensity;
            public UniformFloat Time;
            public UniformVector2 Resolution;
        }
    }
}
