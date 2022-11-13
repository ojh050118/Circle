using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
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
        private readonly ITrackStore trackStore;
        private readonly IResourceStore<byte[]> localStore;

        public BeatmapStorage(Storage files, AudioManager audioManager, IResourceStore<byte[]> store, GameHost host = null)
        {
            Storage = files;
            largeTextureStore = new LargeTextureStore(host?.Renderer, host?.CreateTextureLoaderStore(new StorageBackedResourceStore(files)));
            trackStore = audioManager.GetTrackStore(new StorageBackedResourceStore(files));
            localStore = store;
        }

        public byte[] Get(string name) => localStore.Get(name);

        public Task<byte[]> GetAsync(string name, CancellationToken cancellationToken = default) => localStore.GetAsync(name, cancellationToken);

        public Stream GetStream(string name) => localStore.GetStream(name);

        public IEnumerable<string> GetAvailableResources() => localStore.GetAvailableResources();

        public BeatmapInfo[] GetBeatmapInfos()
        {
            List<BeatmapInfo> beatmapInfo = new List<BeatmapInfo>();

            try
            {
                foreach (var dir in Storage.GetDirectories(string.Empty))
                {
                    DirectoryInfo di = new DirectoryInfo(Storage.GetFullPath(dir));

                    foreach (var fi in di.GetFiles("*.circle"))
                    {
                        var beatmap = GetBeatmap(Path.Combine(fi.Directory?.Name, fi.Name));
                        beatmapInfo.Add(new BeatmapInfo(beatmap, fi));
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log($"Error getting Beatmap Infos: {e.Message}");
            }

            return beatmapInfo.ToArray();
        }

        public BeatmapInfo GetBeatmapInfo(Beatmap beatmap)
        {
            foreach (var dir in Storage.GetDirectories(string.Empty))
            {
                DirectoryInfo di = new DirectoryInfo(Storage.GetFullPath(dir));

                foreach (var fi in di.GetFiles("*.circle"))
                {
                    if (beatmap == GetBeatmap(Path.Combine(fi.Directory?.Name, fi.Name)))
                        return new BeatmapInfo(beatmap, fi);
                }
            }

            return null;
        }

        /// <summary>
        /// BeatmapInfo를 반환합니다.
        /// </summary>
        /// <param name="path">상대 경로.</param>
        /// <returns></returns>
        public Beatmap GetBeatmap(string path)
        {
            Beatmap beatmap = default;

            if (string.IsNullOrEmpty(path))
                return null;

            using (StreamReader sr = File.OpenText(Storage.GetFullPath(path)))
            {
                try
                {
                    beatmap = JsonConvert.DeserializeObject<Beatmap>(sr.ReadLine());
                }
                catch
                {
                    Logger.Log($"Failed to parse beatmap. File path: {path}");
                }
            }

            return beatmap;
        }

        public Texture GetBackground(BeatmapInfo info)
        {
            if (!Storage.Exists(info.RelativeBackgroundPath))
                return null;

            try
            {
                return largeTextureStore.Get(info.RelativeBackgroundPath);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to load background.");
                return null;
            }
        }

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

        public Stream GetVideo(BeatmapInfo info)
        {
            if (!Storage.Exists(info.VideoPath))
                return null;

            try
            {
                return File.Open(info.VideoPath, FileMode.Open);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to load video.");
                return null;
            }
        }

        public Track GetTrack(BeatmapInfo info)
        {
            if (!Storage.Exists(info.SongPath))
                return new TrackVirtual(1000);

            try
            {
                return trackStore.Get(info.RelativeSongPath);
            }
            catch
            {
                Logger.Log($"Failed to load beatmap track({info.RelativeSongPath}).");
                return null;
            }
        }

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

        /// <summary>
        /// 비트맵을 파일로 저장합니다.
        /// </summary>
        /// <param name="beatmap">저장할 비트맵.</param>
        public void Create(Beatmap beatmap)
        {
            string json = JsonConvert.SerializeObject(beatmap);

            var fileName = $"[{beatmap.Settings.Author}] {beatmap.Settings.Artist} - {beatmap.Settings.Song}.circle";
            var path = Storage.GetStorageForDirectory(Path.GetFileNameWithoutExtension(fileName)).GetFullPath(string.Empty);

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

        public void Dispose()
        {
        }
    }
}
