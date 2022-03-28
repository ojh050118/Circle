using System;
using System.Collections.Generic;
using System.Linq;
using Circle.Game.Beatmaps;
using Circle.Game.Configuration;
using Circle.Game.Rulesets.Extensions;
using Circle.Game.Rulesets.Objects;
using Circle.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;

namespace Circle.Game.Screens.Play
{
    public class Playfield : Container
    {
        private ObjectContainer tileContainer;
        private Planet redPlanet;
        private Planet bluePlanet;
        private Container<Planet> planetContainer;
        private Container cameraContainer;

        private TileInfo[] tilesInfo;

        public Action OnComplete { get; set; }

        private readonly Beatmap currentBeatmap;

        public Playfield(Beatmap beatmap)
        {
            currentBeatmap = beatmap;
        }

        [BackgroundDependencyLoader]
        private void load(CircleConfigManager config)
        {
            RelativeSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Child = cameraContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Child = new Container
                {
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Children = new Drawable[]
                    {
                        tileContainer = new ObjectContainer(currentBeatmap),
                        planetContainer = new Container<Planet>
                        {
                            AutoSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Children = new[]
                            {
                                redPlanet = new Planet(Color4Utils.GetColor4(config.Get<Color4Enum>(CircleSetting.PlanetRed)))
                                {
                                    PlanetColour = config.GetBindable<Color4Enum>(CircleSetting.PlanetRed)
                                },
                                bluePlanet = new Planet(Color4Utils.GetColor4(config.Get<Color4Enum>(CircleSetting.PlanetBlue)))
                                {
                                    PlanetColour = config.GetBindable<Color4Enum>(CircleSetting.PlanetBlue)
                                },
                            }
                        }
                    }
                }
            };

            tilesInfo = tileContainer.GetTilesInfo().ToArray();
            redPlanet.Expansion = bluePlanet.Expansion = 0;
            bluePlanet.Rotation = tilesInfo.First().Angle - 180;
        }

        protected override void LoadComplete()
        {
            addTileTransforms(currentBeatmap.Settings.Offset - 60000 / currentBeatmap.Settings.Bpm);
            addTransforms(currentBeatmap.Settings.Offset - 60000 / currentBeatmap.Settings.Bpm);
            addCameraTransforms(currentBeatmap.Settings.Offset - 60000 / currentBeatmap.Settings.Bpm);

            base.LoadComplete();
        }

        private void addTransforms(double gameplayStartTime)
        {
            // 회전하고 있는 행성.
            PlanetState planetState = PlanetState.Ice;

            double startTimeOffset = gameplayStartTime;
            float prevAngle;
            float bpm = currentBeatmap.Settings.Bpm;
            int floor = 0;
            Easing easing = Easing.None;

            #region Initial planet rotation

            using (bluePlanet.BeginAbsoluteSequence(startTimeOffset))
            {
                startTimeOffset += CalculationExtensions.GetRelativeDuration(bluePlanet.Rotation, tilesInfo[floor].Angle, bpm);
                bluePlanet.ExpandTo(1, 60000 / bpm, Easing.Out);
                bluePlanet.RotateTo(tilesInfo[floor].Angle, CalculationExtensions.GetRelativeDuration(bluePlanet.Rotation, tilesInfo[floor].Angle, bpm), easing);
            }

            using (bluePlanet.BeginAbsoluteSequence(startTimeOffset))
            {
                prevAngle = tilesInfo[floor].Angle;
                floor++;

                if (floor < tilesInfo.Length)
                {
                    bluePlanet.ExpandTo(0);
                    using (planetContainer.BeginAbsoluteSequence(startTimeOffset))
                        planetContainer.MoveTo(tilesInfo[floor].Position);

                    if (tilesInfo[floor].TileType != TileType.Midspin)
                        planetState = PlanetState.Fire;
                }
            }

            #endregion

            while (floor < tilesInfo.Length)
            {
                var fixedRotation = computeRotation(floor, prevAngle);
                bpm = getNewBpm(bpm, floor);
                prevAngle = tilesInfo[floor].Angle;

                // Apply easing
                var easingAction = tilesInfo[floor].Action.FirstOrDefault(action => action.EventType == EventType.SetPlanetRotation);
                easing = easingAction.Ease;

                #region Planet rotation

                switch (planetState)
                {
                    case PlanetState.Fire:
                        using (redPlanet.BeginAbsoluteSequence(startTimeOffset, false))
                        {
                            redPlanet.ExpandTo(1);
                            redPlanet.RotateTo(fixedRotation);
                            redPlanet.RotateTo(tilesInfo[floor].Angle, getRelativeDuration(fixedRotation, floor, bpm), easing);
                        }

                        break;

                    case PlanetState.Ice:
                        using (bluePlanet.BeginAbsoluteSequence(startTimeOffset, false))
                        {
                            bluePlanet.ExpandTo(1);
                            bluePlanet.RotateTo(fixedRotation);
                            bluePlanet.RotateTo(tilesInfo[floor].Angle, getRelativeDuration(fixedRotation, floor, bpm), easing);
                        }

                        break;
                }

                #endregion

                // 회전을 마치면 다른 행성으로 회전할 준비를 해야합니다.
                startTimeOffset += getRelativeDuration(fixedRotation, floor, bpm);
                floor++;

                #region Planet reducation

                switch (planetState)
                {
                    case PlanetState.Fire:
                        using (redPlanet.BeginAbsoluteSequence(startTimeOffset, false))
                            redPlanet.ExpandTo(0);

                        break;

                    case PlanetState.Ice:
                        using (bluePlanet.BeginAbsoluteSequence(startTimeOffset, false))
                            bluePlanet.ExpandTo(0);

                        break;
                }

                #endregion

                #region Move PlanetContainer

                if (floor < tilesInfo.Length)
                {
                    using (planetContainer.BeginAbsoluteSequence(startTimeOffset, false))
                        planetContainer.MoveTo(tilesInfo[floor].Position);

                    if (tilesInfo[floor].TileType != TileType.Midspin && tilesInfo[floor - 1].TileType != TileType.Midspin)
                        planetState = planetState == PlanetState.Fire ? PlanetState.Ice : PlanetState.Fire;
                    else if (tilesInfo[floor].TileType == TileType.Midspin && tilesInfo[floor - 1].TileType == TileType.Midspin)
                        planetState = planetState == PlanetState.Fire ? PlanetState.Ice : PlanetState.Fire;
                }
                else
                {
                    // 마지막 타일에 도달했음을 의미합니다.
                    switch (planetState)
                    {
                        case PlanetState.Fire:
                            using (redPlanet.BeginAbsoluteSequence(startTimeOffset, false))
                            {
                                redPlanet.ExpandTo(1);
                                redPlanet.Spin(60000 / bpm * 2, getIsClockwise(floor), prevAngle);
                            }

                            break;

                        case PlanetState.Ice:
                            using (bluePlanet.BeginAbsoluteSequence(startTimeOffset, false))
                            {
                                bluePlanet.ExpandTo(1);
                                bluePlanet.Spin(60000 / bpm * 2, getIsClockwise(floor), prevAngle);
                            }

                            break;
                    }
                }

                #endregion
            }
        }

        private void addCameraTransforms(double gameplayStartTime)
        {
            float bpm = currentBeatmap.Settings.Bpm;
            var offset = CalculationExtensions.GetTileHitTime(currentBeatmap, gameplayStartTime);

            for (int floor = 0; floor < tilesInfo.Length; floor++)
            {
                bpm = getNewBpm(bpm, floor);
                var prevAngle = tilesInfo[floor].Angle;
                List<Actions> pendingActions = new List<Actions>();

                // Camera
                using (cameraContainer.Child.BeginAbsoluteSequence(offset[floor], false))
                    cameraContainer.Child.MoveTo(-tilesInfo[floor].Position, 400 + 60 / bpm * 500, Easing.OutSine);

                // 한 타일의 액션에 접근
                for (int actionIndex = 0; actionIndex < tilesInfo[floor].Action.Length; actionIndex++)
                {
                    var action = tilesInfo[floor].Action[actionIndex];

                    switch (action.EventType)
                    {
                        case EventType.MoveCamera:
                            addCameraEvents(action, prevAngle, bpm, offset[floor]);
                            break;

                        // 이벤트 반복은 원래 이벤트를 포함해 반복하지 않습니다. (ex: 반복횟수가 1이면 이벤트는 총 2번 실행됨)
                        // 이벤트반복 이벤트가 앞이나 중간에 있으면 이벤트 반복을 안하는 것으로 보여 이벤트를 나중에 처리합니다.
                        case EventType.RepeatEvents:
                            pendingActions.Add(action);
                            break;
                    }
                }

                addRepeatedEvents(bpm, offset[floor], pendingActions);
            }
        }

        private void addRepeatedEvents(float bpm, double offset, List<Actions> actionQueue)
        {
            // 원소가 바뀔 때 Trnasforms가 덮어씌워짐
            foreach (var action in actionQueue)
            {
                var prevAngle = tilesInfo[action.Floor].Angle;
                var intervalBeat = 60000 / bpm * action.Interval;
                var events = Array.FindAll(tilesInfo[action.Floor].Action, a => a.EventType == EventType.MoveCamera);

                for (int i = 1; i <= action.Repetitions; i++)
                {
                    var startOffset = offset + intervalBeat * i;
                    foreach (var cameraAction in events)
                    {
                        if (cameraAction.EventTag == action.Tag)
                            addCameraEvents(cameraAction, prevAngle, bpm, startOffset);
                    }
                }
            }
        }

        private void addCameraEvents(Actions action, float prevAngle, float bpm, double transformStartOffset)
        {
            int floor = action.Floor;
            var fixedRotation = computeRotation(floor, prevAngle);
            float cameraRotation = action.Rotation ?? 0;
            float cameraZoom = 1 / ((float)(action.Zoom ?? 100) / 100);
            var angleTimeOffset = CalculationExtensions.GetRelativeDuration(fixedRotation, fixedRotation + action.AngleOffset, bpm);

            if (tilesInfo[floor].TileType == TileType.Midspin)
                floor++;

            if (float.IsInfinity(cameraZoom))
                cameraZoom = 0;

            double duration = getRelativeDuration(fixedRotation, floor, bpm) * action.Duration;

            using (cameraContainer.BeginAbsoluteSequence(transformStartOffset + angleTimeOffset))
            {
                cameraContainer.ScaleTo(cameraZoom, duration, action.Ease);
                cameraContainer.RotateTo(cameraRotation, duration, action.Ease);
            }

            Logger.Log($"Transform is added. Count: {cameraContainer.Transforms.Count()}");
            Logger.Log($"Floor: {floor}, {transformStartOffset + angleTimeOffset}-{transformStartOffset + angleTimeOffset + duration}");
        }

        private void addTileTransforms(double gameplayStartTime)
        {
            float bpm = currentBeatmap.Settings.Bpm;
            var tilesOffset = CalculationExtensions.GetTileHitTime(currentBeatmap, gameplayStartTime);

            for (int i = 8; i < tilesInfo.Length; i++)
                tileContainer.Children[i].Alpha = 0;

            // Fade in
            for (int i = 8; i < tilesInfo.Length; i++)
            {
                bpm = getNewBpm(bpm, i - 8);

                using (tileContainer.Children[i].BeginAbsoluteSequence(tilesOffset[i - 8], false))
                    tileContainer.Children[i].FadeTo(0.45f, 60000 / bpm, Easing.Out);
            }

            bpm = currentBeatmap.Settings.Bpm;

            // Fade out
            for (int i = 0; i < tilesInfo.Length; i++)
            {
                bpm = getNewBpm(bpm, i);

                if (i > 3)
                {
                    using (tileContainer.Children[i - 4].BeginAbsoluteSequence(tilesOffset[i], false))
                        tileContainer.Children[i - 4].FadeOut(60000 / bpm, Easing.Out).Then().Expire();
                }
            }
        }

        private float computeRotation(int floor, float prevAngle) => CalculationExtensions.ComputeRotation(tilesInfo, floor, prevAngle);

        private RotationDirection getIsClockwise(int floor) => CalculationExtensions.GetIsClockwise(tilesInfo, floor) ? RotationDirection.Clockwise : RotationDirection.Counterclockwise;

        private float getRelativeDuration(float oldRotation, int floor, float bpm) => CalculationExtensions.GetRelativeDuration(oldRotation, tilesInfo[floor].Angle, bpm);

        private float getNewBpm(float current, int floor) => CalculationExtensions.GetNewBpm(tilesInfo, current, floor);
    }
}
