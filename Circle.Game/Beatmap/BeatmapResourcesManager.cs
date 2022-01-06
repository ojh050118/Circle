using System;
using System.IO;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace Circle.Game.Beatmap
{
    public class BeatmapResourcesManager
    {
        private readonly LargeTextureStore largeTextureStore;
        private readonly ITrackStore trackStore;
        private readonly Storage files;

        public BeatmapResourcesManager(Storage storage, AudioManager audioManager, GameHost host = null)
        {
            trackStore = audioManager.GetTrackStore(new StorageBackedResourceStore(storage.GetStorageForDirectory("tracks")));
            files = storage;
            largeTextureStore = new LargeTextureStore(host?.CreateTextureLoaderStore(new StorageBackedResourceStore(storage.GetStorageForDirectory("backgrounds"))));
        }

        public Texture GetBackground(BeatmapInfo info)
        {
            if (string.IsNullOrEmpty(info.Settings.BgImage))
                return null;

            var backgrounds = files.GetStorageForDirectory("backgrounds");

            try
            {
                return largeTextureStore.Get(Path.Combine(backgrounds.GetFullPath(string.Empty), $"{info.Settings.BgImage}"));
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to load background.");
                return null;
            }
        }

        public Texture GetBackground(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            var backgrounds = files.GetStorageForDirectory("backgrounds");

            try
            {
                return largeTextureStore.Get(Path.Combine(backgrounds.GetFullPath(string.Empty), $"{name}"));
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to load background.");
                return null;
            }
        }

        public Track GetBeatmapTrack(BeatmapInfo info)
        {
            if (string.IsNullOrEmpty(info.Settings.SongFileName))
                return null;

            var tracks = files.GetStorageForDirectory("tracks");

            try
            {
                return trackStore.Get(Path.Combine(tracks.GetFullPath(string.Empty), $"{info.Settings.SongFileName}"));
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to load beatmap track.");
                return null;
            }
        }

        public Track GetBeatmapTrack(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            var tracks = files.GetStorageForDirectory("tracks");

            try
            {
                return trackStore.Get(Path.Combine(tracks.GetFullPath(string.Empty), $"{name}"));
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to load beatmap track.");
                return null;
            }
        }
    }
}
