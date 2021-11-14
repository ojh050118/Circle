using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using Circle.Game.Graphics.Containers;
using Circle.Game.Screens.Select.Carousel;
using osuTK;
using Circle.Game.Beatmap;

namespace Circle.Game.Screens.Select
{
    public class BeatmapCarousel : CompositeDrawable
    {
        protected readonly CarouselScrollContiner Scroll;

        private CarouselItem selectedCarouselItem => Scroll?.Child.Children.FirstOrDefault(i => i.State.Value == CarouselItemState.Selected);

        private int oldScrollHeight;

        public BeatmapCarousel()
        {
            RelativeSizeAxes = Axes.Both;
            InternalChildren = new Drawable[]
            {
                Scroll = new CarouselScrollContiner
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
            {
                Scroll.Child.Add(new CarouselItem
                {
                    BeatmapInfo = beatmap
                });
            }

            foreach (var item in Scroll.Child.Children)
            {
                item.State.ValueChanged += v =>
                {
                    updateItemScale();

                    if (v.NewValue == CarouselItemState.Selected)
                    {
                        Scroll.ScrollTo(item.Y + item.Height / 2);

                        foreach (var item2 in Scroll.Child.Children)
                        {
                            if (item2 != item)
                                item2.State.Value = CarouselItemState.Collapsed;
                        }
                    }
                };
            }
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            var idx = new Random().Next(0, Scroll.Child.Children.Count);
            Scheduler.AddDelayed(() => Scroll.Child.Children[idx].State.Value = CarouselItemState.Selected, 50);
        }

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();

            Scroll.ScrollContent.Height = DrawHeight + Scroll.ScrollContent.Child.Height;

            // 부동소수점 끼리 비교할 수 없기 때문에 정수로 비교.
            if (oldScrollHeight != (int)Math.Round(Scroll.ScrollContent.Height))
            {
                oldScrollHeight = (int)Math.Round(Scroll.ScrollContent.Height);

                // CarouselItem 스케일이 조정되면서 스크롤 콘텐츠 높이가 조정됩니다.
                // 높이가 조정되면서 스크롤 위치에서 오차가 발생할 수 있습니다.
                // 따라서 높이 조정이 멈출 때 까지 다시 스크롤 합니다.
                if (selectedCarouselItem != null)
                    Scroll.ScrollTo(selectedCarouselItem.Y + selectedCarouselItem.Height / 2);
            }
        }

        private void updateItemScale()
        {
            var idx = Scroll.Child.Children.ToList().FindIndex(i => i.Equals(selectedCarouselItem));
            float nextScale = 1;

            for (int i = idx; i < Scroll.Child.Children.Count; i++)
            {
                Scroll.Child.Children[i].ScaleTo(nextScale, 1000, Easing.OutPow10);
                if (nextScale >= 0.7f)
                    nextScale -= 0.1f;
            }

            nextScale = 1;

            for (int i = idx; i >= 0; i--)
            {
                Scroll.Child.Children[i].ScaleTo(nextScale, 1000, Easing.OutPow10);
                if (nextScale >= 0.7f)
                    nextScale -= 0.1f;
            }
        }

        protected class CarouselScrollContiner : CircleScrollContainer<FillFlowContainer<CarouselItem>>
        {
            public CarouselScrollContiner()
            {
                ScrollContent.AutoSizeAxes = Axes.None;
                Masking = false;
            }
        }
    }
}
