using System.Runtime.InteropServices;
using Circle.Game.Rulesets.Graphics.Shaders;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class VhsFilter : CameraFilter, IHasIntensity, IHasTime
    {
        public float Intensity { get; set; }

        public float IntensityForShader => Intensity / 100f;

        public float Time { get; set; }

        private IUniformBuffer<VhsParameters>? parameters;

        public VhsFilter()
            : base("vhs", 2, "VHS")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            base.UpdateUniforms(renderer);

            parameters ??= renderer.CreateUniformBuffer<VhsParameters>();

            parameters.Data = parameters.Data with { Intensity = IntensityForShader, Time = Time, TextureRect1 = TextureRects![0], TextureRect2 = TextureRects![1] };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private record struct VhsParameters
        {
            public UniformFloat Intensity;
            public UniformFloat Time;
            public UniformPadding8 Padding;
            public UniformVector4 TextureRect1;
            public UniformVector4 TextureRect2;
        }
    }
}
