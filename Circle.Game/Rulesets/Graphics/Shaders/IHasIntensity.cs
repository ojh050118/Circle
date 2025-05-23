namespace Circle.Game.Rulesets.Graphics.Shaders
{
    public interface IHasIntensity
    {
        float Intensity { get; set; }

        protected float IntensityForShader { get; }
    }
}
