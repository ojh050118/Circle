using System;
using Circle.Game.Graphics.UserInterface;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Screens.Setting
{
    public class SettingsSlider<T> : Container
        where T : struct, IEquatable<T>, IComparable<T>, IConvertible
    {
        public string Text
        {
            get => text.Text.ToString();
            set => text.Text = value;
        }
        private CircleSliderBar<T> circleSliderBar { get; }

        public BindableNumber<T> Current
        {
            get => (BindableNumber<T>)circleSliderBar.Current;
            set => circleSliderBar.Current = value;
        }

        public float KeyboardStep
        {
            get => circleSliderBar.KeyboardStep;
            set => circleSliderBar.KeyboardStep = value;
        }

        public IconUsage LeftIcon
        {
            get => leftIcon.Icon;
            set => leftIcon.Icon = value;
        }

        public IconUsage RightIcon
        {
            get => rightIcon.Icon;
            set => rightIcon.Icon = value;
        }

        private readonly SpriteIcon leftIcon;
        private readonly SpriteIcon rightIcon;
        private readonly SpriteText text;

        public SettingsSlider()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.X;
            Height = 40;
            Masking = true;
            CornerRadius = 5;
            Children = new Drawable[]
            {
                new Box
                {
                    Colour = Color4.Black,
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0.2f
                },
                text = new SpriteText
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Font = FontUsage.Default.With(size: 22),
                    Margin = new MarginPadding { Left = 20 }
                },
                new Container
                {
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Margin = new MarginPadding { Right = 20 },
                    Width = 0.4f,
                    Children = new Drawable[]
                    {
                        leftIcon = new SpriteIcon
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Size = new Vector2(30),
                        },
                        rightIcon = new SpriteIcon
                        {
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.CentreRight,
                            Size = new Vector2(30),
                        },
                        new Container
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Padding = new MarginPadding { Horizontal = 50 },
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Child = circleSliderBar = new CircleSliderBar<T>
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                RelativeSizeAxes = Axes.X,
                            }
                        }
                        
                    }
                }
            };
        }
    }
}
