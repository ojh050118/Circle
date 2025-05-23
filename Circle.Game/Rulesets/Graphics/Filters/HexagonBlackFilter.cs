using System;
using Circle.Game.Rulesets.Graphics.Shaders;
using osu.Framework.Graphics.Rendering;
using osuTK;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class HexagonBlackFilter : CameraFilter, IHasIntensity, IHasResolution
    {
        public float Intensity { get; set; }

        public float IntensityForShader => Math.Max(0.2f, Intensity / 100f);

        public Vector2 Resolution { get; set; }

        private IUniformBuffer<IntensityResolutionParameters>? parameters;

        public HexagonBlackFilter()
            : base("hexagonblack")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            base.UpdateUniforms(renderer);

            parameters ??= renderer.CreateUniformBuffer<IntensityResolutionParameters>();

            parameters.Data = parameters.Data with { Intensity = IntensityForShader, Resolution = Resolution };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }
    }
}
