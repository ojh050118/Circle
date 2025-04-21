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
        [Resolved]
        private BeatmapManager beatmapManager { get; set; }

        public double ChangeDuration;
        public Easing ChangeEasing;
        public Color4 DefaultColour;

        private CancellationTokenSource backgroundColorCancellationTokenSource;

        public BackgroundColorContainer(Background target)
        {
            target.BackgroundColorChanged += (_, b) =>
            {
                backgroundColorCancellationTokenSource?.Cancel();
                backgroundColorCancellationTokenSource = new CancellationTokenSource();

                Task.Run(async () => await backgroundColorChanged(b).ConfigureAwait(false), cancellationToken: backgroundColorCancellationTokenSource.Token);
            };
        }

        [BackgroundDependencyLoader]
        private void load() => Colour = DefaultColour;

        private async Task backgroundColorChanged(BeatmapInfo beatmapInfo)
        {
            Color4 color = DefaultColour;
            byte[] data = beatmapManager.GetWorkingBeatmap(beatmapInfo).Get(Path.Combine(beatmapInfo.File.Directory?.Name ?? string.Empty, beatmapInfo.Metadata.BgImage));

            if (data != null)
                color = await ImageUtil.GetAverageColorAsync(data).ConfigureAwait(true);

            Schedule(() => this.FadeColour(color, ChangeDuration, ChangeEasing));
        }
    }
}
