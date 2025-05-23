using System.Runtime.InteropServices;
using osu.Framework.Graphics.Shaders.Types;

namespace Circle.Game.Rulesets.Graphics.Shaders
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public record struct IntensityTimeResolutionParameters
    {
        public UniformFloat Intensity;
        public UniformFloat Time;
        public UniformVector2 Resolution;
    }
}
