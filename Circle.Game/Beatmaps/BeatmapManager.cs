#nullable disable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Circle.Game.IO.Archives;
using osu.Framework.Logging;
using SharpCompress.Archives;

namespace Circle.Game.Beatmaps
{
    public class BeatmapManager
    {
        public IList<BeatmapInfo> LoadedBeatmaps => loadedBeatmaps;

        public BeatmapInfo CurrentBeatmap
        {
            get => currentBeatmap;
            set
            {
                var oldBeatmap = currentBeatmap;
                currentBeatmap = value;

                if (!BeatmapUtils.Compare(oldBeatmap, currentBeatmap))
                    OnBeatmapChanged?.Invoke((oldBeatmap, currentBeatmap));
            }
        }

        private readonly BeatmapStorage beatmapStorage;

        private BeatmapInfo currentBeatmap;

        private List<BeatmapInfo> loadedBeatmaps;

        public BeatmapManager(BeatmapStorage beatmaps)
        {
            beatmapStorage = beatmaps;
            OnBeatmapChanged += beatmap =>
            {
                Logger.Log($"Beatmap changed: {beatmap.oldBeatmap} to {beatmap.newBeatmap}.");
            };
        }

        /// <summary>
        /// 폴더에 존재하는 비트맵을 로드합니다.
        /// </summary>
        public void LoadBeatmaps()
        {
            Logger.Log("Loading beatmaps...");
            loadedBeatmaps = beatmapStorage.GetBeatmapInfos().ToList();
            OnLoadedBeatmaps?.Invoke(loadedBeatmaps);

            if (!loadedBeatmaps.Exists(b => b.Equals(CurrentBeatmap)))
                ClearCurrentBeatmap();

            Logger.Log($"Loaded {loadedBeatmaps.Count} beatmaps!");
        }

        /// <summary>
        /// 폴더에 존재하는 비트맵을 로드합니다.
        /// </summary>
        public async Task LoadBeatmapsAsync()
        {
            Logger.Log("Loading beatmaps...");
            loadedBeatmaps = await beatmapStorage.GetBeatmapInfosAsync();
            OnLoadedBeatmaps?.Invoke(loadedBeatmaps);

            if (!loadedBeatmaps.Exists(b => b.Equals(CurrentBeatmap)))
                ClearCurrentBeatmap();

            Logger.Log($"Loaded {loadedBeatmaps.Count} beatmaps!");
        }

        /// <summary>
        /// 현재 비트맵을 비웁니다.
        /// </summary>
        public void ClearCurrentBeatmap()
        {
            if (currentBeatmap == null)
                return;

            CurrentBeatmap = null;
        }

        public void Import(string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(path).Trim(' ');
            string beatmap = beatmapStorage.Storage.GetStorageForDirectory(fileName).GetFullPath(string.Empty);

            if (Path.GetExtension(path).Equals(@".circle", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    File.Copy(path, Path.Combine(beatmap, Path.GetFileName(path)), true);
                    OnImported?.Invoke(Path.GetFileName(path));
                }
                catch (Exception e)
                {
                    Logger.Log($"Error during import {Path.GetFileName(path)}: {e.Message}");
                }

                return;
            }

            using (var reader = new ZipArchiveReader(File.Open(path, FileMode.Open, FileAccess.Read), Path.GetFileName(path)))
            {
                reader.Archive.WriteToDirectory(beatmap);
                OnImported?.Invoke(Path.GetFileName(path));
            }
        }

        public void Import(Stream stream, string name)
        {
            if (stream == null)
            {
                Logger.Log($"Error when importing local beatmap: {name}");
                return;
            }

            string beatmap = beatmapStorage.Storage.GetStorageForDirectory(name).GetFullPath(string.Empty);

            using (var reader = new ZipArchiveReader(stream))
            {
                reader.Archive.WriteToDirectory(beatmap);

                OnImported?.Invoke(name);
            }
        }

        /// <summary>
        /// 비트맵을 파일로 저장합니다.
        /// </summary>
        /// <param name="beatmap">저장할 비트맵.</param>
        public void Save(Beatmap beatmap) => beatmapStorage.Create(beatmap);

        /// <summary>
        /// 비트맵 파일을 삭제합니다. 삭제할 비트맵이 현재 비트맵일 때 현재 비트맵을 비웁니다.
        /// </summary>
        /// <param name="beatmap">삭제할 비트맵.</param>
        /// <param name="deleteResources">비트맵에 사용된 리소스 삭제 여부.</param>
        /// <returns></returns>
        public bool Delete(BeatmapInfo beatmap, bool deleteResources)
        {
            if (currentBeatmap.Equals(beatmap))
                ClearCurrentBeatmap();

            beatmapStorage.Delete(beatmap, deleteResources);

            return LoadedBeatmaps.Remove(beatmap);
        }

        public event Action<(BeatmapInfo oldBeatmap, BeatmapInfo newBeatmap)> OnBeatmapChanged;

        public event Action<IList<BeatmapInfo>> OnLoadedBeatmaps;

        public event Action<string> OnImported;
    }
}
