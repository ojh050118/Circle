#nullable disable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Extensions;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace Circle.Game.Beatmaps
{
    public class BeatmapStorage : IResourceStore<byte[]>
    {
        public Storage Storage { get; }
        private readonly LargeTextureStore largeTextureStore;
        private readonly IResourceStore<byte[]> localStore;
        private readonly ITrackStore trackStore;

        public BeatmapStorage(Storage files, AudioManager audioManager, IResourceStore<byte[]> store, GameHost host = null)
        {
            Storage = files;
            largeTextureStore = new LargeTextureStore(host?.Renderer, host?.CreateTextureLoaderStore(new StorageBackedResourceStore(files)));
            trackStore = audioManager.GetTrackStore(new StorageBackedResourceStore(files));
            localStore = store;
        }

        #region Disposal

        public void Dispose()
        {
            largeTextureStore?.Dispose();
            trackStore?.Dispose();
            localStore?.Dispose();
        }

        #endregion

        public byte[] Get(string name)
        {
            using (Stream stream = Storage.GetStream(name))
            {
                if (stream == null)
                    return localStore.Get(name);

                return stream.ReadAllBytesToArray();
            }
        }

        public async Task<byte[]> GetAsync(string name, CancellationToken cancellationToken = default)
        {
            using (Stream stream = Storage.GetStream(name))
            {
                if (stream == null)
                    return await localStore.GetAsync(name, cancellationToken);

                return await stream.ReadAllBytesToArrayAsync(cancellationToken);
            }
        }

        public Stream GetStream(string name)
        {
            if (localStore.GetStream(name) == null)
                return Storage.GetStream(name);

            return localStore.GetStream(name);
        }

        public IEnumerable<string> GetAvailableResources()
        {
            return Storage.GetDirectories(string.Empty).Where(d => Directory.GetFiles(d, "*.circle").Length != 0);
        }

        public IList<BeatmapInfo> GetBeatmapInfos()
        {
            List<BeatmapInfo> beatmapInfo = new List<BeatmapInfo>();

            try
            {
                foreach (string dir in Storage.GetDirectories(string.Empty))
                {
                    DirectoryInfo di = new DirectoryInfo(Storage.GetFullPath(dir));

                    foreach (var fi in di.GetFiles("*.circle"))
                    {
                        string fileDir = fi.Directory?.Name ?? string.Empty;

                        var beatmap = GetBeatmap(Path.Combine(fileDir, fi.Name));
                        beatmapInfo.Add(new BeatmapInfo(beatmap, fi));
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log($"Error getting Beatmap Infos: {e.Message}");
            }

            return beatmapInfo;
        }

        public async Task<List<BeatmapInfo>> GetBeatmapInfosAsync()
        {
            List<BeatmapInfo> beatmapInfo = new List<BeatmapInfo>();

            try
            {
                foreach (string dir in Storage.GetDirectories(string.Empty))
                {
                    DirectoryInfo di = new DirectoryInfo(Storage.GetFullPath(dir));

                    foreach (var fi in di.GetFiles("*.circle"))
                    {
                        string fileDir = fi.Directory?.Name ?? string.Empty;

                        var beatmap = await GetBeatmapAsync(Path.Combine(fileDir, fi.Name));
                        beatmapInfo.Add(new BeatmapInfo(beatmap, fi));
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log($"Error getting Beatmap Infos: {e.Message}");
            }

            return beatmapInfo;
        }

        public BeatmapInfo GetBeatmapInfo(Beatmap beatmap)
        {
            foreach (string dir in Storage.GetDirectories(string.Empty))
            {
                DirectoryInfo di = new DirectoryInfo(Storage.GetFullPath(dir));

                foreach (var fi in di.GetFiles("*.circle"))
                {
                    string fileDir = fi.Directory?.Name ?? string.Empty;

                    if (beatmap == GetBeatmap(Path.Combine(fileDir, fi.Name)))
                        return new BeatmapInfo(beatmap, fi);
                }
            }

            return null;
        }

        public async Task<BeatmapInfo> GetBeatmapInfoAsync(Beatmap beatmap)
        {
            foreach (string dir in Storage.GetDirectories(string.Empty))
            {
                DirectoryInfo di = new DirectoryInfo(Storage.GetFullPath(dir));

                foreach (var fi in di.GetFiles("*.circle"))
                {
                    string fileDir = fi.Directory?.Name ?? string.Empty;

                    if (beatmap == await GetBeatmapAsync(Path.Combine(fileDir, fi.Name)))
                        return new BeatmapInfo(beatmap, fi);
                }
            }

            return null;
        }

        /// <summary>
        /// BeatmapInfo를 반환합니다.
        /// </summary>
        /// <param name="path">상대 경로.</param>
        /// <returns>비트맵.</returns>
        public Beatmap GetBeatmap(string path)
        {
            Beatmap beatmap = default;

            if (string.IsNullOrEmpty(path))
                return null;

            using (StreamReader sr = File.OpenText(Storage.GetFullPath(path)))
            {
                try
                {
                    beatmap = JsonConvert.DeserializeObject<Beatmap>(sr.ReadToEnd());
                }
                catch
                {
                    Logger.Log($"Failed to parse beatmap. File path: {path}");
                }
            }

            return beatmap;
        }

        /// <summary>
        /// BeatmapInfo를 반환합니다.
        /// </summary>
        /// <param name="path">상대 경로.</param>
        /// <returns>비트맵.</returns>
        public async Task<Beatmap> GetBeatmapAsync(string path)
        {
            Beatmap beatmap = default;

            if (string.IsNullOrEmpty(path))
                return null;

            using (StreamReader sr = File.OpenText(Storage.GetFullPath(path)))
            {
                try
                {
                    string data = await sr.ReadToEndAsync();
                    beatmap = JsonConvert.DeserializeObject<Beatmap>(data);
                }
                catch
                {
                    Logger.Log($"Failed to parse beatmap. File path: {path}");
                }
            }

            return beatmap;
        }

        public Texture GetBackground(BeatmapInfo info) => GetBackground(info.RelativeBackgroundPath);

        public async Task<Texture> GetBackgroundAsync(BeatmapInfo info, CancellationToken cancellationToken = default) => await GetBackgroundAsync(info.RelativeBackgroundPath, cancellationToken);

        public Texture GetBackground(string name)
        {
            if (!Storage.Exists(name))
                return null;

            try
            {
                return largeTextureStore.Get(name);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to load background.");
                return null;
            }
        }

        public async Task<Texture> GetBackgroundAsync(string name, CancellationToken cancellationToken = default)
        {
            if (!Storage.Exists(name))
                return null;

            try
            {
                return await largeTextureStore.GetAsync(name, cancellationToken);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to load background.");
                return null;
            }
        }

        public Stream GetVideo(string name)
        {
            if (!Storage.Exists(name))
                return null;

            try
            {
                return File.Open(name, FileMode.Open);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to load video.");
                return null;
            }
        }

        public Stream GetVideo(BeatmapInfo info) => GetVideo(info.VideoPath);

        public Track GetTrack(BeatmapInfo info) => GetTrack(info.RelativeSongPath);

        public Track GetTrack(string name)
        {
            if (!Storage.Exists(name))
                return new TrackVirtual(1000);

            try
            {
                return trackStore.Get(name);
            }
            catch
            {
                Logger.Log($"Failed to load beatmap track({name}).");
                return null;
            }
        }

        public async Task<Track> GetTrackAsync(string name)
        {
            if (!Storage.Exists(name))
                return new TrackVirtual(1000);

            try
            {
                return await trackStore.GetAsync(name);
            }
            catch
            {
                Logger.Log($"Failed to load beatmap track({name}).");
                return null;
            }
        }

        /// <summary>
        /// 비트맵을 파일로 저장합니다.
        /// </summary>
        /// <param name="beatmap">저장할 비트맵.</param>
        public void Create(Beatmap beatmap)
        {
            string json = JsonConvert.SerializeObject(beatmap);

            string fileName = $"[{beatmap.Settings.Author}] {beatmap.Settings.Artist} - {beatmap.Settings.Song}.circle";
            string path = Storage.GetStorageForDirectory(Path.GetFileNameWithoutExtension(fileName)).GetFullPath(string.Empty);

            using (StreamWriter sw = File.CreateText(Path.Combine(path, fileName)))
                sw.WriteLine(json);
        }

        /// <summary>
        /// 비트맵 파일을 삭제합니다.
        /// </summary>
        /// <param name="beatmap">삭제할 비트맵.</param>
        /// <param name="deleteResources">비트맵에 사용된 리소스 삭제 여부.</param>
        public void Delete(BeatmapInfo beatmap, bool deleteResources)
        {
            beatmap.Delete();

            if (deleteResources)
            {
                File.Delete(Storage.GetFullPath(beatmap.RelativeSongPath));
                File.Delete(Storage.GetFullPath(beatmap.RelativeBackgroundPath));
            }
        }
    }
}
