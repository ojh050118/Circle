using System.Runtime.InteropServices;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;
using osuTK;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class ScreenScrollFilter : CameraFilter, IHasTime
    {
        public float StartTime { get; set; }

        public float Time { get; set; }

        public Vector2 Start { get; set; }

        public Vector2 Speed { get; set; }

        private IUniformBuffer<ScreenScrollParameters>? parameters;

        public ScreenScrollFilter()
            : base("screenscroll")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            parameters ??= renderer.CreateUniformBuffer<ScreenScrollParameters>();

            parameters.Data = new ScreenScrollParameters
            {
                StartTime = StartTime,
                Time = Time,
                StartX = Start.X,
                StartY = Start.Y,
                SpeedX = Speed.X,
                SpeedY = Speed.Y,
            };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private record struct ScreenScrollParameters
        {
            public UniformFloat StartTime;
            public UniformFloat Time;
            public UniformFloat StartX;
            public UniformFloat StartY;
            public UniformFloat SpeedX;
            public UniformFloat SpeedY;
        }
    }
}
