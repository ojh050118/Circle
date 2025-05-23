using Circle.Game.Rulesets.Graphics.Shaders;
using osu.Framework.Graphics.Rendering;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public class PixelSnowFilter : CameraFilter, IHasIntensity, IHasTime
    {
        public override bool Enabled => base.Enabled && Intensity > 0;

        public float Intensity { get; set; }

        public float IntensityForShader => (float)(Intensity / 100f * 0.10000000149011612 + 0.89999997615814209);

        public float Time { get; set; }

        private IUniformBuffer<IntensityTimeParameters>? parameters;

        public PixelSnowFilter()
            : base("pixelsnow")
        {
        }

        public override void UpdateUniforms(IRenderer renderer)
        {
            base.UpdateUniforms(renderer);

            parameters ??= renderer.CreateUniformBuffer<IntensityTimeParameters>();

            parameters.Data = parameters.Data with { Intensity = IntensityForShader, Time = Time };

            Shader.BindUniformBlock(@"m_FilterParameters", parameters);
        }
    }
}
