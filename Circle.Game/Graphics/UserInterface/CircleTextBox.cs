using Circle.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Graphics.UserInterface
{
    public partial class CircleTextBox : TextBox
    {
        private readonly Box background;
        private readonly Container focusBorderContainer;

        private Color4 backgroundCommit;

        public CircleTextBox()
        {
            Add(new Container
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                CornerRadius = 5,
                Depth = 1,
                Children = new Drawable[]
                {
                    background = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.Black.Opacity(0)
                    },
                    focusBorderContainer = new Container
                    {
                        Alpha = 1,
                        RelativeSizeAxes = Axes.Both,
                        Masking = true,
                        CornerRadius = 5,
                        BorderColour = Color4.DeepSkyBlue,
                        Child = new Box { RelativeSizeAxes = Axes.Both, Colour = Color4.White.Opacity(0) }
                    }
                }
            });

            TextContainer.Height = 0.75f;
        }

        [BackgroundDependencyLoader]
        private void load(CircleColour colours)
        {
            background.Colour = colours.TransparentBlack;
            backgroundCommit = colours.TransparentGray;
        }

        protected override SpriteText CreatePlaceholder() => new CircleSpriteText
        {
            Anchor = Anchor.CentreLeft,
            Origin = Anchor.CentreLeft,
            Colour = Color4.White.Opacity(0.5f),
            X = 2
        };

        protected override Drawable GetDrawableCharacter(char c) => new CircleSpriteText
        {
            Text = c.ToString()
        };

        protected override Caret CreateCaret() => new CircleCaret
        {
            CaretWidth = 2,
            SelectionColour = Color4.DeepSkyBlue
        };

        protected override void NotifyInputError()
        {
            focusBorderContainer.TransformTo(nameof(focusBorderContainer.BorderColour), Color4.Red)
                                .FadeColour(Color4.DeepSkyBlue, 200, Easing.Out);
        }

        protected override void OnTextCommitted(bool textChanged)
        {
            base.OnTextCommitted(textChanged);

            background.FlashColour(backgroundCommit, 400);
        }

        protected override void OnFocusLost(FocusLostEvent e)
        {
            base.OnFocusLost(e);

            focusBorderContainer.TransformTo(nameof(focusBorderContainer.BorderThickness), 0f, 500, Easing.OutPow10);
        }

        protected override void OnFocus(FocusEvent e)
        {
            base.OnFocus(e);

            focusBorderContainer.TransformTo(nameof(focusBorderContainer.BorderThickness), 3.5f, 500, Easing.OutPow10);
        }

        public partial class CircleCaret : Caret
        {
            public float CaretWidth { get; set; }

            public Color4 SelectionColour { get; set; }

            public CircleCaret()
            {
                Colour = Color4.Transparent;

                InternalChild = new Container
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    RelativeSizeAxes = Axes.Both,
                    Height = 0.8f,
                    CornerRadius = 1f,
                    Masking = true,
                    Child = new Box { RelativeSizeAxes = Axes.Both },
                };
            }

            public override void Hide() => this.FadeOut(500f, Easing.OutPow10);

            public override void DisplayAt(Vector2 position, float? selectionWidth)
            {
                if (selectionWidth != null)
                {
                    this.MoveTo(new Vector2(position.X, position.Y), 60, Easing.Out);
                    this.ResizeWidthTo(selectionWidth.Value + CaretWidth / 2, 75, Easing.Out);
                    this
                        .FadeTo(0.5f, 200, Easing.Out)
                        .FadeColour(SelectionColour, 200, Easing.Out);
                }
                else
                {
                    this.MoveTo(new Vector2(position.X - CaretWidth / 2, position.Y), 60, Easing.Out);
                    this.ResizeWidthTo(CaretWidth, 75, Easing.Out);
                    this
                        .FadeColour(Color4.White, 200, Easing.Out)
                        .Loop(c => c.FadeTo(0.7f).FadeTo(0.4f, 500, Easing.InOutSine));
                }
            }
        }
    }
}
