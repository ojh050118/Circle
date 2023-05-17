#nullable disable

using System.Threading;
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
        private CancellationTokenSource dataGetCancellation;

        [Resolved]
        private BeatmapStorage beatmaps { get; set; }

        public double ChangeDuration;
        public Easing ChangeEasing;

        public BackgroundColorContainer(Background target)
        {
            background = target;
            background.BackgroundColorChanged += async t => await backgroundColorChanged(t);
        }

        private async Task backgroundColorChanged(string texturePath)
        {
            dataGetCancellation?.Cancel();

            Color4 color = Color4.White;
            byte[] data = await beatmaps.GetAsync(texturePath, (dataGetCancellation = new CancellationTokenSource()).Token);

            if (data != null)
                color = await ImageUtil.GetAverageColorAsync(data);

            this.FadeColour(color, ChangeDuration, ChangeEasing);
        }
    }
}
