using System.Runtime.InteropServices;
using osu.Framework.Graphics.Shaders.Types;

namespace Circle.Game.Rulesets.Graphics.Shaders
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public record struct TimeParameters
    {
        public UniformFloat Time;
        private readonly UniformPadding12 padding;
    }
}
