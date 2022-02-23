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
    public class Background : CompositeDrawable
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

        private readonly Sprite sprite;
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
            AddInternal(currentTexture = new BufferedContainer(cachedFrameBuffer: true)
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
                },
            });
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            if (!string.IsNullOrEmpty(TextureName))
                sprite.Texture = source == TextureSource.Internal ? largeTexture.Get(TextureName) : beatmaps.GetBackground(TextureName);
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
            if (dimBox == null)
            {
                AddInternal(dimBox = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black,
                    Alpha = 0
                });
            }

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
                    AddInternal(queuedTexture);
                    if (dimBox != null)
                        ChangeInternalChildDepth(dimBox, -1);
                    queuedTexture.FadeIn(duration, easing).Then().Schedule(() => lastTexture.Expire());
                }
                else
                    queuedTexture.Dispose();
            });
        }
    }

    public enum TextureSource
    {
        Internal,
        External
    }
}
