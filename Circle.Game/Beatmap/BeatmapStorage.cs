using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace Circle.Game.Beatmap
{
    public class BeatmapStorage
    {
        private readonly Storage beatmapStorage;
        private readonly BeatmapResourcesManager resources;

        public BeatmapStorage(Storage files, BeatmapResourcesManager resourcesManager)
        {
            beatmapStorage = files?.GetStorageForDirectory("beatmaps");
            resources = resourcesManager;
        }

        /// <summary>
        /// 폴더에 존재하는 비트맵의 집합을 반환합니다.
        /// </summary>
        /// <returns>폴더에 존재하는 비트맵의 집합.</returns>
        public IReadOnlyList<BeatmapInfo> GetBeatmaps()
        {
            List<BeatmapInfo> beatmaps = new List<BeatmapInfo>();

            foreach (var file in beatmapStorage.GetFiles(string.Empty))
            {
                StreamReader sr = File.OpenText(Path.Combine(beatmapStorage.GetFullPath(string.Empty), $"{file}"));
                var text = sr.ReadLine();
                sr.Close();

                // 파싱에 실패하는 비트맵이 존재할 수 있음.
                try
                {
                    var beatmap = JsonConvert.DeserializeObject<BeatmapInfo>(text);
                    beatmaps.Add(beatmap);
                }
                catch
                {
                    Logger.Log($"Failed to load beatmap({file}).");
                }
            }

            return beatmaps;
        }

        /// <summary>
        /// 비트맵을 파일로 저장합니다.
        /// </summary>
        /// <param name="beatmap">저장할 비트맵.</param>
        public void CreateBeatmap(BeatmapInfo beatmap)
        {
            string json = JsonConvert.SerializeObject(beatmap);

            StreamWriter sw = File.CreateText(Path.Combine(beatmapStorage.GetFullPath(string.Empty), $"[{beatmap.Settings.Author}] {beatmap.Settings.Artist} - {beatmap.Settings.Song}.circle"));
            sw.WriteLine(json);
            sw.Close();
        }

        /// <summary>
        /// 비트맵 파일을 삭제합니다.
        /// </summary>
        /// <param name="beatmap">삭제할 비트맵.</param>
        /// <param name="deleteResources">비트맵에 사용된 리소스 삭제 여부.</param>
        public void DeleteBeatmap(BeatmapInfo beatmap, bool deleteResources)
        {
            File.Delete(Path.Combine(beatmapStorage.GetFullPath(string.Empty), $"[{beatmap.Settings.Author}] {beatmap.Settings.Artist} - {beatmap.Settings.Song}.circle"));

            if (deleteResources)
            {
                File.Delete(Path.Combine(resources.Backgrounds.GetFullPath(string.Empty), beatmap.Settings.BgImage));
                File.Delete(Path.Combine(resources.Backgrounds.GetFullPath(string.Empty), beatmap.Settings.SongFileName));
            }
        }
    }
}
