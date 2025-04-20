#nullable disable

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
            Texture = beatmapManager.GetWorkingBeatmap(beatmapInfo).GetBackground();
        }
    }
}
