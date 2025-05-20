using System.Runtime.InteropServices;
using osu.Framework.Graphics.Shaders.Types;

namespace Circle.Game.Rulesets.Graphics.Shaders
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public record struct IntensityParameters
    {
        public UniformFloat Intensity;
        private readonly UniformPadding12 padding;
    }
}
