using System;
using Circle.Game.Rulesets.Graphics.Shaders;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Utils;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class WaterDropFilter : CameraFilter, IHasIntensity, IHasTime
    {
        public float Intensity { get; set; }

        public float IntensityForShader => Interpolation.ValueAt(Math.Clamp(Intensity / 100f, 0f, 1f), 64f, 8f, 0.0, 1.0);

        public float Time { get; set; }

        private IUniformBuffer<IntensityTimeTextureRectParameters>? parameters;

        public WaterDropFilter()
            : base("waterdrop", 1, "WaterDrop")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            base.UpdateUniforms(renderer);

            parameters ??= renderer.CreateUniformBuffer<IntensityTimeTextureRectParameters>();

            parameters.Data = parameters.Data with { Intensity = IntensityForShader, Time = Time, TextureRect = TextureRects![0] };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }
    }
}
