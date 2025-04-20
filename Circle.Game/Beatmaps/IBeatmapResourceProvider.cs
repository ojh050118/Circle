using Circle.Game.IO;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;

namespace Circle.Game.Beatmaps
{
    public interface IBeatmapResourceProvider : IStorageResourceProvider
    {
        /// <summary>
        /// Retrieve a global large texture store, used for loading beatmap backgrounds.
        /// </summary>
        TextureStore LargeTextureStore { get; }

        /// <summary>
        /// Access a global track store for retrieving beatmap tracks from.
        /// </summary>
        ITrackStore Tracks { get; }
    }
}
