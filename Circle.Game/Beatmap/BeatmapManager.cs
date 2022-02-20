using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Logging;

namespace Circle.Game.Beatmap
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
                OnBeatmapChanged?.Invoke((currentBeatmap, value));
                currentBeatmap = value;
            }
        }

        private List<BeatmapInfo> loadedBeatmaps;

        public IList<BeatmapInfo> LoadedBeatmaps => loadedBeatmaps;

        public event Action<(BeatmapInfo oldBeatmap, BeatmapInfo newBeatmap)> OnBeatmapChanged;

        public event Action<IList<BeatmapInfo>> OnLoadedBeatmaps;

        public BeatmapManager(BeatmapStorage beatmaps)
        {
            beatmapStorage = beatmaps;
            OnBeatmapChanged += beatmap =>
            {
                var oldBeatmapName = $"[{beatmap.oldBeatmap.Settings.Author}] {beatmap.oldBeatmap.Settings.Artist} - {beatmap.oldBeatmap.Settings.Song}";
                var newBeatmapName = $"[{beatmap.newBeatmap.Settings.Author}] {beatmap.newBeatmap.Settings.Artist} - {beatmap.newBeatmap.Settings.Song}";
                Logger.Log($"Beatmap changed: {oldBeatmapName} → {newBeatmapName}.");
            };
        }

        /// <summary>
        /// 폴더에 존재하는 비트맵을 로드합니다.
        /// </summary>
        public void ReloadBeatmaps()
        {
            loadedBeatmaps = beatmapStorage.GetBeatmaps().ToList();
            OnLoadedBeatmaps?.Invoke(loadedBeatmaps);
            if (!loadedBeatmaps.Exists(b => b.Equals(currentBeatmap)))
                ClearCurrentBeatmap();
        }

        /// <summary>
        /// 현재 비트맵을 비웁니다.
        /// </summary>
        public void ClearCurrentBeatmap()
        {
            var oldBeatmap = CurrentBeatmap;
            CurrentBeatmap = new BeatmapInfo();
            OnBeatmapChanged?.Invoke((oldBeatmap, CurrentBeatmap));
        }

        /// <summary>
        /// 비트맵을 파일로 저장합니다.
        /// </summary>
        /// <param name="beatmap">저장할 비트맵.</param>
        public void SaveBeatmap(BeatmapInfo beatmap) => beatmapStorage.CreateBeatmap(beatmap);

        /// <summary>
        /// 비트맵 파일을 삭제합니다. 삭제할 비트맵이 현재 비트맵일 때 현재 비트맵을 비웁니다.
        /// </summary>
        /// <param name="beatmap">삭제할 비트맵.</param>
        /// <param name="deleteResources">비트맵에 사용된 리소스 삭제 여부.</param>
        /// <returns></returns>
        public bool DeleteBeatmap(BeatmapInfo beatmap, bool deleteResources)
        {
            beatmapStorage.DeleteBeatmap(beatmap, deleteResources);

            if (beatmap.Equals(currentBeatmap))
                ClearCurrentBeatmap();

            return LoadedBeatmaps.Remove(beatmap);
        }
    }
}
