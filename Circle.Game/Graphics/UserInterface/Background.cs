﻿using Circle.Game.Beatmap;
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
        [Resolved]
        private LargeTextureStore largeTexture { get; set; }

        [Resolved]
        private BeatmapResourcesManager beatmapResources { get; set; }

        private readonly Sprite sprite;
        private BufferedContainer currentTexture;
        private Box dimBox;

        private readonly string textureName;
        private readonly TextureSource source;
        private Vector2 blurSigma;
        private float dim;

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

        public Background(TextureSource source = TextureSource.Internal, string textureName = @"")
        {
            this.source = source;
            this.textureName = textureName;
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
            if (!string.IsNullOrEmpty(textureName))
                sprite.Texture = source == TextureSource.Internal ? largeTexture.Get(textureName) : beatmapResources.GetBackground(textureName);
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
                Texture = source == TextureSource.Internal ? largeTexture.Get(textureName) : beatmapResources.GetBackground(textureName)
            };
        }

        public void ChangeTexture(TextureSource source, string name, double duration = 0, Easing easing = Easing.None)
        {
            if (string.IsNullOrEmpty(name))
                return;

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
