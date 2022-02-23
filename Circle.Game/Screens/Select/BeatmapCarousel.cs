﻿using System;
using System.Linq;
using Circle.Game.Beatmaps;
using Circle.Game.Graphics.Containers;
using Circle.Game.Input;
using Circle.Game.Overlays;
using Circle.Game.Screens.Select.Carousel;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;

namespace Circle.Game.Screens.Select
{
    public class BeatmapCarousel : CompositeDrawable, IKeyBindingHandler<InputAction>
    {
        protected CarouselScrollContainer Scroll;

        [Resolved]
        private BeatmapManager beatmapManager { get; set; }

        [Resolved]
        private MusicController music { get; set; }

        public Bindable<bool> PlayRequested { get; set; } = new Bindable<bool>(false);

        private CarouselItem selectedItem;

        /// <summary>
        /// BeatmapDetails에서 비트맵을 변경할 때 필요합니다.
        /// BeatmapDetails에서 BeatmapManager.OnBeatmapChanged 이벤트를 구독하면 TextFlowContainer에서 오류가 발생하기 떄문입니다.
        /// </summary>
        public Bindable<BeatmapInfo> SelectedBeatmap;

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

            SelectedBeatmap = new Bindable<BeatmapInfo>(beatmapManager.CurrentBeatmap);
            if (beatmapManager.LoadedBeatmaps == null)
                beatmapManager.ReloadBeatmaps();

            foreach (var beatmap in beatmapManager.LoadedBeatmaps)
                Scroll.Child.Add(new CarouselItem { BeatmapInfo = beatmap });

            foreach (var item in Scroll.Child.Children)
                item.State.BindValueChanged(state => onChangedItemState(item, state));
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            if (Scroll.Child.Children.Count == 0)
                return;

            if (string.IsNullOrEmpty(beatmapManager.CurrentBeatmap?.ToString()))
            {
                var idx = new Random().Next(0, Scroll.Child.Children.Count);
                Scheduler.AddDelayed(() => Scroll.Child.Children[idx].State.Value = CarouselItemState.Selected, 50);
                return;
            }

            foreach (var item in Scroll.Child.Children)
            {
                if (item.BeatmapInfo.Equals(beatmapManager.CurrentBeatmap))
                {
                    Scheduler.AddDelayed(() => item.State.Value = CarouselItemState.Selected, 50);
                    break;
                }
            }
        }

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();

            Scroll.ScrollContent.Height = Scroll.ScrollContent.Child.Height + DrawHeight;
        }

        public virtual bool OnPressed(KeyBindingPressEvent<InputAction> e)
        {
            if (e.Action == InputAction.Select && selectedItem != null)
            {
                selectedItem.State.Value = CarouselItemState.PlayRequested;
                return true;
            }

            return false;
        }

        public virtual void OnReleased(KeyBindingReleaseEvent<InputAction> e)
        {
        }

        public void SelectBeatmap(VerticalDirection direction)
        {
            if (Scroll.Child.Children?.Count == 0 || selectedItem == null)
                return;

            int idx = Scroll.Child.IndexOf(selectedItem);

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
                    if (state.OldValue == CarouselItemState.NotSelected)
                    {
                        onSelected(item);
                        selectedItem = item;
                    }
                    else
                        item.State.Value = CarouselItemState.PlayRequested;

                    break;

                case CarouselItemState.PlayRequested:
                    PlayRequested.Value = true;
                    break;
            }
        }

        private void onSelected(CarouselItem item)
        {
            updateItemScale(item);
            Scroll.ScrollTo(getScaledPositionY(item) + item.Height / 2);

            if (beatmapManager.CurrentBeatmap == null || beatmapManager.CurrentBeatmap != item.BeatmapInfo)
            {
                beatmapManager.CurrentBeatmap = item.BeatmapInfo;
                SelectedBeatmap.Value = beatmapManager.CurrentBeatmap;
                music.ChangeTrack(item.BeatmapInfo);
                music.SeekTo(item.BeatmapInfo.Beatmap.Settings.PreviewSongStart * 1000);
                music.Play();
            }

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
