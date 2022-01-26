using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace Circle.Game.Beatmap
{
    public class BeatmapStorage
    {
        private readonly Storage storage;

        public BeatmapStorage(Storage storage)
        {
            this.storage = storage?.GetStorageForDirectory("beatmaps");
        }

        public IReadOnlyList<BeatmapInfo> GetBeatmaps()
        {
            List<BeatmapInfo> beatmaps = new List<BeatmapInfo>();

            foreach (var file in storage.GetFiles(string.Empty))
            {
                StreamReader sr = File.OpenText(storage.GetFullPath(string.Empty) + $"/{file}");
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

        public void CreateBeatmap(BeatmapInfo info)
        {
            string json = JsonConvert.SerializeObject(info);

            StreamWriter sw = File.CreateText(storage.GetFullPath(string.Empty) + $"/[{info.Settings.Author}] {info.Settings.Artist} - {info.Settings.Song}.circle");
            sw.WriteLine(json);
            sw.Close();
        }

        public void DeleteBeatmap(string name) => Directory.Delete(storage.GetFullPath(string.Empty) + $"/{name}.circle");
    }
}
