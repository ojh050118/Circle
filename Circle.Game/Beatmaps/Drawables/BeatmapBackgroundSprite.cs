#nullable disable

using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;

namespace Circle.Game.Beatmaps.Drawables
{
    public partial class BeatmapBackgroundSprite : Sprite
    {
        private readonly BeatmapInfo beatmapInfo;

        public BeatmapBackgroundSprite(BeatmapInfo info)
        {
            beatmapInfo = info;
        }

        [BackgroundDependencyLoader]
        private void load(BeatmapManager beatmapManager)
        {
            Task.Factory.StartNew(() =>
            {
                var background = beatmapManager.GetWorkingBeatmap(beatmapInfo).GetBackground();

                Schedule(() => Texture = background);
            }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
    }
}
