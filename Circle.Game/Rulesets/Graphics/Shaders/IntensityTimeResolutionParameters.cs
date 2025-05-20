using System.Runtime.InteropServices;
using osu.Framework.Graphics.Shaders.Types;

namespace Circle.Game.Rulesets.Graphics.Shaders
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public record struct IntensityTimeResolutionParameters
    {
        public UniformFloat Intensity;
        public UniformFloat Time;
        private UniformPadding8 padding;
        public UniformVector2 Resolution;
        private UniformPadding8 padding2;
    }
}
