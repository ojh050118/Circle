using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;

namespace Circle.Game.Graphics.Sprites
{
    public partial class GlowingSpriteText : Container, IHasText
    {
        private readonly SpriteText spriteText, blurredText;

        public LocalisableString Text
        {
            get => spriteText.Text;
            set => blurredText.Text = spriteText.Text = value;
        }

        public FontUsage Font
        {
            get => spriteText.Font;
            set => blurredText.Font = spriteText.Font = value.With(fixedWidth: true);
        }

        public Vector2 TextSize
        {
            get => spriteText.Size;
            set => blurredText.Size = spriteText.Size = value;
        }

        public ColourInfo TextColour
        {
            get => spriteText.Colour;
            set => spriteText.Colour = value;
        }

        public ColourInfo GlowColour
        {
            get => blurredText.Colour;
            set => blurredText.Colour = value;
        }

        public Vector2 Spacing
        {
            get => spriteText.Spacing;
            set => spriteText.Spacing = blurredText.Spacing = value;
        }

        public bool UseFullGlyphHeight
        {
            get => spriteText.UseFullGlyphHeight;
            set => spriteText.UseFullGlyphHeight = blurredText.UseFullGlyphHeight = value;
        }

        public Bindable<string> Current
        {
            get => spriteText.Current;
            set => spriteText.Current = value;
        }

        public GlowingSpriteText()
        {
            AutoSizeAxes = Axes.Both;

            Children = new Drawable[]
            {
                new BufferedContainer(cachedFrameBuffer: true)
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    BlurSigma = new Vector2(3),
                    RedrawOnScale = false,
                    RelativeSizeAxes = Axes.Both,
                    Blending = BlendingParameters.Additive,
                    Size = new Vector2(3f),
                    Children = new[]
                    {
                        blurredText = new SpriteText
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Font = CircleFont.Default
                        },
                    },
                },
                spriteText = new SpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Font = CircleFont.Default
                },
            };
        }
    }
}
