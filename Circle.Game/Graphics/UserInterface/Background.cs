using System.Linq;
using Circle.Game.Beatmap;
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
        private BufferedContainer bufferedContainer;
        private BufferedContainer newBufferedContainer;
        private Box dim;

        public readonly Sprite Sprite;

        private readonly string textureName;

        public Vector2 BlurSigma
        {
            get
            {
                if (bufferedContainer == null)
                    return newBufferedContainer.BlurSigma;
                else
                    return bufferedContainer.BlurSigma;
            }
            set => bufferedContainer.BlurSigma = value;
        }

        public float Dim;

        private readonly TextureSource source;

        [Resolved]
        private LargeTextureStore largeTexture { get; set; }

        [Resolved]
        private BeatmapResourcesManager beatmapResources { get; set; }

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
        private void load()
        {
            if (!string.IsNullOrEmpty(textureName))
                Sprite.Texture = source == TextureSource.Internal ? largeTexture.Get(textureName) : beatmapResources.GetBackground(textureName);
        }

        public void BlurTo(Vector2 newBlurSigma, double duration = 0, Easing easing = Easing.None)
        {
            Schedule(() => bufferedContainer?.BlurTo(newBlurSigma, duration, easing));
            Schedule(() => newBufferedContainer?.BlurTo(newBlurSigma, duration, easing));
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
            Dim = newAlpha;
        }

        private Sprite loadTexture(TextureSource source, string textureName)
        {
            return new Sprite
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                FillMode = FillMode.Fill,
                Texture = source == TextureSource.Internal ? largeTexture.Get(textureName) : beatmapResources.GetBackground(textureName)
            };
        }

        public void FadeTextureTo(TextureSource source, string textureName, double duration = 0, Easing easing = Easing.None)
        {
            if (Sprite is null || string.IsNullOrEmpty(textureName))
                return;

            AddInternal(newBufferedContainer = new BufferedContainer(cachedFrameBuffer: true)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Alpha = 0,
                RelativeSizeAxes = Axes.Both,
                RedrawOnScale = false,
                Masking = true,
                Child = loadTexture(source, textureName),
            });
            if (dim != null)
                ChangeInternalChildDepth(dim, -1);

            newBufferedContainer.BlurSigma = BlurSigma;
            newBufferedContainer.FadeIn(duration, easing).Then().Schedule(() =>
            {
                RemoveInternal(InternalChildren.First());
                bufferedContainer = newBufferedContainer;
            });
        }
    }

    public enum TextureSource
    {
        Internal,
        External
    }
}
