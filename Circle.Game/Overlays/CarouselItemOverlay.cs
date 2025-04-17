using System;
using Circle.Game.Graphics.Containers;
using Circle.Game.Screens.Select.Carousel;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Circle.Game.Overlays
{
    public partial class CarouselItemOverlay : CircleFocusedOverlayContainer
    {
        private CarouselItem? currentItem;

        public CarouselItemOverlay()
        {
            Content = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
            };
        }

        public void Push(CarouselItem target)
        {
            ArgumentNullException.ThrowIfNull(target);

            target.ProxyToContainer((Container)Content);
            currentItem = target;
            target.BorderContainer.MoveToX(-500, 750, Easing.OutPow10);

            Show();
        }

        public override void Hide()
        {
            currentItem?.BorderContainer.MoveToX(0, 750, Easing.OutPow10);
            base.Hide();

            currentItem?.ReturnProxy();
        }
    }
}
