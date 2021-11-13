using System.Linq;
using System.Threading.Tasks;
using Circle.Game.IO;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Graphics.UserInterface
{
    public class Background : CompositeDrawable
    {
        private BufferedContainer bufferedContainer;
        private BufferedContainer newBufferedContainer;
        private Box dim;

        public readonly Sprite Sprite;

        private readonly string textureName;

        public Bindable<Vector2> BlurSigma { get; set; } = new Bindable<Vector2>();

        private readonly TextureSource source;

        private LargeTextureStore largeTexture;
        private MonitoredLargeTextureStore monitoredLargeTexture;

        public Background(TextureSource source = TextureSource.Internal, string textureName = @"")
        {
            this.source = source;
            this.textureName = textureName;
            RelativeSizeAxes = Axes.Both;
            AddInternal(bufferedContainer = new BufferedContainer(cachedFrameBuffer: true)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                RedrawOnScale = false,
                Masking = true,
                Child = Sprite = new Sprite
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    FillMode = FillMode.Fill,
                },
            });
        }

        [BackgroundDependencyLoader]
        private void load(LargeTextureStore textures, MonitoredLargeTextureStore monitoredTextures)
        {
            largeTexture = textures;
            monitoredLargeTexture = monitoredTextures;

            if (!string.IsNullOrEmpty(textureName))
                Sprite.Texture = source == TextureSource.Internal ? textures.Get(textureName) : monitoredTextures.Get(textureName);

            BlurSigma.ValueChanged += v => BlurTo(v.NewValue);
            BlurSigma.TriggerChange();
        }

        public void BlurTo(Vector2 newBlurSigma, double duration = 0, Easing easing = Easing.None)
        {
            bufferedContainer.BlurTo(newBlurSigma, duration, easing);
            newBufferedContainer?.BlurTo(newBlurSigma, duration, easing);
        }

        public void ColorTo(Color4 color4, double duration = 0, Easing easing = Easing.None)
        {
            bufferedContainer.FadeColour(color4, duration, easing);
        }

        public void DimTo(float newAlpha, double duration = 0, Easing easing = Easing.None)
        {
            if (dim == null)
            {
                AddInternal(dim = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black,
                    Alpha = 0
                });
            }

            dim?.FadeTo(newAlpha, duration, easing);
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

        public void FadeTextureTo(TextureSource source, string textureName, double duration, Easing easing = Easing.None)
        {
            if (Sprite is null)
                return;

            BufferedContainer newContainer;

            Schedule(() => InternalChildren.First().FadeOut(duration, easing));
            Scheduler.AddDelayed(() => RemoveInternal(InternalChildren.First()), duration);

            if (source == TextureSource.Internal)
            {
                LoadComponentAsync(newContainer = new BufferedContainer(cachedFrameBuffer: true)
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Alpha = 0,
                    RelativeSizeAxes = Axes.Both,
                    RedrawOnScale = false,
                    Masking = true,
                    Child = loadTexture(source, textureName).GetAwaiter().GetResult(),
                }, _ =>
                {
                    AddInternal(newContainer);
                    newBufferedContainer = newContainer;
                    newContainer.BlurTo(BlurSigma.Value);
                    newContainer.FadeIn(duration, easing);
                    Scheduler.AddDelayed(() => bufferedContainer = newContainer, duration);
                });
            }
            else
            {
                LoadComponentAsync(newContainer = new BufferedContainer(cachedFrameBuffer: true)
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Alpha = 0,
                    RelativeSizeAxes = Axes.Both,
                    RedrawOnScale = false,
                    Masking = true,
                    Child = loadTexture(source, textureName).GetAwaiter().GetResult(),
                }, _ =>
                {
                    AddInternal(newContainer);
                    newBufferedContainer = newContainer;
                    newContainer.BlurTo(BlurSigma.Value);
                    newContainer.FadeIn(duration, easing);
                    Scheduler.AddDelayed(() => bufferedContainer = newContainer, duration);
                });
            }
        }
    }

    public enum TextureSource
    {
        Internal,
        External
    }
}
