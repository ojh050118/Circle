using System.Runtime.InteropServices;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class BlizzardFilter : CameraFilter, IHasIntensity, IHasTime
    {
        public float Intensity { get; set; }
        public float Time { get; set; }

        private IUniformBuffer<BlizzardParameters>? parameters;

        public BlizzardFilter()
            : base("blizzard", 1, "Blizzard")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            parameters ??= renderer.CreateUniformBuffer<BlizzardParameters>();

            parameters.Data = new BlizzardParameters { Intensity = Intensity, Time = Time };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private record struct BlizzardParameters
        {
            public UniformFloat Intensity;
            public UniformFloat Time;
            public UniformVector4 TextureRect;
        }
    }
}
