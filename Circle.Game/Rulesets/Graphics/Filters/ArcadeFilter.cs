using System.Runtime.InteropServices;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;
using osuTK;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class ArcadeFilter : CameraFilter, IHasTime, IHasResolution
    {
        public float Time { get; set; }
        public Vector2 Resolution { get; set; }

        private IUniformBuffer<ArcadeParameters>? parameters;

        public ArcadeFilter()
            : base("arcade")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            parameters ??= renderer.CreateUniformBuffer<ArcadeParameters>();

            parameters.Data = new ArcadeParameters { Time = Time, Resolution = Resolution };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private record struct ArcadeParameters
        {
            public UniformFloat Time;
            public UniformVector2 Resolution;
            public UniformPadding4 Padding;
        }
    }
}
