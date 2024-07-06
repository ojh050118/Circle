using Circle.Game.Graphics.Containers;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Circle.Game.Overlays
{
    public partial class CarouselItemOverlay : CircleFocusedOverlayContainer
    {
        private Drawable currentItem;

        public CarouselItemOverlay()
        {
            Content = new Container()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
            };
        }

        public void Push(Drawable item)
        {
            currentItem = item;

            Content.Clear();
            Content.Add(item);

            Show();
        }
    }
}
