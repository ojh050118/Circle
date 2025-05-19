using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Textures;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public abstract class CameraFilter
    {
        public bool Enabled { get; set; }

        public Texture[]? Textures { get; set; }
        public IShader Shader { get; set; } = null!;

        public readonly string ShaderName;

        protected CameraFilter(string shaderName)
        {
            ShaderName = shaderName;
        }

        public abstract void UpdateUniforms(IRenderer renderer);
    }
}
