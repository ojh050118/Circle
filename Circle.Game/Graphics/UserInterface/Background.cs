#nullable disable

using System;
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
    public partial class Background : Container, IEquatable<Background>
    {
        private readonly TextureSource source;

        private Container backgroundContainer;
        private Vector2 blurSigma;

        private BufferedContainer currentTexture;
        private float dim;
        private Box dimBox;

        public Background(TextureSource source = TextureSource.Internal, string textureName = @"")
        {
            this.source = source;
            TextureName = textureName;
            TextureSource = source;
            RelativeSizeAxes = Axes.Both;
        }

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

        public event Action<string> BackgroundColorChanged;

        [Resolved]
        private LargeTextureStore largeTexture { get; set; }

        [Resolved]
        private BeatmapStorage beatmaps { get; set; }

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
                        Child = new Sprite
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
            var texture = source == TextureSource.Internal ? largeTexture.Get(textureName) : beatmaps.GetBackground(textureName);

            return new Sprite
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                FillMode = FillMode.Fill,
                Texture = texture
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

                BackgroundColorChanged?.Invoke(name);
            });
        }

        private void trimUnusedBackground(double untilExpireDuration)
        {
            foreach (var b in backgroundContainer)
            {
                if (!ReferenceEquals(currentTexture, b))
                    b.Delay(untilExpireDuration).Expire();
            }
        }

        public virtual bool Equals(Background other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return other.GetType() == GetType()
                   && other.TextureName == TextureName;
        }
    }

    public enum TextureSource
    {
        Internal,
        External
    }
}
