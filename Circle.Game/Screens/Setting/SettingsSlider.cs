#nullable disable

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
    public partial class SettingsSlider<T> : Container
        where T : struct, IEquatable<T>, IComparable<T>, IConvertible
    {
        public string Text
        {
            get => text.Text.ToString();
            set => text.Text = value;
        }

        public Bindable<T> Current
        {
            get => sliderBar.Current;
            set => sliderBar.Current = value;
        }

        public float KeyboardStep
        {
            get => sliderBar.KeyboardStep;
            set => sliderBar.KeyboardStep = value;
        }

        public bool TransferValueOnCommit
        {
            get => sliderBar.TransferValueOnCommit;
            set => sliderBar.TransferValueOnCommit = value;
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

        private readonly IconButton leftIcon;
        private readonly IconButton rightIcon;
        private readonly SpriteText text;

        private CircleSliderBar<T> sliderBar { get; }

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
                new GridContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    RowDimensions = new[]
                    {
                        new Dimension(),
                        new Dimension(GridSizeMode.Relative),
                    },
                    Content = new[]
                    {
                        new Drawable[]
                        {
                            text = new SpriteText
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Font = FontUsage.Default.With(size: 22),
                                Padding = new MarginPadding { Left = 20 },
                                Truncate = true,
                            },
                            new Container
                            {
                                Anchor = Anchor.CentreRight,
                                Origin = Anchor.CentreRight,
                                RelativeSizeAxes = Axes.Both,
                                Padding = new MarginPadding { Right = 20 },
                                Children = new Drawable[]
                                {
                                    leftIcon = new IconButton
                                    {
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        Size = new Vector2(30),
                                        Action = () => sliderBar?.Commit(false)
                                    },
                                    rightIcon = new IconButton
                                    {
                                        Anchor = Anchor.CentreRight,
                                        Origin = Anchor.CentreRight,
                                        Size = new Vector2(30),
                                        Action = () => sliderBar?.Commit(true)
                                    },
                                    new Container
                                    {
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        Padding = new MarginPadding { Horizontal = 50 },
                                        RelativeSizeAxes = Axes.X,
                                        AutoSizeAxes = Axes.Y,
                                        Child = sliderBar = new CircleSliderBar<T>
                                        {
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            RelativeSizeAxes = Axes.X,
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
