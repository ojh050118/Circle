using System.Collections.Generic;
using System.Linq;
using Circle.Game.Rulesets.Graphics.Filters;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Textures;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Rulesets.Graphics
{
    public partial class PostProcessingContainer : Container, IBufferedDrawable
    {
        private readonly BufferedDrawNodeSharedData sharedData = new BufferedDrawNodeSharedData(2);

        public readonly List<CameraFilter> Filters = new List<CameraFilter>();
        public readonly List<CameraFilter> FiltersInUse = new List<CameraFilter>();

        #region Filter Fields

        public readonly AberrationFilter Aberration;
        public readonly ArcadeFilter Arcade;
        public readonly BlizzardFilter Blizzard;
        public readonly BloomFilter Bloom;
        public readonly BlurFilter Blur;
        public readonly BlurFocusFilter BlurFocus;
        public readonly CompressionFilter Compression;
        public readonly ContrastFilter Contrast;
        public readonly DrawingFilter Drawing;
        public readonly EdgeBlackLineFilter EdgeBlackLine;
        public readonly EightiesTvFilter EightiesTv;
        public new readonly EmptyFilter Empty;
        public readonly FiftiesTvFilter FiftiesTv;
        public readonly FisheyeFilter Fisheye;
        public readonly FunkFilter Funk;
        public readonly GaussianBlurFilter GaussianBlur;
        public readonly GlitchFilter Glitch;
        public readonly GrainFilter Grain;
        public readonly GrayscaleFilter Grayscale;
        public readonly HandheldFilter Handheld;
        public readonly HexagonBlackFilter HexagonBlack;
        public readonly InvertFilter Invert;
        public readonly LedFilter Led;
        public readonly LightWaterFilter LightWater;
        public readonly NeonFilter Neon;
        public readonly NightVisionFilter NightVision;
        public readonly OilPaintFilter OilPaint;
        public readonly PixelateFilter Pixelate;
        public readonly PixelSnowFilter PixelSnow;
        public readonly PosterizeFilter Posterize;
        public readonly RainFilter Rain;
        public readonly ScreenScrollFilter ScreenScroll;
        public readonly ScreenTilingFilter ScreenTiling;
        public readonly SepiaFilter Sepia;
        public readonly SharpenFilter Sharpen;
        public readonly StaticFilter Static;
        public readonly SuperDotFilter SuperDot;
        public readonly TunnelFilter Tunnel;
        public readonly VhsFilter Vhs;
        public readonly WaterDropFilter WaterDrop;
        public readonly WavesFilter Waves;
        public readonly Weird3DFilter Weird3D;

        #endregion

        #region Filter Intensity

        public float AberrationIntensity
        {
            get => Aberration.Intensity;
            set => Aberration.Intensity = value;
        }

        public float BlizzardIntensity
        {
            get => Blizzard.Intensity;
            set => Blizzard.Intensity = value;
        }

        public float BloomIntensity
        {
            get => Bloom.Intensity;
            set => Bloom.Intensity = value;
        }

        public float BlurIntensity
        {
            get => Blur.Intensity;
            set => Blur.Intensity = value;
        }

        public float BlurFocusIntensity
        {
            get => BlurFocus.Intensity;
            set => BlurFocus.Intensity = value;
        }

        public float CompressionIntensity
        {
            get => Compression.Intensity;
            set => Compression.Intensity = value;
        }

        public float ContrastIntensity
        {
            get => Contrast.Intensity;
            set => Contrast.Intensity = value;
        }

        public float DrawingIntensity
        {
            get => Drawing.Intensity;
            set => Drawing.Intensity = value;
        }

        public float FisheyeIntensity
        {
            get => Fisheye.Intensity;
            set => Fisheye.Intensity = value;
        }

        public float GaussianBlurIntensity
        {
            get => GaussianBlur.Intensity;
            set => GaussianBlur.Intensity = value;
        }

        public float GrainIntensity
        {
            get => Grain.Intensity;
            set => Grain.Intensity = value;
        }

        public float GrayscaleIntensity
        {
            get => Grayscale.Intensity;
            set => Grayscale.Intensity = value;
        }

        public float HexagonBlackIntensity
        {
            get => HexagonBlack.Intensity;
            set => HexagonBlack.Intensity = value;
        }

        public float LedIntensity
        {
            get => Led.Intensity;
            set => Led.Intensity = value;
        }

        public float LightWaterIntensity
        {
            get => LightWater.Intensity;
            set => LightWater.Intensity = value;
        }

        public float OilPaintIntensity
        {
            get => OilPaint.Intensity;
            set => OilPaint.Intensity = value;
        }

        public float PixelateIntensity
        {
            get => Pixelate.Intensity;
            set => Pixelate.Intensity = value;
        }

        public float PixelSnowIntensity
        {
            get => PixelSnow.Intensity;
            set => PixelSnow.Intensity = value;
        }

        public float PosterizeIntensity
        {
            get => Posterize.Intensity;
            set => Posterize.Intensity = value;
        }

        public float RainIntensity
        {
            get => Rain.Intensity;
            set => Rain.Intensity = value;
        }

        public float SepiaIntensity
        {
            get => Sepia.Intensity;
            set => Sepia.Intensity = value;
        }

        public float SharpenIntensity
        {
            get => Sharpen.Intensity;
            set => Sharpen.Intensity = value;
        }

        public float StaticIntensity
        {
            get => Static.Intensity;
            set => Static.Intensity = value;
        }

        public float VhsIntensity
        {
            get => Vhs.Intensity;
            set => Vhs.Intensity = value;
        }

        public float WaterDropIntensity
        {
            get => WaterDrop.Intensity;
            set => WaterDrop.Intensity = value;
        }

        public float WavesIntensity
        {
            get => Waves.Intensity;
            set => Waves.Intensity = value;
        }

        #endregion

        #region Filter Enabled

        public float AberrationEnabled
        {
            get => Aberration.Enabled ? 1 : 0;
            set => Aberration.Enabled = value == 1;
        }

        public float ArcadeEnabled
        {
            get => Arcade.Enabled ? 1 : 0;
            set => Arcade.Enabled = value == 1;
        }

        public float BlizzardEnabled
        {
            get => Blizzard.Enabled ? 1 : 0;
            set => Blizzard.Enabled = value == 1;
        }

        public float BloomEnabled
        {
            get => Bloom.Enabled ? 1 : 0;
            set => Bloom.Enabled = value == 1;
        }

        public float BlurEnabled
        {
            get => Blur.Enabled ? 1 : 0;
            set => Blur.Enabled = value == 1;
        }

        public float BlurFocusEnabled
        {
            get => BlurFocus.Enabled ? 1 : 0;
            set => BlurFocus.Enabled = value == 1;
        }

        public float CompressionEnabled
        {
            get => Compression.Enabled ? 1 : 0;
            set => Compression.Enabled = value == 1;
        }

        public float ContrastEnabled
        {
            get => Contrast.Enabled ? 1 : 0;
            set => Contrast.Enabled = value == 1;
        }

        public float DrawingEnabled
        {
            get => Drawing.Enabled ? 1 : 0;
            set => Drawing.Enabled = value == 1;
        }

        public float EdgeBlackLineEnabled
        {
            get => EdgeBlackLine.Enabled ? 1 : 0;
            set => EdgeBlackLine.Enabled = value == 1;
        }

        public float EightiesTvEnabled
        {
            get => EightiesTv.Enabled ? 1 : 0;
            set => EightiesTv.Enabled = value == 1;
        }

        public float FiftiesTvEnabled
        {
            get => FiftiesTv.Enabled ? 1 : 0;
            set => FiftiesTv.Enabled = value == 1;
        }

        public float FisheyeEnabled
        {
            get => Fisheye.Enabled ? 1 : 0;
            set => Fisheye.Enabled = value == 1;
        }

        public float FunkEnabled
        {
            get => Funk.Enabled ? 1 : 0;
            set => Funk.Enabled = value == 1;
        }

        public float GaussianBlurEnabled
        {
            get => GaussianBlur.Enabled ? 1 : 0;
            set => GaussianBlur.Enabled = value == 1;
        }

        public float GlitchEnabled
        {
            get => Glitch.Enabled ? 1 : 0;
            set => Glitch.Enabled = value == 1;
        }

        public float GrainEnabled
        {
            get => Grain.Enabled ? 1 : 0;
            set => Grain.Enabled = value == 1;
        }

        public float GrayscaleEnabled
        {
            get => Grayscale.Enabled ? 1 : 0;
            set => Grayscale.Enabled = value == 1;
        }

        public float HandheldEnabled
        {
            get => Handheld.Enabled ? 1 : 0;
            set => Handheld.Enabled = value == 1;
        }

        public float HexagonBlackEnabled
        {
            get => HexagonBlack.Enabled ? 1 : 0;
            set => HexagonBlack.Enabled = value == 1;
        }

        public float InvertEnabled
        {
            get => Invert.Enabled ? 1 : 0;
            set => Invert.Enabled = value == 1;
        }

        public float LedEnabled
        {
            get => Led.Enabled ? 1 : 0;
            set => Led.Enabled = value == 1;
        }

        public float LightWaterEnabled
        {
            get => LightWater.Enabled ? 1 : 0;
            set => LightWater.Enabled = value == 1;
        }

        public float NeonEnabled
        {
            get => Neon.Enabled ? 1 : 0;
            set => Neon.Enabled = value == 1;
        }

        public float NightVisionEnabled
        {
            get => NightVision.Enabled ? 1 : 0;
            set => NightVision.Enabled = value == 1;
        }

        public float OilPaintEnabled
        {
            get => OilPaint.Enabled ? 1 : 0;
            set => OilPaint.Enabled = value == 1;
        }

        public float PixelateEnabled
        {
            get => Pixelate.Enabled ? 1 : 0;
            set => Pixelate.Enabled = value == 1;
        }

        public float PixelSnowEnabled
        {
            get => PixelSnow.Enabled ? 1 : 0;
            set => PixelSnow.Enabled = value == 1;
        }

        public float PosterizeEnabled
        {
            get => Posterize.Enabled ? 1 : 0;
            set => Posterize.Enabled = value == 1;
        }

        public float RainEnabled
        {
            get => Rain.Enabled ? 1 : 0;
            set => Rain.Enabled = value == 1;
        }

        public float ScreenScrollEnabled
        {
            get => ScreenScroll.Enabled ? 1 : 0;
            set => ScreenScroll.Enabled = value == 1;
        }

        public float ScreenTilingEnabled
        {
            get => ScreenTiling.Enabled ? 1 : 0;
            set => ScreenTiling.Enabled = value == 1;
        }

        public float SepiaEnabled
        {
            get => Sepia.Enabled ? 1 : 0;
            set => Sepia.Enabled = value == 1;
        }

        public float SharpenEnabled
        {
            get => Sharpen.Enabled ? 1 : 0;
            set => Sharpen.Enabled = value == 1;
        }

        public float StaticEnabled
        {
            get => Static.Enabled ? 1 : 0;
            set => Static.Enabled = value == 1;
        }

        public float SuperDotEnabled
        {
            get => SuperDot.Enabled ? 1 : 0;
            set => SuperDot.Enabled = value == 1;
        }

        public float TunnelEnabled
        {
            get => Tunnel.Enabled ? 1 : 0;
            set => Tunnel.Enabled = value == 1;
        }

        public float VhsEnabled
        {
            get => Vhs.Enabled ? 1 : 0;
            set => Vhs.Enabled = value == 1;
        }

        public float WaterDropEnabled
        {
            get => WaterDrop.Enabled ? 1 : 0;
            set => WaterDrop.Enabled = value == 1;
        }

        public float WavesEnabled
        {
            get => Waves.Enabled ? 1 : 0;
            set => Waves.Enabled = value == 1;
        }

        public float Weird3DEnabled
        {
            get => Weird3D.Enabled ? 1 : 0;
            set => Weird3D.Enabled = value == 1;
        }

        #endregion

        public IShader TextureShader { get; private set; } = null!;

        public Color4 BackgroundColour => new Color4(0, 0, 0, 0);

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
                Blur ??= new BlurFilter(),
                BlurFocus ??= new BlurFocusFilter(),
                Compression ??= new CompressionFilter(),
                Contrast ??= new ContrastFilter(),
                Drawing ??= new DrawingFilter(),
                EdgeBlackLine ??= new EdgeBlackLineFilter(),
                EightiesTv ??= new EightiesTvFilter(),
                Empty ??= new EmptyFilter(),
                FiftiesTv ??= new FiftiesTvFilter(),
                Fisheye ??= new FisheyeFilter(),
                Funk ??= new FunkFilter(),
                GaussianBlur ??= new GaussianBlurFilter(),
                Glitch ??= new GlitchFilter(),
                Grain ??= new GrainFilter(),
                Grayscale ??= new GrayscaleFilter(),
                Handheld ??= new HandheldFilter(),
                HexagonBlack ??= new HexagonBlackFilter(),
                Invert ??= new InvertFilter(),
                Led ??= new LedFilter(),
                LightWater ??= new LightWaterFilter(),
                Neon ??= new NeonFilter(),
                NightVision ??= new NightVisionFilter(),
                OilPaint ??= new OilPaintFilter(),
                Pixelate ??= new PixelateFilter(),
                PixelSnow ??= new PixelSnowFilter(),
                Posterize ??= new PosterizeFilter(),
                Rain ??= new RainFilter(),
                ScreenScroll ??= new ScreenScrollFilter(),
                ScreenTiling ??= new ScreenTilingFilter(),
                Sepia ??= new SepiaFilter(),
                Sharpen ??= new SharpenFilter(),
                Static ??= new StaticFilter(),
                SuperDot ??= new SuperDotFilter(),
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

                renderer.PushScissorState(false);
                drawEffectBuffers(renderer);
                renderer.PopScissorState();
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

                        filter.UpdateUniforms(renderer);

                        filter.Shader.Bind();
                        renderer.DrawFrameBuffer(currentEffectBuffer, new RectangleF(0, 0, currentEffectBuffer.Texture.Width, currentEffectBuffer.Texture.Height),
                            ColourInfo.SingleColour(Color4.White));
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
