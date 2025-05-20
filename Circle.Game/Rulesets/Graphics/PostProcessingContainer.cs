using System.Collections.Generic;
using System.Linq;
using Circle.Game.Rulesets.Graphics.Filters;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Textures;
using osuTK;
using osuTK.Graphics;
using osuTK.Graphics.OpenGL;

namespace Circle.Game.Rulesets.Graphics
{
    public partial class PostProcessingContainer : Container, IBufferedDrawable
    {
        private readonly BufferedDrawNodeSharedData sharedData = new BufferedDrawNodeSharedData(2);

        public readonly List<CameraFilter> Filters = new List<CameraFilter>();
        public readonly List<CameraFilter> FiltersInUse = new List<CameraFilter>();

        public readonly AberrationFilter Aberration;
        public readonly ArcadeFilter Arcade;
        public readonly BlizzardFilter Blizzard;
        public readonly BloomFilter Bloom;
        public readonly CompressionFilter Compression;
        public readonly DrawingFilter Drawing;
        public readonly EightiesTvFilter EightiesTv;
        public new readonly EmptyFilter Empty;
        public readonly FiftiesTvFilter FiftiesTv;
        public readonly FisheyeFilter Fisheye;
        public readonly FunkFilter Funk;
        public readonly GlitchFilter Glitch;
        public readonly GrainFilter Grain;
        public readonly GrayscaleFilter Grayscale;
        public readonly HandheldFilter Handheld;
        public readonly InvertFilter Invert;
        public readonly LedFilter Led;
        public readonly NeonFilter Neon;
        public readonly NightVisionFilter NightVision;
        public readonly PixelateFilter Pixelate;
        public readonly PixelSnowFilter PixelSnow;
        public readonly RainFilter Rain;
        public readonly ScreenScrollFilter ScreenScroll;
        public readonly ScreenTilingFilter ScreenTiling;
        public readonly SepiaFilter Sepia;
        public readonly StaticFilter Static;
        public readonly TunnelFilter Tunnel;
        public readonly VhsFilter Vhs;
        public readonly WaterDropFilter WaterDrop;
        public readonly WavesFilter Waves;
        public readonly Weird3DFilter Weird3D;

        public IShader TextureShader { get; private set; } = null!;

        public Color4 BackgroundColour => Color4.Black;

        public DrawColourInfo? FrameBufferDrawColour => DrawColourInfo;

        public Vector2 FrameBufferScale => Vector2.One;

        protected override bool RequiresChildrenUpdate => true;

        protected override DrawNode CreateDrawNode() => new PostProcessingContainerDrawNode(this, sharedData);

        public PostProcessingContainer()
        {
            Filters.AddRange(new CameraFilter[]
            {
                Aberration ??= new AberrationFilter(),
                Arcade ??= new ArcadeFilter(),
                Blizzard ??= new BlizzardFilter(),
                Bloom ??= new BloomFilter(),
                Compression ??= new CompressionFilter(),
                Drawing ??= new DrawingFilter(),
                EightiesTv ??= new EightiesTvFilter(),
                Empty ??= new EmptyFilter { Enabled = true },
                FiftiesTv ??= new FiftiesTvFilter(),
                Fisheye ??= new FisheyeFilter(),
                Funk ??= new FunkFilter(),
                Glitch ??= new GlitchFilter(),
                Grain ??= new GrainFilter(),
                Grayscale ??= new GrayscaleFilter(),
                Handheld ??= new HandheldFilter(),
                Invert ??= new InvertFilter(),
                Led ??= new LedFilter(),
                Neon ??= new NeonFilter(),
                NightVision ??= new NightVisionFilter(),
                Pixelate ??= new PixelateFilter(),
                PixelSnow ??= new PixelSnowFilter(),
                Rain ??= new RainFilter(),
                ScreenScroll ??= new ScreenScrollFilter(),
                ScreenTiling ??= new ScreenTilingFilter(),
                Sepia ??= new SepiaFilter(),
                Static ??= new StaticFilter(),
                Tunnel ??= new TunnelFilter(),
                Vhs ??= new VhsFilter(),
                WaterDrop ??= new WaterDropFilter(),
                Waves ??= new WavesFilter(),
                Weird3D ??= new Weird3DFilter()
            });
        }

        [BackgroundDependencyLoader]
        private void load(ShaderManager shaderManager, LargeTextureStore textures)
        {
            TextureShader = shaderManager.Load(VertexShaderDescriptor.TEXTURE_2, FragmentShaderDescriptor.TEXTURE);

            foreach (var filter in Filters)
            {
                filter.Shader = shaderManager.Load(VertexShaderDescriptor.TEXTURE_2, filter.ShaderName);

                if (filter.Textures == null)
                    continue;

                for (int i = 0; i < filter.TextureCount; i++)
                    filter.Textures[i] = textures.Get($"Shader/{filter.TextureName}-{i}");
            }
        }

        protected override void Update()
        {
            base.Update();

            updateUsingShaders();
            Invalidate(Invalidation.DrawNode);
        }

        private void updateUsingShaders()
        {
            FiltersInUse.Clear();

            bool flag = false;

            foreach (var filter in Filters)
            {
                if (filter.Enabled)
                {
                    flag = true;
                    FiltersInUse.Add(filter);
                }
            }

            if (flag)
                return;

            FiltersInUse.Add(Empty);
        }

        private class PostProcessingContainerDrawNode : BufferedDrawNode, ICompositeDrawNode
        {
            protected new PostProcessingContainer Source => (PostProcessingContainer)base.Source;

            private readonly List<CameraFilter> filters = new List<CameraFilter>();

            public PostProcessingContainerDrawNode(PostProcessingContainer source, BufferedDrawNodeSharedData sharedData)
                : base(source, new CompositeDrawableDrawNode(source), sharedData)
            {
            }

            public override void ApplyState()
            {
                base.ApplyState();

                filters.Clear();
                filters.AddRange(Source.FiltersInUse);
            }

            protected override void PopulateContents(IRenderer renderer)
            {
                base.PopulateContents(renderer);
                drawEffectBuffers(renderer);
            }

            protected override void DrawContents(IRenderer renderer)
            {
                if (filters.Any())
                {
                    renderer.DrawFrameBuffer(SharedData.CurrentEffectBuffer, DrawRectangle, DrawColourInfo.Colour);
                }
                else
                {
                    base.DrawContents(renderer);
                }
            }

            private void drawEffectBuffers(IRenderer renderer)
            {
                foreach (var filter in filters)
                {
                    if (!filter.Enabled)
                        continue;

                    var currentEffectBuffer = SharedData.CurrentEffectBuffer;
                    var nextEffectBuffer = SharedData.GetNextEffectBuffer();

                    renderer.SetBlend(BlendingParameters.None);

                    using (BindFrameBuffer(nextEffectBuffer))
                    {
                        if (filter is IHasTime time)
                            time.Time = (float)(Source.Time.Current / 1000);

                        if (filter is IHasResolution resolution)
                            resolution.Resolution = nextEffectBuffer.Size;

                        if (filter.TextureCount > 0)
                        {
                            for (int i = 0; i < filter.TextureCount; i++)
                            {
                                int unit = i + 1;
                                filter.Textures?[i].Bind((int)(TextureUnit.Texture0 + unit));
                                filter.Shader.GetUniform<int>($"s_Texture{unit}").UpdateValue(ref unit);

                                var textureRect = filter.Textures![i].GetTextureRect();
                                var vector4 = new Vector4(textureRect.Left, textureRect.Right, textureRect.Top, textureRect.Bottom);

                                filter.Shader.GetUniform<Vector4>($"s_TexRect{unit}").UpdateValue(ref vector4);
                            }
                        }

                        filter.UpdateUniforms(renderer);

                        filter.Shader.Bind();
                        renderer.DrawFrameBuffer(currentEffectBuffer, new RectangleF(0, 0, currentEffectBuffer.Texture.Width, currentEffectBuffer.Texture.Height), DrawColourInfo.Colour);
                        filter.Shader.Unbind();
                    }
                }
            }

            protected new CompositeDrawableDrawNode Child => (CompositeDrawableDrawNode)base.Child;

            public List<DrawNode>? Children
            {
                get => Child.Children;
                set => Child.Children = value;
            }

            public bool AddChildDrawNodes => RequiresRedraw;
        }
    }
}
