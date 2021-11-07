using System.Collections.Generic;
using System.IO;
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
        public int Floor;
        public Event Event;
    }

    public struct Event
    {
        public EventType EventType;
        public double ToBPM;
        public bool IsRelativeTile;
    }

    public enum EventType
    {
        Twirl,
        SetSpeed,
        MoveCamera
    }
}
