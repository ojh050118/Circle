﻿#nullable disable

using System.Collections.Generic;
using Circle.Game.Beatmaps;
using Circle.Game.Converting.Adofai;
using Circle.Game.Converting.Adofai.Elements;
using osu.Framework.Graphics;
using EventType = Circle.Game.Beatmaps.EventType;
using Settings = Circle.Game.Beatmaps.Settings;

namespace Circle.Game.Converting.Circle
{
    public class CircleBeatmapConverter : BeatmapConverter<AdofaiBeatmap, Beatmap>
    {
        public override Beatmap Convert(AdofaiBeatmap adofai)
        {
            adofai.AngleData ??= ParseAngleData(adofai.PathData);

            Beatmap circle = new Beatmap
            {
                AngleData = adofai.AngleData,
                Settings = new Settings
                {
                    Artist = adofai.Settings.Artist,
                    Author = adofai.Settings.Author,
                    BgImage = adofai.Settings.BgImage,
                    BgVideo = adofai.Settings.BgVideo,
                    BeatmapDesc = adofai.Settings.LevelDesc,
                    CountdownTicks = adofai.Settings.CountdownTicks,
                    Difficulty = adofai.Settings.Difficulty,
                    Bpm = adofai.Settings.Bpm,
                    Offset = adofai.Settings.Offset,
                    VidOffset = adofai.Settings.VidOffset,
                    Pitch = adofai.Settings.Pitch,
                    Volume = adofai.Settings.Volume,
                    PlanetEasing = convertEasing(ParseEase(adofai.Settings.PlanetEase)),
                    PreviewSongStart = adofai.Settings.PreviewSongStart,
                    PreviewSongDuration = adofai.Settings.PreviewSongDuration,
                    SeparateCountdownTime = convertToggle(ParseToggle(adofai.Settings.SeparateCountdownTime)),
                    Song = adofai.Settings.Song,
                    SongFileName = adofai.Settings.SongFilename,
                    RelativeTo = ParseRelativity(adofai.Settings.RelativeTo),
                    Position = adofai.Settings.Position,
                    Rotation = adofai.Settings.Rotation,
                    Zoom = adofai.Settings.Zoom,
                }
            };
            List<Actions> actions = new List<Actions>();

            // 액션 적용.
            foreach (var action in adofai.Actions)
            {
                Actions newAction = new Actions
                {
                    Floor = action.Floor,
                    EventType = convertEventType(action.EventType),
                    RelativeTo = action.RelativeTo,
                    BpmMultiplier = (float)action.BPMMultiplier,
                    BeatsPerMinute = (float)action.BeatsPerMinute,
                    Ease = convertEasing(action.Ease),
                    Duration = action.Duration,
                    Rotation = action.Rotation,
                    AngleOffset = action.AngleOffset,
                    Position = action.Position,
                    Zoom = action.Zoom,
                    Repetitions = action.Repetitions,
                    Interval = action.Interval,
                    Tag = action.Tag,
                    EventTag = action.EventTag,
                };

                if (action.EventType != Adofai.Elements.EventType.MoveCamera)
                    newAction.RelativeTo = null;

                newAction.SpeedType = action.SpeedType;

                if (action.BeatsPerMinute != 0 && newAction.SpeedType == null)
                    newAction.SpeedType = SpeedType.Bpm;
                else if (action.BPMMultiplier != 0 && newAction.SpeedType == null)
                    newAction.SpeedType = SpeedType.Multiplier;

                actions.Add(newAction);
            }

            circle.Actions = actions.ToArray();

            return circle;
        }

        private bool convertToggle(Toggle toggle)
        {
            switch (toggle)
            {
                case Toggle.Disabled:
                    return false;

                case Toggle.Enabled:
                    return true;

                default:
                    return false;
            }
        }

        private Easing convertEasing(Ease ease)
        {
            switch (ease)
            {
                case Ease.Unset:
                    return Easing.None;

                case Ease.OutSine:
                    return Easing.OutSine;

                case Ease.OutQuint:
                    return Easing.OutQuint;

                case Ease.OutQuart:
                    return Easing.OutQuart;

                case Ease.OutQuad:
                    return Easing.OutQuad;

                case Ease.OutExpo:
                    return Easing.OutExpo;

                case Ease.OutElastic:
                    return Easing.OutElastic;

                case Ease.OutCubic:
                    return Easing.OutCubic;

                case Ease.OutCirc:
                    return Easing.OutCirc;

                case Ease.OutBounce:
                    return Easing.OutBounce;

                case Ease.OutBack:
                    return Easing.OutBack;

                // 비슷한 에이징.
                case Ease.OutFlash:
                    return Easing.OutExpo;

                case Ease.Linear:
                    return Easing.None;

                case Ease.InSine:
                    return Easing.InSine;

                case Ease.InQuint:
                    return Easing.InQuint;

                case Ease.InQuart:
                    return Easing.InQuart;

                case Ease.InQuad:
                    return Easing.InQuad;

                case Ease.InOutSine:
                    return Easing.InOutSine;

                case Ease.InOutQuint:
                    return Easing.InOutQuint;

                case Ease.InOutQuart:
                    return Easing.InOutQuart;

                case Ease.InOutQuad:
                    return Easing.InOutQuad;

                case Ease.InOutExpo:
                    return Easing.InOutExpo;

                case Ease.InOutElastic:
                    return Easing.InOutElastic;

                case Ease.InOutCubic:
                    return Easing.InOutCubic;

                case Ease.InOutCirc:
                    return Easing.InOutCirc;

                case Ease.InOutBounce:
                    return Easing.InOutBounce;

                case Ease.InOutBack:
                    return Easing.InOutBack;

                case Ease.InExpo:
                    return Easing.InExpo;

                case Ease.InElastic:
                    return Easing.InElastic;

                case Ease.InCubic:
                    return Easing.InCubic;

                case Ease.InCirc:
                    return Easing.InCirc;

                case Ease.InBounce:
                    return Easing.InBounce;

                case Ease.InBack:
                    return Easing.InBack;

                case Ease.InFlash:
                    return Easing.InExpo;

                default:
                    return Easing.None;
            }
        }

        private EventType convertEventType(Adofai.Elements.EventType eventType)
        {
            switch (eventType)
            {
                case Adofai.Elements.EventType.Twirl:
                    return EventType.Twirl;

                case Adofai.Elements.EventType.SetSpeed:
                    return EventType.SetSpeed;

                case Adofai.Elements.EventType.SetPlanetRotation:
                    return EventType.SetPlanetRotation;

                case Adofai.Elements.EventType.MoveCamera:
                    return EventType.MoveCamera;

                case Adofai.Elements.EventType.RepeatEvents:
                    return EventType.RepeatEvents;

                default:
                    return EventType.Other;
            }
        }
    }
}
