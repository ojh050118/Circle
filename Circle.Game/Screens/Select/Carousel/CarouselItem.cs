using System;
using Circle.Game.Beatmaps;
using Circle.Game.Graphics.Containers;
using osu.Framework;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Pooling;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Screens.Select.Carousel
{
    public class CarouselItem : PoolableDrawable, IStateful<SelectionState>
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
        private void load(AudioManager audio)
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

        protected override void LoadComplete()
        {
            base.LoadComplete();

            UpdateItem();
        }

        public void UpdateItem()
        {
            DelayedLoadWrapper background;
            DelayedLoadWrapper text;

            BorderContainer.Children = new Drawable[]
            {
                background = new DelayedLoadWrapper(() => new PanelBackground(BeatmapInfo), 300)
                {
                    RelativeSizeAxes = Axes.Both
                },
                text = new DelayedLoadWrapper(() => new PanelContent(BeatmapInfo), 100)
                {
                    RelativeSizeAxes = Axes.Both
                }
            };

            background.DelayedLoadComplete += fadeInContent;
            text.DelayedLoadComplete += fadeInContent;
        }

        private void fadeInContent(Drawable d) => d.FadeInFromZero(750, Easing.OutQuint);

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
