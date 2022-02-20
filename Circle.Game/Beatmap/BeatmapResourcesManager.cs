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

        public readonly Storage Backgrounds;
        public readonly Storage Tracks;

        public BeatmapResourcesManager(Storage storage, AudioManager audioManager, GameHost host = null)
        {
            var files = storage;
            Backgrounds = files.GetStorageForDirectory("backgrounds");
            Tracks = files.GetStorageForDirectory("tracks");
            largeTextureStore = new LargeTextureStore(host?.CreateTextureLoaderStore(new StorageBackedResourceStore(Backgrounds)));
            trackStore = audioManager.GetTrackStore(new StorageBackedResourceStore(Tracks));
        }

        public Texture GetBackground(BeatmapInfo info)
        {
            if (string.IsNullOrEmpty(info.Settings.BgImage))
                return null;

            try
            {
                return largeTextureStore.Get(Path.Combine(Backgrounds.GetFullPath(string.Empty), $"{info.Settings.BgImage}"));
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

            try
            {
                return largeTextureStore.Get(Path.Combine(Backgrounds.GetFullPath(string.Empty), $"{name}"));
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

            try
            {
                return trackStore.Get(Path.Combine(Tracks.GetFullPath(string.Empty), $"{info.Settings.SongFileName}"));
            }
            catch
            {
                Logger.Log($"Failed to load beatmap track({info.Settings.SongFileName}).");
                return new TrackVirtual(1000);
            }
        }

        public Track GetBeatmapTrack(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            try
            {
                return trackStore.Get(Path.Combine(Tracks.GetFullPath(string.Empty), $"{name}"));
            }
            catch
            {
                Logger.Log($"Failed to load beatmap track({name}).");
                return new TrackVirtual(1000);
            }
        }
    }
}
