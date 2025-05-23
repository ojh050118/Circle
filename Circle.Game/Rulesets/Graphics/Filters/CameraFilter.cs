using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Textures;
using osuTK;

namespace Circle.Game.Rulesets.Graphics.Filters
{
    public abstract class CameraFilter
    {
        public virtual bool Enabled { get; set; }

        public Texture[]? Textures { get; set; }
        public Vector4[]? TextureRects { get; set; }

        public int TextureCount { get; }

        public IShader Shader { get; set; } = null!;

        public readonly string ShaderName;

        public readonly string? TextureName;

        protected CameraFilter(string shaderName, int textureCount = 0, string? textureName = null)
        {
            ShaderName = shaderName;

            if (textureCount > 0)
            {
                TextureCount = textureCount;
                Textures = new Texture[TextureCount];
                TextureRects = new Vector4[TextureCount];
            }

            if (textureName != null)
                TextureName = textureName;
        }

        public virtual void UpdateUniforms(IRenderer renderer)
        {
            for (int i = 0; i < TextureCount; i++)
            {
                Textures![i].Bind(i + 1);

                var textureRect = Textures![i].GetTextureRect();

                TextureRects![i] = new Vector4(textureRect.Left, textureRect.Top, textureRect.Right, textureRect.Bottom);
            }
        }
    }
}
