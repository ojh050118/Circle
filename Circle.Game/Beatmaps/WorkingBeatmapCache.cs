#nullable disable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Circle.Game.IO;
using JetBrains.Annotations;
using Newtonsoft.Json;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Rendering.Dummy;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Statistics;

namespace Circle.Game.Beatmaps
{
    public class WorkingBeatmapCache : IBeatmapResourceProvider
    {
        private readonly List<BeatmapManagerWorkingBeatmap> workingCache = new List<BeatmapManagerWorkingBeatmap>();

        /// <summary>
        /// Beatmap files may specify this filename to denote that they don't have an audio track.
        /// </summary>
        private const string virtual_track_filename = @"virtual";

        /// <summary>
        /// A default representation of a WorkingBeatmap to use when no beatmap is available.
        /// </summary>
        public readonly WorkingBeatmap DefaultBeatmap;

        private readonly AudioManager audioManager;
        private readonly IResourceStore<byte[]> resources;
        private readonly LargeTextureStore largeTextureStore;
        private readonly LargeTextureStore gameTextureStore;
        private readonly ITrackStore trackStore;
        private readonly IResourceStore<byte[]> files;

        [CanBeNull]
        private readonly GameHost host;

        public WorkingBeatmapCache(ITrackStore trackStore, AudioManager audioManager, IResourceStore<byte[]> resources, IResourceStore<byte[]> files, WorkingBeatmap defaultBeatmap = null,
                                   GameHost host = null)
        {
            DefaultBeatmap = defaultBeatmap;

            this.audioManager = audioManager;
            this.resources = resources;
            this.host = host;
            this.files = files;
            largeTextureStore = new LargeTextureStore(host?.Renderer ?? new DummyRenderer(), host?.CreateTextureLoaderStore(files));
            gameTextureStore = new LargeTextureStore(host?.Renderer, host?.CreateTextureLoaderStore(new NamespacedResourceStore<byte[]>(resources, @"Textures")));
            this.trackStore = trackStore;
        }

        public void Invalidate(BeatmapSetInfo info)
        {
            foreach (var b in info.Beatmaps)
                Invalidate(b);
        }

        public void Invalidate(BeatmapInfo info)
        {
            lock (workingCache)
            {
                var working = workingCache.FirstOrDefault(w => info.Equals(w.BeatmapInfo));

                if (working != null)
                {
                    Logger.Log($"Invalidating working beatmap cache for {info}");
                    workingCache.Remove(working);
                    OnInvalidated?.Invoke(working);
                }
            }
        }

        public event Action<WorkingBeatmap> OnInvalidated;

        public virtual WorkingBeatmap GetWorkingBeatmap([CanBeNull] BeatmapInfo beatmapInfo)
        {
            if (beatmapInfo == null || ReferenceEquals(beatmapInfo, DefaultBeatmap.BeatmapInfo))
                return DefaultBeatmap;

            lock (workingCache)
            {
                var working = workingCache.FirstOrDefault(w => beatmapInfo.Equals(w.BeatmapInfo));

                if (working != null)
                    return working;

                workingCache.Add(working = new BeatmapManagerWorkingBeatmap(beatmapInfo, this));

                // best effort; may be higher than expected.
                GlobalStatistics.Get<int>("Beatmaps", $"Cached {nameof(WorkingBeatmap)}s").Value = workingCache.Count;

                return working;
            }
        }

        #region IResourceStorageProvider

        TextureStore IBeatmapResourceProvider.LargeTextureStore => largeTextureStore;
        TextureStore IBeatmapResourceProvider.GameTextureStore => gameTextureStore;
        ITrackStore IBeatmapResourceProvider.Tracks => trackStore;
        IRenderer IStorageResourceProvider.Renderer => host?.Renderer ?? new DummyRenderer();
        AudioManager IStorageResourceProvider.AudioManager => audioManager;
        IResourceStore<byte[]> IStorageResourceProvider.Files => files;
        IResourceStore<byte[]> IStorageResourceProvider.Resources => resources;
        IResourceStore<TextureUpload> IStorageResourceProvider.CreateTextureLoaderStore(IResourceStore<byte[]> underlyingStore) => host?.CreateTextureLoaderStore(underlyingStore);

        #endregion

        private class BeatmapManagerWorkingBeatmap : WorkingBeatmap
        {
            [NotNull]
            private readonly IBeatmapResourceProvider resources;

            public BeatmapManagerWorkingBeatmap(BeatmapInfo beatmapInfo, [NotNull] IBeatmapResourceProvider resources)
                : base(beatmapInfo, resources.AudioManager)
            {
                this.resources = resources;
            }

            protected override Beatmap GetBeatmap()
            {
                if (BeatmapInfo.File == null || !BeatmapInfo.File.Exists)
                    return new Beatmap { BeatmapInfo = BeatmapInfo };

                try
                {
                    string fileStorePath = Path.Combine(BeatmapInfo.File.Directory!.Name, BeatmapInfo.File.Name);

                    var stream = GetStream(fileStorePath);

                    if (stream == null)
                    {
                        Logger.Log($"Beatmap failed to load (file {BeatmapInfo.File.Name} not found on disk at expected location {fileStorePath}).", level: LogLevel.Error);
                        return null;
                    }

                    using (var reader = new StreamReader(stream))
                        return JsonConvert.DeserializeObject<Beatmap>(reader.ReadToEnd());
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Beatmap failed to load");
                    return null;
                }
            }

            public override Texture GetBackground() => getBackgroundFromStore(resources.LargeTextureStore);

            public override Stream GetVideo()
            {
                if (string.IsNullOrEmpty(Metadata?.BgVideo) || BeatmapInfo.File == null)
                    return null;

                try
                {
                    string fileStorePath = Path.Combine(BeatmapInfo.File.Directory!.Name, Metadata.BgVideo);

                    if (resources.Files.GetAvailableResources().FirstOrDefault(f => f == fileStorePath) == null)
                    {
                        Logger.Log($"Beatmap video failed to load (file {Metadata.BgVideo} not found on disk at expected location {fileStorePath}).");
                        return null;
                    }

                    var video = GetStream(fileStorePath);

                    return video;
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Video failed to load");
                    return null;
                }
            }

            public override byte[] Get(string name) => resources.Files.Get(name);

            private Texture getBackgroundFromStore(TextureStore store)
            {
                if (string.IsNullOrEmpty(Metadata?.BgImage) || BeatmapInfo.File == null)
                    return resources.GameTextureStore.Get("bg1");

                try
                {
                    string fileStorePath = Path.Combine(BeatmapInfo.File.Directory!.Name, Metadata.BgImage);
                    var texture = store.Get(fileStorePath);

                    if (texture == null)
                    {
                        Logger.Log($"Beatmap background failed to load (file {Metadata.BgImage} not found on disk at expected location {fileStorePath}).");
                        return resources.GameTextureStore.Get("bg1");
                    }

                    return texture;
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Background failed to load");
                    return resources.GameTextureStore.Get("bg1");
                }
            }

            protected override Track GetBeatmapTrack()
            {
                if (string.IsNullOrEmpty(Metadata.SongFileName) || BeatmapInfo.File == null)
                    return null;

                if (Metadata.SongFileName == virtual_track_filename)
                    return null;

                try
                {
                    string fileStorePath = Path.Combine(BeatmapInfo.File.Directory!.Name, Metadata.SongFileName);
                    var track = resources.Tracks.Get(fileStorePath);

                    if (track == null)
                    {
                        Logger.Log($"Beatmap failed to load (file {Metadata.SongFileName} not found on disk at expected location {fileStorePath}).", level: LogLevel.Error);
                        return null;
                    }

                    return track;
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Track failed to load");
                    return null;
                }
            }

            public override Stream GetStream(string storagePath) => resources.Files.GetStream(storagePath);
        }
    }
}
