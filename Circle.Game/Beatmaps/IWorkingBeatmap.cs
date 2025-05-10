#nullable disable

using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;

namespace Circle.Game.Beatmaps
{
    public interface IWorkingBeatmap
    {
        bool BeatmapLoaded { get; }

        bool TrackLoaded { get; }

        BeatmapInfo BeatmapInfo { get; }

        Beatmap Beatmap { get; }

        Track Track { get; }

        Texture GetBackground();

        Track LoadTrack();
    }
}
