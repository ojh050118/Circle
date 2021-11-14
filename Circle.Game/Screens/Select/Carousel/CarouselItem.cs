﻿using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics;
using osuTK.Graphics;
using osu.Framework.Graphics.Effects;
using Circle.Game.Graphics.UserInterface;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Bindables;
using osuTK;
using osu.Framework.Input.Events;
using osu.Framework.Allocation;
using Circle.Game.Beatmap;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Sprites;

namespace Circle.Game.Screens.Select.Carousel
{
    public class CarouselItem : Container
    {
        public Container BorderContainer;

        protected override Container<Drawable> Content { get; } = new Container { RelativeSizeAxes = Axes.Both };

        public readonly Bindable<CarouselItemState> State = new Bindable<CarouselItemState>(CarouselItemState.NotSelected);

        public BeatmapInfo BeatmapInfo { get; set; }

        private Background background;

        public CarouselItem()
        {
            Size = new Vector2(1, 250);
            RelativeSizeAxes = Axes.X;
            Anchor = Anchor.TopCentre;
            Origin = Anchor.TopCentre;
            InternalChild = BorderContainer = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                CornerRadius = 10,
                BorderColour = Color4.SkyBlue,
            };
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            background = BeatmapInfo.Settings.BackgroundTexture == string.Empty
                ? new Background(textureName: "Duelyst")
                : new Background(TextureSource.External, BeatmapInfo.Settings.BackgroundTexture);

            BorderContainer.Children = new Drawable[]
            {
                background,
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
                            Text = BeatmapInfo.Settings.Track,
                            Font = FontUsage.Default.With("OpenSans-Bold", size: 28)
                        },
                        new SpriteText
                        {
                            Text = BeatmapInfo.Settings.Author,
                            Font = FontUsage.Default.With(size: 24)
                        }
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            State.BindValueChanged(updateState, true);
        }

        private void updateState(ValueChangedEvent<CarouselItemState> state)
        {
            switch (state.NewValue)
            {
                case CarouselItemState.NotSelected:
                case CarouselItemState.Collapsed:
                    BorderContainer.BorderThickness = 0;
                    BorderContainer.EdgeEffect = new EdgeEffectParameters
                    {
                        Type = EdgeEffectType.Shadow,
                        Radius = 10,
                        Colour = Color4.Black.Opacity(100),
                    };
                    break;

                case CarouselItemState.Selected:
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
            State.Value = CarouselItemState.Selected;

            return base.OnClick(e);
        }
    }
}