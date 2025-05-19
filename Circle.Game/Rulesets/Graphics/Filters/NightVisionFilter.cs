using System.Runtime.InteropServices;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class NightVisionFilter : CameraFilter, IHasTime
    {
        public float Time { get; set; }

        private IUniformBuffer<NightVisionParameters>? parameters;

        public NightVisionFilter()
            : base("nightvision")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            parameters ??= renderer.CreateUniformBuffer<NightVisionParameters>();

            parameters.Data = new NightVisionParameters { Time = Time };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private record struct NightVisionParameters
        {
            public UniformFloat Time;
            public UniformPadding12 Padding;
        }
    }
}
