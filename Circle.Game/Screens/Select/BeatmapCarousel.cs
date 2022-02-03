using System;
using System.Linq;
using Circle.Game.Beatmap;
using Circle.Game.Graphics.Containers;
using Circle.Game.Graphics.UserInterface;
using Circle.Game.Overlays;
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
        protected readonly CarouselScrollContainer Scroll;

        private int oldScrollHeight;

        [Resolved]
        private Bindable<BeatmapInfo> working { get; set; }

        [Resolved]
        private MusicController music { get; set; }

        [Resolved]
        private Background background { get; set; }

        public Bindable<bool> PlayRequested { get; set; } = new Bindable<bool>(false);

        public BeatmapCarousel()
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

        [BackgroundDependencyLoader]
        private void load(BeatmapStorage beatmaps)
        {
            foreach (var beatmap in beatmaps.GetBeatmaps())
                Scroll.Child.Add(new CarouselItem { BeatmapInfo = beatmap });

            foreach (var item in Scroll.Child.Children)
                item.State.BindValueChanged(state => onChangedItemState(item, state));
        }

        private void onChangedItemState(CarouselItem item, ValueChangedEvent<CarouselItemState> state)
        {
            switch (state.NewValue)
            {
                case CarouselItemState.NotSelected:
                    break;

                case CarouselItemState.Selected:
                    if (state.OldValue == CarouselItemState.NotSelected)
                        onSelected(item);
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

            if (!string.IsNullOrEmpty(item.BeatmapInfo.Settings.BgImage))
                background.FadeTextureTo(TextureSource.External, item.BeatmapInfo.Settings.BgImage, 1000, Easing.OutPow10);
            else
                background.FadeTextureTo(TextureSource.Internal, "Duelyst", 1000, Easing.OutPow10);

            if (!working.Value.Equals(item.BeatmapInfo))
            {
                working.Value = item.BeatmapInfo;
                music.ChangeTrack(item.BeatmapInfo);
                music.SeekTo(item.BeatmapInfo.Settings.PreviewSongStart * 1000);
                music.Play();
            }

            foreach (var notSelected in Scroll.Child.Children)
            {
                if (notSelected != item)
                    notSelected.State.Value = CarouselItemState.NotSelected;
            }
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            if (Scroll.Child.Children.Count == 0)
                return;

            if (string.IsNullOrEmpty(working.Value.Settings.Song))
            {
                var idx = new Random().Next(0, Scroll.Child.Children.Count);
                Scheduler.AddDelayed(() => Scroll.Child.Children[idx].State.Value = CarouselItemState.Selected, 50);
                return;
            }

            foreach (var item in Scroll.Child.Children)
            {
                if (item.BeatmapInfo.Equals(working.Value))
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

            // 부동소수점 끼리 비교할 수 없기 때문에 정수로 비교.
            if (oldScrollHeight != (int)Math.Round(Scroll.ScrollContent.Height))
                oldScrollHeight = (int)Math.Round(Scroll.ScrollContent.Height);
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
