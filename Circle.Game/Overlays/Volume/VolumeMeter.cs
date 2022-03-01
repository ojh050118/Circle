using System;
using System.Globalization;
using Circle.Game.Graphics.Containers;
using Circle.Game.Input;
using osu.Framework;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;
using Shape = osu.Framework.Graphics.Shapes;

namespace Circle.Game.Overlays.Volume
{
    public class VolumeMeter : Container, IKeyBindingHandler<InputAction>, IStateful<SelectionState>
    {
        private CircularProgress volumeCircle;
        private CircularProgress volumeCircleGlow;
        private SpriteText volumeText;
        private CircularContainer meterContainer;
        private Sample sampleHover;

        private readonly string name;
        private readonly float circleSize;
        private readonly Color4 meterColour;

        public BindableDouble Current { get; } = new BindableDouble { MinValue = 0, MaxValue = 1, Precision = 0.01 };

        public event Action<SelectionState> StateChanged;

        private SelectionState state;

        public SelectionState State
        {
            get => state;
            set
            {
                if (state == value)
                    return;

                state = value;
                StateChanged?.Invoke(value);

                updateState();
            }
        }

        private double displayVolume;

        private const int blur_amount = 5;
        private const float volume_circle_size = 0.93f;
        private const float inner_radius = 0.12f;
        private const float background_opacity = 0.3f;

        public double DisplayVolume
        {
            get => displayVolume;
            set
            {
                displayVolume = value;
                int intValue = (int)Math.Round(value * 100);

                volumeCircleGlow.Current.Value = displayVolume;
                volumeCircle.Current.Value = displayVolume;
                volumeText.Text = intValue.ToString();
            }
        }

        public double Volume
        {
            get => Current.Value;
            private set => Current.Value = value;
        }

        public VolumeMeter(string name, float circleSize, Color4 meterColour)
        {
            this.name = name;
            this.circleSize = circleSize;
            this.meterColour = meterColour;

            AutoSizeAxes = Axes.Both;
            Alpha = 0.5f;
        }

        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {
            sampleHover = audio.Samples.Get("button-hover");
            Children = new Drawable[]
            {
                meterContainer = new CircularContainer
                {
                    Size = new Vector2(circleSize),
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        new Shape.Circle
                        {
                            Colour = Color4.Black.Opacity(background_opacity),
                            RelativeSizeAxes = Axes.Both,
                        },
                        volumeText = new SpriteText
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Text = displayVolume.ToString(CultureInfo.InvariantCulture),
                            Font = FontUsage.Default.With("OpenSans-Bold", size: 0.3f * circleSize)
                        },
                        new Container
                        {
                            Name = "Circle background",
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            RelativeSizeAxes = Axes.Both,
                            Size = new Vector2(volume_circle_size),
                            Padding = new MarginPadding(blur_amount * 2),
                            Child = new CircularProgress
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                RelativeSizeAxes = Axes.Both,
                                Colour = Color4.Black.Opacity(0.4f),
                                InnerRadius = inner_radius,
                                Current = new Bindable<double>(1)
                            }
                        },
                        new Container
                        {
                            Name = "Circle glow",
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            RelativeSizeAxes = Axes.Both,
                            Size = new Vector2(volume_circle_size),
                            Children = new Drawable[]
                            {
                                (volumeCircleGlow = new CircularProgress
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = meterColour,
                                    InnerRadius = inner_radius,
                                }).WithEffect(new GlowEffect
                                {
                                    Colour = meterColour,
                                    BlurSigma = new Vector2(blur_amount),
                                    Strength = 2,
                                    PadExtent = true
                                }),
                            }
                        },
                        new Container
                        {
                            Name = "Circle white",
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            RelativeSizeAxes = Axes.Both,
                            Size = new Vector2(volume_circle_size),
                            Padding = new MarginPadding(blur_amount * 2),
                            Child = volumeCircle = new CircularProgress
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                RelativeSizeAxes = Axes.Both,
                                Colour = Color4.White.Opacity(0.7f),
                                InnerRadius = inner_radius,
                            }
                        },
                    },
                    EdgeEffect = new EdgeEffectParameters
                    {
                        Type = EdgeEffectType.Shadow,
                        Radius = 15,
                        Colour = Color4.Black.Opacity(background_opacity),
                    }
                },
                new CircularContainer
                {
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    Margin = new MarginPadding { Right = circleSize * 1.1f },
                    Masking = true,
                    AutoSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        new Shape.Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Color4.Black.Opacity(background_opacity),
                        },
                        new Container
                        {
                            AutoSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Horizontal = 15 },
                            Child = new SpriteText
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Text = name,
                                Font = FontUsage.Default.With("OpenSans-Bold", size: 24)
                            }
                        }
                    }
                }
            };

            Current.BindValueChanged(volume => this.TransformTo(nameof(DisplayVolume), volume.NewValue, 500, Easing.OutQuint), true);
        }

        private void updateState()
        {
            switch (state)
            {
                case SelectionState.NotSelected:
                    this.FadeTo(0.5f, 500);
                    meterContainer.ScaleTo(1, 500, Easing.OutQuint);
                    break;

                case SelectionState.Selected:
                    this.FadeTo(1, 250);
                    meterContainer.ScaleTo(1.03f, 250, Easing.OutQuint);
                    break;
            }
        }

        public void Increase(double amount = 5) => Volume += amount / 100;

        public void Decrease(double amount = 5) => Volume -= amount / 100;

        protected override bool OnScroll(ScrollEvent e)
        {
            Volume += 0.05 * e.ScrollDelta.Y;

            return true;
        }

        protected override bool OnHover(HoverEvent e)
        {
            State = SelectionState.Selected;
            sampleHover?.Play();

            return false;
        }

        protected override bool OnMouseMove(MouseMoveEvent e)
        {
            State = SelectionState.Selected;

            return base.OnMouseMove(e);
        }

        public bool OnPressed(KeyBindingPressEvent<InputAction> e)
        {
            if (!IsHovered)
                return false;

            switch (e.Action)
            {
                case InputAction.PreviousBeatmap:
                    State = SelectionState.Selected;
                    Increase();
                    return true;

                case InputAction.NextBeatmap:
                    State = SelectionState.Selected;
                    Decrease();
                    return true;
            }

            return false;
        }

        public void OnReleased(KeyBindingReleaseEvent<InputAction> e)
        {
        }
    }
}
