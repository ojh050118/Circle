using Circle.Game.Beatmaps;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Graphics.UserInterface
{
    public class Background : Container
    {
        public Vector2 BlurSigma
        {
            get => blurSigma;
            set => BlurTo(value);
        }

        public float Dim
        {
            get => dim;
            set => DimTo(value);
        }

        public string TextureName { get; private set; }

        public TextureSource TextureSource { get; private set; }

        [Resolved]
        private LargeTextureStore largeTexture { get; set; }

        [Resolved]
        private BeatmapStorage beatmaps { get; set; }

        private Container backgroundContainer;

        private Sprite sprite;
        private BufferedContainer currentTexture;
        private Box dimBox;

        private readonly TextureSource source;
        private Vector2 blurSigma;
        private float dim;

        public Background(TextureSource source = TextureSource.Internal, string textureName = @"")
        {
            this.source = source;
            TextureName = textureName;
            TextureSource = source;
            RelativeSizeAxes = Axes.Both;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Children = new Drawable[]
            {
                backgroundContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = currentTexture = new BufferedContainer(cachedFrameBuffer: true)
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                        RedrawOnScale = false,
                        Masking = true,
                        Child = sprite = new Sprite
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            FillMode = FillMode.Fill,
                            Texture = source == TextureSource.Internal ? largeTexture.Get(TextureName) : beatmaps.GetBackground(TextureName)
                        },
                    }
                },
                dimBox = new Box
                {
                    Alpha = dim,
                    Colour = Color4.Black,
                    RelativeSizeAxes = Axes.Both,
                }
            };
        }

        public void BlurTo(Vector2 newBlurSigma, double duration = 0, Easing easing = Easing.None)
        {
            blurSigma = newBlurSigma;
            Schedule(() => currentTexture?.BlurTo(newBlurSigma, duration, easing));
        }

        public void ColorTo(Color4 color4, double duration = 0, Easing easing = Easing.None)
        {
            currentTexture.FadeColour(color4, duration, easing);
        }

        public void DimTo(float newAlpha, double duration = 0, Easing easing = Easing.None)
        {
            dimBox?.FadeTo(newAlpha, duration, easing);
            dim = newAlpha;
        }

        private Sprite loadTexture(TextureSource source, string textureName)
        {
            return new Sprite
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                FillMode = FillMode.Fill,
                Texture = source == TextureSource.Internal ? largeTexture.Get(textureName) : beatmaps.GetBackground(textureName)
            };
        }

        public void ChangeTexture(TextureSource source, string name, double duration = 0, Easing easing = Easing.None)
        {
            if (string.IsNullOrEmpty(name))
                return;

            if (TextureSource == source && TextureName == name)
                return;

            TextureName = name;
            TextureSource = source;
            var queuedTexture = new BufferedContainer(cachedFrameBuffer: true)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Alpha = 0,
                BlurSigma = BlurSigma,
                RelativeSizeAxes = Axes.Both,
                RedrawOnScale = false,
                Masking = true,
                Child = loadTexture(source, name),
            };
            var lastTexture = currentTexture;
            currentTexture = queuedTexture;

            Schedule(() =>
            {
                if (queuedTexture == currentTexture)
                {
                    backgroundContainer.Add(queuedTexture);
                    queuedTexture.FadeIn(duration, easing);
                    trimUnusedBackground(duration);
                }
                else
                    queuedTexture.Dispose();
            });
        }

        private void trimUnusedBackground(double untilExpireDuration)
        {
            foreach (var b in backgroundContainer)
            {
                if (!currentTexture.Equals(b))
                    b.Delay(untilExpireDuration).Expire();
            }
        }
    }

    public enum TextureSource
    {
        Internal,
        External
    }
}
