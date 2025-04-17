#nullable disable

using System.Threading;
using System.Threading.Tasks;
using Circle.Game.Beatmaps;
using Circle.Game.Graphics.UserInterface;
using Circle.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK.Graphics;

namespace Circle.Game.Graphics.Containers
{
    public partial class BackgroundColorContainer : Container
    {
        private CancellationTokenSource dataGetCancellation;

        [Resolved]
        private BeatmapStorage beatmaps { get; set; }

        public double ChangeDuration;
        public Easing ChangeEasing;
        public Color4 DefaultColour;

        public BackgroundColorContainer(Background target)
        {
            target.BackgroundColorChanged += async t => await backgroundColorChanged(t).ConfigureAwait(false);
        }

        [BackgroundDependencyLoader]
        private void load() => Colour = DefaultColour;

        private async Task backgroundColorChanged(string texturePath)
        {
            dataGetCancellation?.CancelAsync().ConfigureAwait(false);

            Color4 color = DefaultColour;
            byte[] data = await beatmaps.GetAsync(texturePath, (dataGetCancellation = new CancellationTokenSource()).Token).ConfigureAwait(true);

            if (data != null)
                color = await ImageUtil.GetAverageColorAsync(data).ConfigureAwait(true);

            this.FadeColour(color, ChangeDuration, ChangeEasing);
        }
    }
}
