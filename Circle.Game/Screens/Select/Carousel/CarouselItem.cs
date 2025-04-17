#nullable disable

using System;
using Circle.Game.Beatmaps;
using Circle.Game.Graphics.Containers;
using Circle.Game.Overlays;
using JetBrains.Annotations;
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
using osuTK.Graphics;
using osuTK.Input;

namespace Circle.Game.Screens.Select.Carousel
{
    public partial class CarouselItem : PoolableDrawable, IStateful<SelectionState>
    {
        public const float ITEM_HEIGHT = 250;

        public BeatmapInfo BeatmapInfo { get; }
        public Container BorderContainer;

        public Action DoubleClicked { get; private set; }

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

        private Sample sampleClick;
        private Sample sampleDoubleClick;

        private SelectionState state;

        private Container<Drawable> defaultProxyTarget;
        private Container<Drawable> currentProxyTarget;
        private Drawable proxy;

        [Resolved]
        private CarouselItemOverlay carouselItemOverlay { get; set; }

        public CarouselItem(BeatmapInfo info, Action doubleClicked, Container defaultContainer)
        {
            BeatmapInfo = info;
            DoubleClicked = doubleClicked;
            SetupDefaultContainer(defaultContainer);
        }

        public void UpdateItem()
        {
            DelayedLoadUnloadWrapper background;
            DelayedLoadUnloadWrapper text;

            BorderContainer.Children = new Drawable[]
            {
                background = new DelayedLoadUnloadWrapper(() => new PanelBackground(BeatmapInfo), 200)
                {
                    RelativeSizeAxes = Axes.Both
                },
                text = new DelayedLoadUnloadWrapper(() => new PanelContent(BeatmapInfo), 100)
                {
                    RelativeSizeAxes = Axes.Both
                }
            };

            background.DelayedLoadComplete += fadeInContent;
            text.DelayedLoadComplete += fadeInContent;
        }

        public Drawable ProxyToContainer(Container<Drawable> c)
        {
            if (currentProxyTarget != null)
                throw new InvalidOperationException("Previous proxy usage was not returned");

            if (defaultProxyTarget == null)
                throw new InvalidOperationException($"{nameof(SetupDefaultContainer)} must be called first");

            currentProxyTarget = c;

            defaultProxyTarget.Remove(proxy, false);
            currentProxyTarget.Add(proxy);
            return proxy;
        }

        public void ReturnProxy()
        {
            if (currentProxyTarget == null)
                return;

            if (defaultProxyTarget == null)
                throw new InvalidOperationException($"{nameof(SetupDefaultContainer)} must be called first");

            currentProxyTarget.Remove(proxy, false);
            currentProxyTarget = null;

            defaultProxyTarget.Add(proxy);
        }

        public void SetupDefaultContainer(Container<Drawable> container)
        {
            defaultProxyTarget = container;

            proxy = CreateProxy();
            proxy.Depth = 1;

            defaultProxyTarget.Add(proxy);
        }

        public void ChangeAnchor(Anchor anchor)
        {
            var previousAnchor = AnchorPosition;
            Anchor = anchor;
            Position -= AnchorPosition - previousAnchor;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            UpdateItem();
        }

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            if (e.Key == Key.ShiftLeft && State == SelectionState.Selected)
            {
                carouselItemOverlay.Push(this);
            }

            return base.OnKeyDown(e);
        }

        protected override bool OnClick(ClickEvent e)
        {
            if (State == SelectionState.Selected)
                DoubleClicked?.Invoke();

            State = SelectionState.Selected;

            return base.OnClick(e);
        }

        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {
            sampleClick = audio.Samples.Get("SongSelect/select-click");
            sampleDoubleClick = audio.Samples.Get("SongSelect/select-double-click");
            Height = ITEM_HEIGHT;
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

        [CanBeNull]
        public event Action<SelectionState> StateChanged;
    }
}
