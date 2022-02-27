using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Circle.Game.IO.Archives;
using osu.Framework.Logging;
using SharpCompress.Archives;

namespace Circle.Game.Beatmaps
{
    public class BeatmapManager
    {
        private readonly BeatmapStorage beatmapStorage;

        private BeatmapInfo currentBeatmap;

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

        private List<BeatmapInfo> loadedBeatmaps;

        public IList<BeatmapInfo> LoadedBeatmaps => loadedBeatmaps;

        public event Action<(BeatmapInfo oldBeatmap, BeatmapInfo newBeatmap)> OnBeatmapChanged;

        public event Action<IList<BeatmapInfo>> OnLoadedBeatmaps;

        public event Action<string> OnImported;

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
        public void ReloadBeatmaps()
        {
            Logger.Log("Loading beatmaps...");
            loadedBeatmaps = beatmapStorage.GetBeatmapInfos().ToList();
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

        public void Import(string path, bool migration = false)
        {
            var fileName = Path.GetFileNameWithoutExtension(path).Trim(' ');
            var beatmap = beatmapStorage.Storage.GetStorageForDirectory(fileName).GetFullPath(string.Empty);

            if (Path.GetExtension(path).Equals(@".circle", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    File.Copy(path, Path.Combine(beatmap, Path.GetFileName(path)), true);
                    if (!migration)
                        OnImported?.Invoke(Path.GetFileName(path));
                }
                catch (Exception e)
                {
                    Logger.Log($"Error during import {Path.GetFileName(path)}: {e.Message}");
                }
                finally
                {
                    if (migration)
                        File.Delete(path);
                }

                return;
            }

            using (var reader = new ZipArchiveReader(File.Open(path, FileMode.Open, FileAccess.Read), Path.GetFileName(path)))
            {
                reader.Archive.WriteToDirectory(beatmap);
                if (migration)
                    File.Delete(path);
                else
                    OnImported?.Invoke(Path.GetFileName(path));
            }
        }

        public void Migrate()
        {
            foreach (var bi in beatmapStorage.GetBeatmapInfos())
            {
                if (bi.Directory != @"beatmaps")
                    continue;

                Logger.Log($"Migrating {bi}...");
                migrateTracks(bi);
                migrateBackgrounds(bi);
                Import(bi.BeatmapPath, true);
                Logger.Log($"Migrated {bi}!");
            }

            deleteOldStorages();
            ReloadBeatmaps();
        }

        private void migrateTracks(BeatmapInfo info)
        {
            var songFileName = info.Beatmap.Settings.SongFileName;
            var tracks = Path.Combine(beatmapStorage.Storage.GetFullPath("tracks"), songFileName);

            if (string.IsNullOrEmpty(songFileName) || !beatmapStorage.Storage.Exists(tracks))
                return;

            var fileName = Path.GetFileNameWithoutExtension(info.Name).Trim(' ');
            var beatmap = beatmapStorage.Storage.GetStorageForDirectory(fileName).GetFullPath(string.Empty);

            try
            {
                File.Copy(tracks, Path.Combine(beatmap, songFileName));
            }
            catch (Exception e)
            {
                Logger.Log($"Error during migration track {songFileName}: {e.Message}");
            }
        }

        private void migrateBackgrounds(BeatmapInfo info)
        {
            var bgImage = info.Beatmap.Settings.BgImage;
            var backgrounds = Path.Combine(beatmapStorage.Storage.GetFullPath("backgrounds"), bgImage);

            if (string.IsNullOrEmpty(bgImage) || !beatmapStorage.Storage.Exists(backgrounds))
                return;

            var fileName = Path.GetFileNameWithoutExtension(info.Name).Trim(' ');
            var beatmap = beatmapStorage.Storage.GetStorageForDirectory(fileName).GetFullPath(string.Empty);

            try
            {
                File.Copy(backgrounds, Path.Combine(beatmap, bgImage));
            }
            catch (Exception e)
            {
                Logger.Log($"Error during migration background {bgImage}: {e.Message}");
            }
        }

        private void deleteOldStorages()
        {
            beatmapStorage.Storage.DeleteDirectory(@"backgrounds");
            beatmapStorage.Storage.DeleteDirectory(@"beatmaps");
            beatmapStorage.Storage.DeleteDirectory(@"tracks");
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
    }
}
