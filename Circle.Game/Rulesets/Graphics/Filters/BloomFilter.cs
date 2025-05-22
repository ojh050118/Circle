using System.Runtime.InteropServices;
using Circle.Game.Rulesets.Graphics.Shaders;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class BloomFilter : CameraFilter, IHasIntensity, IHasResolution
    {
        public float Intensity { get; set; }
        public float Threshold { get; set; }
        public Color4 Color { get; set; }
        public Vector2 Resolution { get; set; }

        private IUniformBuffer<BloomParameters>? parameters;

        public BloomFilter()
            : base("bloom")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            base.UpdateUniforms(renderer);

            parameters ??= renderer.CreateUniformBuffer<BloomParameters>();

            parameters.Data = new BloomParameters { Intensity = Intensity, Threshold = Threshold, R = Color.R, G = Color.G, B = Color.B, Resolution = Resolution };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private record struct BloomParameters
        {
            public UniformFloat Intensity;
            public UniformFloat Threshold;
            public UniformFloat R;
            public UniformFloat G;
            public UniformFloat B;
            public UniformPadding4 Padding;
            public UniformVector2 Resolution;
        }
    }
}
