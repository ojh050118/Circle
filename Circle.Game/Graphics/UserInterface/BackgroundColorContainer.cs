#nullable disable

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK.Graphics;

namespace Circle.Game.Graphics.UserInterface
{
    public partial class BackgroundColorContainer : Container
    {
        private readonly Background background;

        public double ChangeDuration;
        public Easing ChangeEasing;

        public BackgroundColorContainer(Background target)
        {
            background = target;
            background.BackgroundColorChanged += backgroundColorChanged;
        }

        private void backgroundColorChanged(Color4 color)
        {
            this.FadeColour(color, ChangeDuration, ChangeEasing);
        }
    }
}
