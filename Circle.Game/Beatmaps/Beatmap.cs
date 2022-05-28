using System;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Utils;

namespace Circle.Game.Beatmaps
{
    public class Beatmap : IEquatable<Beatmap>
    {
        public float[] AngleData { get; set; }
        public Settings Settings { get; set; }
        public Actions[] Actions { get; set; }

        public bool Equals(Beatmap beatmap) => beatmap != null && Settings.Equals(beatmap.Settings) && Actions.SequenceEqual(beatmap.Actions);
    }

    public struct Settings : IEquatable<Settings>
    {
        public string Artist { get; set; }
        public string Song { get; set; }
        public string SongFileName { get; set; }
        public string Author { get; set; }
        public bool SeparateCountdownTime { get; set; }
        public int PreviewSongStart { get; set; }
        public int PreviewSongDuration { get; set; }
        public string BeatmapDesc { get; set; }
        public int Difficulty { get; set; }
        public float Bpm { get; set; }
        public int Volume { get; set; }
        public int Offset { get; set; }
        public float VidOffset { get; set; }
        public int Pitch { get; set; }
        public int CountdownTicks { get; set; }
        public string BgImage { get; set; }
        public string BgVideo { get; set; }
        public Relativity RelativeTo { get; set; }
        public float[] Position { get; set; }
        public float Rotation { get; set; }
        public float Zoom { get; set; }
        public Easing PlanetEasing { get; set; }

        public bool Equals(Settings settings) => Artist == settings.Artist &&
                                                 Song == settings.Song &&
                                                 SongFileName == settings.SongFileName &&
                                                 Author == settings.Author &&
                                                 SeparateCountdownTime == settings.SeparateCountdownTime &&
                                                 PreviewSongStart == settings.PreviewSongStart &&
                                                 PreviewSongDuration == settings.PreviewSongDuration &&
                                                 BeatmapDesc == settings.BeatmapDesc &&
                                                 Difficulty == settings.Difficulty &&
                                                 Precision.AlmostEquals(Bpm, settings.Bpm) &&
                                                 Volume == settings.Volume &&
                                                 Offset == settings.Offset &&
                                                 Precision.AlmostEquals(VidOffset, settings.VidOffset) &&
                                                 Pitch == settings.Pitch &&
                                                 CountdownTicks == settings.CountdownTicks &&
                                                 BgImage == settings.BgImage &&
                                                 BgVideo == settings.BgVideo &&
                                                 RelativeTo == settings.RelativeTo &&
                                                 Precision.AlmostEquals(Rotation, settings.Rotation) &&
                                                 PlanetEasing == settings.PlanetEasing;
    }

    public struct Actions : IEquatable<Actions>
    {
        public int Floor { get; set; }
        public EventType EventType { get; set; }
        public SpeedType? SpeedType { get; set; }
        public float BeatsPerMinute { get; set; }
        public float BpmMultiplier { get; set; }
        public Relativity? RelativeTo { get; set; }
        public Easing Ease { get; set; }
        public double Duration { get; set; }
        public float? Rotation { get; set; }
        public float AngleOffset { get; set; }
        public float[]? Position { get; set; }
        public int? Zoom { get; set; }
        public int Repetitions { get; set; }
        public double Interval { get; set; }
        public string Tag { get; set; }
        public string EventTag { get; set; }

        public override string ToString()
        {
            return $"Floor: {Floor} | Event type: {EventType}";
        }

        public bool Equals(Actions actions) => Floor == actions.Floor &&
                                               EventType == actions.EventType &&
                                               SpeedType == actions.SpeedType &&
                                               Precision.AlmostEquals(BeatsPerMinute, actions.BeatsPerMinute) &&
                                               Precision.AlmostEquals(BpmMultiplier, actions.BpmMultiplier) &&
                                               RelativeTo == actions.RelativeTo &&
                                               Ease == actions.Ease &&
                                               Duration == actions.Duration &&
                                               Rotation == actions.Rotation &&
                                               AngleOffset == actions.AngleOffset &&
                                               Zoom == actions.Zoom &&
                                               Repetitions == actions.Repetitions &&
                                               Precision.AlmostEquals(Interval, actions.Interval) &&
                                               Tag == actions.Tag;
    }

    public enum EventType
    {
        Twirl,
        SetSpeed,
        MoveCamera,
        SetPlanetRotation,
        RepeatEvents,
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
