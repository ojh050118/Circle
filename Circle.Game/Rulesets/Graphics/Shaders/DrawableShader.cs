using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Textures;

namespace Circle.Game.Rulesets.Graphics.Shaders
{
    public partial class DrawableShader : Drawable
    {
        private IShader shader = null!;
        private Texture texture = null!;
        private readonly string shaderName;

        public DrawableShader(string shaderName)
        {
            this.shaderName = shaderName;
        }

        protected override DrawNode CreateDrawNode() => CreateShaderDrawNode();

        [BackgroundDependencyLoader]
        private void load(ShaderManager shaderManager, IRenderer renderer)
        {
            shader = shaderManager.Load(VertexShaderDescriptor.TEXTURE_2, shaderName);
            texture = renderer.WhitePixel;
        }

        protected virtual ShaderDrawNode CreateShaderDrawNode() => new ShaderDrawNode(this);

        protected class ShaderDrawNode : DrawNode
        {
            private Quad screenSpaceQuad;

            private IShader? shader;

            protected new DrawableShader Source => (DrawableShader)base.Source;

            public ShaderDrawNode(DrawableShader source)
                : base(source)
            {
            }

            public override void ApplyState()
            {
                base.ApplyState();
                shader = Source.shader;
                screenSpaceQuad = Source.ScreenSpaceDrawQuad;
            }

            protected override void Draw(IRenderer renderer)
            {
                base.Draw(renderer);

                if (shader == null)
                    return;

                shader.Bind();

                UpdateUniforms(shader, renderer);

                renderer.DrawQuad(Source.texture, screenSpaceQuad, DrawColourInfo.Colour, new RectangleF(0, 0, 1, 1), textureCoords: screenSpaceQuad.AABBFloat);

                shader.Unbind();
            }

            protected virtual void UpdateUniforms(IShader shader, IRenderer renderer)
            {
            }
        }
    }
}
