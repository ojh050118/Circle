using System.Runtime.InteropServices;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;
using osuTK;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class ScreenTilingFilter : CameraFilter
    {
        public Vector2 Tiling { get; set; }

        private IUniformBuffer<ScreenTilingParameters>? parameters;

        public ScreenTilingFilter()
            : base("screentiling")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            parameters ??= renderer.CreateUniformBuffer<ScreenTilingParameters>();

            parameters.Data = new ScreenTilingParameters
            {
                TilingX = Tiling.X,
                TilingY = Tiling.Y,
            };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private record struct ScreenTilingParameters
        {
            public UniformFloat TilingX;
            public UniformFloat TilingY;
            public UniformPadding8 Padding;
        }
    }
}
