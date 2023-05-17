#nullable disable

using System.Threading.Tasks;
using Circle.Game.Beatmaps;
using Circle.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK.Graphics;

namespace Circle.Game.Graphics.UserInterface
{
    public partial class BackgroundColorContainer : Container
    {
        private readonly Background background;

        [Resolved]
        private BeatmapStorage beatmaps { get; set; }

        public double ChangeDuration;
        public Easing ChangeEasing;

        public BackgroundColorContainer(Background target)
        {
            background = target;
            background.BackgroundColorChanged += backgroundColorChanged;
        }

        private async void backgroundColorChanged(string texturePath)
        {
            Color4 color = Color4.White;
            byte[] data = beatmaps.Get(texturePath);

            if (data != null)
                color = await ImageUtil.GetAverageColorAsync(data);

            this.FadeColour(color, ChangeDuration, ChangeEasing);
        }
    }
}
