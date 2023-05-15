#nullable disable

using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

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
        private void load(LargeTextureStore largeTextures, BeatmapStorage beatmaps)
        {
            Texture = !beatmaps.Storage.Exists(beatmapInfo.RelativeBackgroundPath)
                ? largeTextures.Get("bg1")
                : beatmaps.GetBackground(beatmapInfo);
        }
    }
}
