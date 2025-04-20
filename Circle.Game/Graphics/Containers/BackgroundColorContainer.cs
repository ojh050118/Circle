#nullable disable

using System.IO;
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
        private CancellationTokenSource colorGetCancellation = new CancellationTokenSource();

        [Resolved]
        private BeatmapManager beatmapManager { get; set; }

        public double ChangeDuration;
        public Easing ChangeEasing;
        public Color4 DefaultColour;

        public BackgroundColorContainer(Background target)
        {
            target.BackgroundColorChanged += async (t, b) => await backgroundColorChanged(b).ConfigureAwait(false);
        }

        [BackgroundDependencyLoader]
        private void load() => Colour = DefaultColour;

        private async Task backgroundColorChanged(BeatmapInfo beatmapInfo)
        {
            await colorGetCancellation.CancelAsync().ConfigureAwait(true);
            colorGetCancellation.Dispose();

            colorGetCancellation = new CancellationTokenSource();

            Color4 color = DefaultColour;
            byte[] data = beatmapManager.GetWorkingBeatmap(beatmapInfo).Get(Path.Combine(beatmapInfo.File.Directory?.Name ?? string.Empty, beatmapInfo.Metadata.BgImage));

            if (data != null)
                color = await ImageUtil.GetAverageColorAsync(data, colorGetCancellation.Token).ConfigureAwait(true);

            this.FadeColour(color, ChangeDuration, ChangeEasing);
        }
    }
}
