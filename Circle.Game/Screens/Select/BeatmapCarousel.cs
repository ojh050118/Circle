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
    public class BeatmapCarousel : CompositeDrawable
    {
        protected CarouselScrollContainer Scroll;

        public int Count => Scroll.Child.Children.Count;

        public Bindable<CarouselItem> SelectedItem { get; private set; }

        public BeatmapCarousel()
        {
            SelectedItem = new Bindable<CarouselItem>();
        }

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
                    Child = new FillFlowContainer<CarouselItem>
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

        public void AddItem(BeatmapInfo info, Action onDoubleClicked)
        {
            CarouselItem item;
            Scroll.Child.Add(item = new CarouselItem { BeatmapInfo = info, DoubleClicked = onDoubleClicked });
            item.State.BindValueChanged(state => onChangedItemState(item, state));

            if (SelectedItem.Value != null)
                updateItemScale(SelectedItem.Value);
        }

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();

            Scroll.ScrollContent.Height = Scroll.ScrollContent.Child.Height + DrawHeight;
        }

        public void SelectBeatmap(VerticalDirection direction)
        {
            if (Scroll.Child.Children?.Count == 0 || SelectedItem.Value == null)
                return;

            int idx = Scroll.Child.IndexOf(SelectedItem.Value);

            if (direction == VerticalDirection.Up)
            {
                if (idx > 0)
                    Scroll.Child.Children[idx - 1].State.Value = CarouselItemState.Selected;
                else
                    Scroll.Child.Children.Last().State.Value = CarouselItemState.Selected;
            }
            else
            {
                if (idx < Scroll.Child.Count - 1)
                    Scroll.Child.Children[idx + 1].State.Value = CarouselItemState.Selected;
                else
                    Scroll.Child.Children.First().State.Value = CarouselItemState.Selected;
            }
        }

        public void SelectBeatmap(BeatmapInfo beatmap)
        {
            foreach (var item in Scroll.Child.Children)
            {
                if (beatmap.Equals(item.BeatmapInfo))
                    item.State.Value = CarouselItemState.Selected;
            }
        }

        private void onChangedItemState(CarouselItem item, ValueChangedEvent<CarouselItemState> state)
        {
            switch (state.NewValue)
            {
                case CarouselItemState.NotSelected:
                    break;

                case CarouselItemState.Selected:
                    onSelected(item);
                    break;
            }
        }

        private void onSelected(CarouselItem item)
        {
            updateItemScale(item);
            Scroll.ScrollTo(getScaledPositionY(item) + item.Height / 2);

            SelectedItem.Value = item;

            foreach (var notSelected in Scroll.Child.Children)
            {
                if (notSelected != item)
                    notSelected.State.Value = CarouselItemState.NotSelected;
            }
        }

        private void updateItemScale(CarouselItem item)
        {
            var idx = Scroll.Child.Children.ToList().FindIndex(i => i.Equals(item));
            float nextScale = 1;

            for (int i = idx; i < Scroll.Child.Children.Count; i++)
            {
                Scroll.Child.Children[i].ScaleTo(nextScale, 1000, Easing.OutPow10);
                if (nextScale > 0.7f)
                    nextScale -= 0.1f;
            }

            nextScale = 0.9f;

            for (int i = idx - 1; i >= 0; i--)
            {
                Scroll.Child.Children[i].ScaleTo(nextScale, 1000, Easing.OutPow10);
                if (nextScale > 0.7f)
                    nextScale -= 0.1f;
            }
        }

        private float getScaledPositionY(CarouselItem item)
        {
            var idx = Scroll.Child.Children.ToList().FindIndex(i => i.Equals(item));
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

        protected class CarouselScrollContainer : CircleScrollContainer<FillFlowContainer<CarouselItem>>
        {
            public CarouselScrollContainer()
            {
                ScrollContent.AutoSizeAxes = Axes.None;
                Masking = false;
            }
        }
    }
}
