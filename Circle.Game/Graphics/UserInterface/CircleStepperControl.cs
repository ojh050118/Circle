using Circle.Game.Graphics.Sprites;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Graphics.UserInterface
{
    public partial class CircleStepperControl<T> : StepperControl<T>
    {
        public LocalisableString LabelText
        {
            get => labelText;
            set
            {
                labelText = value;
                text.Text = value;
            }
        }

        private LocalisableString labelText;

        private readonly CircleSpriteText text;

        public CircleStepperControl()
        {
            Masking = true;
            CornerRadius = 5;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.X;
            Height = 40;

            Background.Child = new Box
            {
                Colour = Color4.Black,
                RelativeSizeAxes = Axes.Both,
                Alpha = 0.2f
            };

            Foreground.Padding = new MarginPadding { Left = 20 };
            Foreground.Width = 0.5f;
            Foreground.Children = new Drawable[]
            {
                text = new CircleSpriteText
                {
                    Text = LabelText,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Padding = new MarginPadding { Right = 20 },
                    Font = CircleFont.Default.With(size: 22),
                    RelativeSizeAxes = Axes.X,
                    Truncate = true,
                }
            };
        }

        protected override StepperControlPanel CreateControlPanel() => new CircleStepperControlPanel();

        public partial class CircleStepperControlPanel : StepperControlPanel
        {
            protected override ClickableContainer PreviousButton => prev;
            protected override ClickableContainer NextButton => next;

            private readonly IconButton prev;
            private readonly IconButton next;

            private readonly CircleSpriteText text;

            public CircleStepperControlPanel()
            {
                Anchor = Anchor.CentreRight;
                Origin = Anchor.CentreRight;
                RelativeSizeAxes = Axes.Both;
                Width = 0.5f;
                Padding = new MarginPadding { Right = 20 };
                Children = new Drawable[]
                {
                    text = new CircleSpriteText
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Font = CircleFont.Default.With(size: 22),
                        Truncate = true,
                    },
                    prev = new IconButton
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Icon = FontAwesome.Solid.AngleLeft,
                        Size = new Vector2(30)
                    },
                    next = new IconButton
                    {
                        Anchor = Anchor.CentreRight,
                        Origin = Anchor.CentreRight,
                        Icon = FontAwesome.Solid.AngleRight,
                        Size = new Vector2(30)
                    },
                };
            }

            public override void OnValueChanged<U>(ValueChangedEvent<U> e)
            {
                text.Text = e.NewValue?.ToString() ?? string.Empty;
            }
        }
    }
}
