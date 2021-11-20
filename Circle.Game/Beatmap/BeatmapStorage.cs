using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using osu.Framework.Graphics;
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

                var beatmap = JsonConvert.DeserializeObject<BeatmapInfo>(text);
                beatmaps.Add(beatmap);
            }

            return beatmaps;
        }

        public void CreateBeatmap(BeatmapInfo info)
        {
            string json = JsonConvert.SerializeObject(info);

            StreamWriter sw = File.CreateText(storage.GetFullPath(string.Empty) + $"/{info.Settings.Author} - {info.Settings.Track}");
            sw.WriteLine(json);
            sw.Close();
        }

        public void DeleteBeatmap(string name) => Directory.Delete(storage.GetFullPath(string.Empty) + $"/{name}");
    }

    public struct BeatmapInfo
    {
        public List<int> Angles;
        public Settings Settings;
        public List<Actions> Actions;
    }

    public struct Settings
    {
        public string Artist;
        public string Track;
        public string Author;
        public bool SeparateCountdownTime;
        public int PreviewTrackStart;
        public string BeatmapDesc;
        public int BeatmapDifficulty;
        public double BPM;
        public double Offset;
        public double Pitch;
        public string BackgroundTexture;
        public Easing PlanetEasing;
    }

    public struct Actions
    {
        public float Floor;
        public Event Event;
    }

    public struct Event
    {
        public EventType EventType;
        public double ToBPM;
        public bool IsRelativeTile;
        public bool IsTwirl;
    }

    public enum EventType
    {
        Twirl,
        SetSpeed,
        MoveCamera
    }
}
