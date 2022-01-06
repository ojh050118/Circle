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
        public float[] Angles;
        public Settings Settings;
        public Actions[] Actions;

        public bool Equals(BeatmapInfo info) => Angles?.Length == info.Angles?.Length && Settings.Equals(info.Settings) && Actions == info.Actions;
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
        public float BPM;
        public double Offset;
        public double Pitch;
        public string BackgroundTexture;
        public Easing PlanetEasing;

        public bool Equals(Settings settings) => Artist == settings.Artist &&
                                                 Track == settings.Track &&
                                                 Author == settings.Author &&
                                                 SeparateCountdownTime == settings.SeparateCountdownTime &&
                                                 PreviewTrackStart == settings.PreviewTrackStart &&
                                                 BeatmapDesc == settings.BeatmapDesc &&
                                                 BeatmapDifficulty == settings.BeatmapDifficulty &&
                                                 (int)BPM == (int)settings.BPM &&
                                                 (int)Offset == (int)settings.Offset &&
                                                 (int)Pitch == (int)settings.Pitch &&
                                                 BackgroundTexture == settings.BackgroundTexture &&
                                                 PlanetEasing == settings.PlanetEasing;
    }

    public struct Actions
    {
        public float Floor;
        public EventType EventType;
        public SpeedType? SpeedType;
        public float BeatsPerMinute;
        public float BpmMultiplier;
        public Relativity? Relativity;
    }

    public enum EventType
    {
        Twirl,
        SetSpeed,
        MoveCamera,
        Other
    }

    public enum SpeedType
    {
        Multiplier,
        Bpm
    }

    public enum Relativity
    {
        Tile,
        LastPosition,
        Player,
        Global,
        RedPlanet,
        BluePlanet
    }
}
