using System.Linq;
using System.Threading.Tasks;
using Circle.Game.Configuration;
using Circle.Game.IO;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Graphics
{
    public class Background : CompositeDrawable
    {
        private BufferedContainer bufferedContainer;
        private BufferedContainer newBufferedContainer;

        public readonly Sprite Sprite;

        private readonly string textureName;

        public Vector2 BlurSigma { get; set; }

        private readonly TextureSource source;

        private LargeTextureStore largeTexture;
        private MonitoredLargeTextureStore monitoredLargeTexture;

        public Background(TextureSource source = TextureSource.Internal, string textureName = @"")
        {
            this.source = source;
            this.textureName = textureName;
            RelativeSizeAxes = Axes.Both;
            AddInternal(Sprite = new Sprite
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                FillMode = FillMode.Fill,
            });
        }

        [BackgroundDependencyLoader]
        private void load(LargeTextureStore textures, MonitoredLargeTextureStore monitoredTextures, CircleConfigManager config)
        {
            largeTexture = textures;
            monitoredLargeTexture = monitoredTextures;

            if (!string.IsNullOrEmpty(textureName))
                Sprite.Texture = source == TextureSource.Internal ? textures.Get(textureName) : monitoredTextures.Get(textureName);

            if (BlurSigma != Vector2.Zero)
                BlurTo(BlurSigma);
        }

        public void BlurTo(Vector2 newBlurSigma, double duration = 0, Easing easing = Easing.None)
        {
            if (bufferedContainer == null && newBlurSigma != Vector2.Zero)
            {
                RemoveInternal(Sprite);

                AddInternal(bufferedContainer = new BufferedContainer
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    CacheDrawnFrameBuffer = true,
                    RedrawOnScale = false,
                    Child = Sprite,
                    Masking = true,
                });
            }

            if (bufferedContainer != null)
            {
                bufferedContainer.BlurTo(newBlurSigma, duration, easing);
                newBufferedContainer?.BlurTo(newBlurSigma, duration, easing);
                BlurSigma = newBlurSigma;
            }
        }

        public void BlurTo(float newBlurSigma, double duration = 0, Easing easing = Easing.None)
        {
            if (bufferedContainer == null && new Vector2(newBlurSigma) != Vector2.Zero)
            {
                RemoveInternal(Sprite);

                AddInternal(bufferedContainer = new BufferedContainer
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    CacheDrawnFrameBuffer = true,
                    RedrawOnScale = false,
                    Child = Sprite,
                    Masking = true,
                });
            }

            if (bufferedContainer != null)
            {
                bufferedContainer.BlurTo(new Vector2(newBlurSigma), duration, easing);
                newBufferedContainer?.BlurTo(new Vector2(newBlurSigma), duration, easing);
                BlurSigma = new Vector2(newBlurSigma);
            }
        }

        public void ColorTo(Color4 color4, double duration, Easing easing = Easing.None)
        {
            bufferedContainer.FadeColour(color4, duration, easing);
        }

        private async Task<Sprite> loadTexture(TextureSource source, string textureName)
        {
            if (source == TextureSource.Internal)
            {
                return new Sprite
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    FillMode = FillMode.Fill,
                    Texture = await largeTexture.GetAsync(textureName)
                };
            }
            else
            {
                return new Sprite
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    FillMode = FillMode.Fill,
                    Texture = await monitoredLargeTexture.GetAsync(textureName)
                };
            }
        }

        public void TextureFadeTo(TextureSource source, string textureName, double duration, Easing easing = Easing.None)
        {
            if (Sprite is null)
                return;

            BufferedContainer newContainer;

            Schedule(() => InternalChildren.First().FadeOut(duration, easing));
            Scheduler.AddDelayed(() => RemoveInternal(InternalChildren.First()), duration);

            if (source == TextureSource.Internal)
            {
                Schedule(() => LoadComponentAsync(newContainer = new BufferedContainer
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Alpha = 0,
                    RelativeSizeAxes = Axes.Both,
                    CacheDrawnFrameBuffer = true,
                    RedrawOnScale = false,
                    Masking = true,
                    Child = loadTexture(source, textureName).GetAwaiter().GetResult(),
                }, _ =>
                {
                    AddInternal(newContainer);
                    newBufferedContainer = newContainer;
                    newContainer.BlurTo(BlurSigma);
                    newContainer.FadeIn(duration, easing);
                    Scheduler.AddDelayed(() => bufferedContainer = newContainer, duration);
                }));
            }
            else
            {
                Schedule(() => LoadComponentAsync(newContainer = new BufferedContainer
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Alpha = 0,
                    RelativeSizeAxes = Axes.Both,
                    CacheDrawnFrameBuffer = true,
                    RedrawOnScale = false,
                    Masking = true,
                    Child = loadTexture(source, textureName).GetAwaiter().GetResult(),
                }, _ =>
                {
                    AddInternal(newContainer);
                    newBufferedContainer = newContainer;
                    newContainer.BlurTo(BlurSigma);
                    newContainer.FadeIn(duration, easing);
                    Scheduler.AddDelayed(() => bufferedContainer = newContainer, duration);
                }));
            }
        }
    }

    public enum TextureSource
    {
        Internal,
        External
    }
}
