using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Textures;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public abstract class CameraFilter
    {
        public bool Enabled { get; set; }

        public Texture[]? Textures { get; set; }

        public int TextureCount { get; }

        public IShader Shader { get; set; } = null!;

        public readonly string ShaderName;

        protected CameraFilter(string shaderName, int textureCount = 0)
        {
            ShaderName = shaderName;

            if (textureCount > 0)
            {
                TextureCount = textureCount;
                Textures = new Texture[TextureCount];
            }
        }

        public abstract void UpdateUniforms(IRenderer renderer);
    }
}
