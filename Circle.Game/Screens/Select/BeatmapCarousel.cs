#nullable disable

using System;
using System.Linq;
using Circle.Game.Beatmaps;
using Circle.Game.Graphics.Containers;
using Circle.Game.Screens.Select.Carousel;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace Circle.Game.Screens.Select
{
    public partial class BeatmapCarousel : Container
    {
        private SelectionCycleFillFlowContainer<CarouselItem> carouselItems;
        protected CarouselScrollContainer Scroll;

        public int ItemCount => carouselItems.Count;

        public Bindable<CarouselItem> SelectedItem { get; private set; } = new Bindable<CarouselItem>();

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;
            InternalChildren = new Drawable[]
            {
                Scroll = new CarouselScrollContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = false,
                    Child = carouselItems = new SelectionCycleFillFlowContainer<CarouselItem>
                    {
                        AutoSizeAxes = Axes.Y,
                        RelativeSizeAxes = Axes.X,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Spacing = new Vector2(10),
                        Direction = FillDirection.Vertical,
                    }
                }
            };
        }

        public void Add(BeatmapInfo info, Action onDoubleClicked)
        {
            CarouselItem item;
            carouselItems.Add(item = new CarouselItem(info, onDoubleClicked));
            item.StateChanged += state =>
            {
                if (state == SelectionState.Selected)
                    updateItems();
            };

            if (carouselItems.Selected != null)
                updateItems(false);
        }

        public void Select(BeatmapInfo info)
        {
            var item = carouselItems.FirstOrDefault(i => i.BeatmapInfo.Equals(info));

            if (item == null)
                return;

            carouselItems.Select(item);
        }

        public void SelectNext()
        {
            if (carouselItems.Count == 0)
                return;

            carouselItems.SelectNext();
        }

        public void SelectPrevious()
        {
            if (carouselItems.Count == 0)
                return;

            carouselItems.SelectPrevious();
        }

        private void updateItems(bool scroll = true)
        {
            SelectedItem.Value = carouselItems.Selected;
            updateItemScale();
            if (scroll)
                Scroll.ScrollTo(getScaledPositionY() + CarouselItem.ITEM_HEIGHT / 2);
        }

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();

            Scroll.ScrollContent.Height = carouselItems.Height + DrawHeight;
        }

        private void updateItemScale()
        {
            var idx = carouselItems.IndexOf(carouselItems.Selected);
            float nextScale = 1;

            for (int i = idx; i < carouselItems.Count; i++)
            {
                carouselItems.Children[i].ScaleTo(nextScale, 1000, Easing.OutPow10);
                if (nextScale > 0.7f)
                    nextScale -= 0.1f;
            }

            nextScale = 0.9f;

            for (int i = idx - 1; i >= 0; i--)
            {
                carouselItems.Children[i].ScaleTo(nextScale, 1000, Easing.OutPow10);
                if (nextScale > 0.7f)
                    nextScale -= 0.1f;
            }
        }

        private float getScaledPositionY()
        {
            var idx = carouselItems.IndexOf(carouselItems.Selected);
            float totalY = 0;
            float nextScale = 1;

            for (int i = idx - 1; i >= 0; i--)
            {
                if (nextScale > 0.7f)
                    nextScale -= 0.1f;

                totalY += 250 * nextScale;
                totalY += 10;
            }

            return totalY;
        }

        protected class CarouselScrollContainer : CircleScrollContainer<SelectionCycleFillFlowContainer<CarouselItem>>
        {
            public CarouselScrollContainer()
            {
                ScrollContent.AutoSizeAxes = Axes.None;
                Masking = false;
            }
        }
    }
}
