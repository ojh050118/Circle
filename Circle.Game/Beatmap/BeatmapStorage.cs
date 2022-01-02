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

        public static bool operator ==(BeatmapInfo info1, BeatmapInfo info2) => info1.Angles.Length == info2.Angles.Length && info1.Settings == info2.Settings && info1.Actions == info2.Actions;

        public static bool operator !=(BeatmapInfo info1, BeatmapInfo info2) => info1.Angles.Length != info2.Angles.Length || info1.Settings != info2.Settings || info1.Actions != info2.Actions;

        public bool Equals(BeatmapInfo other) => Equals(Angles, other.Angles) && Settings.Equals(other.Settings) && Equals(Actions, other.Actions);

        public override bool Equals(object obj) => obj is BeatmapInfo other && Equals(other);
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

        public static bool operator ==(Settings s1, Settings s2) => s1.Artist == s2.Artist &&
                                                                    s1.Track == s2.Track &&
                                                                    s1.Author == s2.Author &&
                                                                    s1.SeparateCountdownTime == s2.SeparateCountdownTime &&
                                                                    s1.PreviewTrackStart == s2.PreviewTrackStart &&
                                                                    s1.BeatmapDesc == s2.BeatmapDesc &&
                                                                    s1.BeatmapDifficulty == s2.BeatmapDifficulty &&
                                                                    s1.BPM == s2.BPM &&
                                                                    s1.BackgroundTexture == s2.BackgroundTexture &&
                                                                    s1.PlanetEasing == s2.PlanetEasing;

        public static bool operator !=(Settings s1, Settings s2) => s1.Artist != s2.Artist ||
                                                                    s1.Track != s2.Track ||
                                                                    s1.Author != s2.Author ||
                                                                    s1.SeparateCountdownTime != s2.SeparateCountdownTime ||
                                                                    s1.PreviewTrackStart != s2.PreviewTrackStart ||
                                                                    s1.BeatmapDesc != s2.BeatmapDesc ||
                                                                    s1.BeatmapDifficulty != s2.BeatmapDifficulty ||
                                                                    s1.BPM != s2.BPM ||
                                                                    s1.BackgroundTexture != s2.BackgroundTexture ||
                                                                    s1.PlanetEasing != s2.PlanetEasing;

        public bool Equals(Settings other) => Artist == other.Artist &&
                                              Track == other.Track &&
                                              Author == other.Author &&
                                              SeparateCountdownTime == other.SeparateCountdownTime &&
                                              PreviewTrackStart == other.PreviewTrackStart &&
                                              BeatmapDesc == other.BeatmapDesc &&
                                              BeatmapDifficulty == other.BeatmapDifficulty &&
                                              BPM.Equals(other.BPM) && Offset.Equals(other.Offset) &&
                                              Pitch.Equals(other.Pitch) &&
                                              BackgroundTexture == other.BackgroundTexture &&
                                              PlanetEasing == other.PlanetEasing;

        public override bool Equals(object obj) => obj is Settings other && Equals(other);
    }

    public struct Actions
    {
        public float Floor;
        public Event Event;
    }

    public struct Event
    {
        public EventType EventType;
        public SpeedType? SpeedType;
        public float? BeatsPerMinute;
        public float? BpmMultiplier;
        public RelativeTo? RelativeTo;
    }

    public enum EventType
    {
        Twirl,
        SetSpeed,
        MoveCamera
    }

    public enum SpeedType
    {
        Multiplier,
        Bpm
    }

    public enum RelativeTo
    {
        Tile,
        Player,
        Global,
        LastPosition
    }
}
