using System;
using Circle.Game.Beatmaps;
using Circle.Game.Graphics.Containers;
using osu.Framework;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Screens.Select.Carousel
{
    public class CarouselItem : Container, IStateful<SelectionState>
    {
        public Container BorderContainer;

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
            }
        }

        public event Action<SelectionState> StateChanged;

        public BeatmapInfo BeatmapInfo { get; }

        public Action DoubleClicked { get; private set; }

        private Sample sampleClick;
        private Sample sampleDoubleClick;

        public const float ITEM_HEIGHT = 250;

        public CarouselItem(BeatmapInfo info, Action doubleClicked)
        {
            BeatmapInfo = info;
            DoubleClicked = doubleClicked;
        }

        [BackgroundDependencyLoader]
        private void load(LargeTextureStore largeTexture, BeatmapStorage beatmaps, AudioManager audio)
        {
            sampleClick = audio.Samples.Get("SongSelect/select-click");
            sampleDoubleClick = audio.Samples.Get("SongSelect/select-double-click");
            Size = new Vector2(1, ITEM_HEIGHT);
            RelativeSizeAxes = Axes.X;
            Anchor = Anchor.TopCentre;
            Origin = Anchor.TopCentre;
            InternalChild = BorderContainer = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                BorderThickness = 0,
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                CornerRadius = 10,
                BorderColour = Color4.SkyBlue,
                Children = new Drawable[]
                {
                    new Sprite
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                        FillMode = FillMode.Fill,
                        Texture = !beatmaps.Storage.Exists(BeatmapInfo.RelativeBackgroundPath)
                            ? largeTexture.Get("bg1")
                            : beatmaps.GetBackground(BeatmapInfo)
                    },
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Direction = FillDirection.Horizontal,
                        // 이렇게 하면 그래디언트가 수평이 아니라 ~40° 각도로 대각선이 됩니다.
                        Shear = new Vector2(0.8f, 0),
                        Alpha = 0.5f,
                        Children = new[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = Color4.Black,
                                Width = 0.4f,
                            },
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = ColourInfo.GradientHorizontal(Color4.Black, new Color4(0f, 0f, 0f, 0.9f)),
                                Width = 0.05f,
                            },
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = ColourInfo.GradientHorizontal(new Color4(0f, 0f, 0f, 0.9f), new Color4(0f, 0f, 0f, 0.1f)),
                                Width = 0.2f,
                            },
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = ColourInfo.GradientHorizontal(new Color4(0f, 0f, 0f, 0.1f), new Color4(0, 0, 0, 0)),
                                Width = 0.05f,
                            },
                        }
                    },
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Direction = FillDirection.Vertical,
                        Padding = new MarginPadding { Horizontal = 20, Vertical = 10 },
                        Spacing = new Vector2(5),
                        Children = new Drawable[]
                        {
                            new SpriteText
                            {
                                Text = BeatmapInfo.Beatmap.Settings.Song,
                                Font = FontUsage.Default.With("OpenSans-Bold", size: 30)
                            },
                            new SpriteText
                            {
                                Text = BeatmapInfo.Beatmap.Settings.Author,
                                Font = FontUsage.Default.With(size: 24)
                            }
                        }
                    }
                },
                EdgeEffect = new EdgeEffectParameters
                {
                    Type = EdgeEffectType.Shadow,
                    Radius = 10,
                    Colour = Color4.Black.Opacity(100),
                }
            };

            DoubleClicked += () => sampleDoubleClick?.Play();
            StateChanged += updateState;
            StateChanged += state =>
            {
                if (state == SelectionState.Selected)
                    sampleClick?.Play();
            };
        }

        private void updateState(SelectionState state)
        {
            switch (state)
            {
                case SelectionState.NotSelected:
                    BorderContainer.BorderThickness = 0;
                    BorderContainer.EdgeEffect = new EdgeEffectParameters
                    {
                        Type = EdgeEffectType.Shadow,
                        Radius = 10,
                        Colour = Color4.Black.Opacity(100),
                    };
                    break;

                case SelectionState.Selected:
                    BorderContainer.BorderThickness = 2.5f;
                    BorderContainer.EdgeEffect = new EdgeEffectParameters
                    {
                        Type = EdgeEffectType.Glow,
                        Colour = Color4.SkyBlue.Opacity(0.6f),
                        Radius = 20,
                        Roundness = 10,
                    };
                    break;
            }
        }

        protected override bool OnClick(ClickEvent e)
        {
            if (State == SelectionState.Selected)
                DoubleClicked?.Invoke();

            State = SelectionState.Selected;

            return base.OnClick(e);
        }
    }
}
