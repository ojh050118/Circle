using System;
using System.Linq;
using Circle.Game.Beatmaps;
using Circle.Game.Configuration;
using Circle.Game.Rulesets.Extensions;
using Circle.Game.Rulesets.Objects;
using Circle.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;

namespace Circle.Game.Screens.Play
{
    public class Playfield : Container
    {
        private ObjectContainer tileContainer;
        private Planet redPlanet;
        private Planet bluePlanet;
        private Container<Planet> planetContainer;

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
                var (fixedRotation, newRotation) = computeRotation(floor, prevAngle);
                bpm = getNewBpm(bpm, floor);
                prevAngle = newRotation;

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
                            redPlanet.RotateTo(newRotation, CalculationExtensions.GetRelativeDuration(fixedRotation, newRotation, bpm), easing);
                        }

                        break;

                    case PlanetState.Ice:
                        using (bluePlanet.BeginAbsoluteSequence(startTimeOffset, false))
                        {
                            bluePlanet.ExpandTo(1);
                            bluePlanet.RotateTo(fixedRotation);
                            bluePlanet.RotateTo(newRotation, CalculationExtensions.GetRelativeDuration(fixedRotation, newRotation, bpm), easing);
                        }

                        break;
                }

                #endregion

                // 회전을 마치면 다른 행성으로 회전할 준비를 해야합니다.
                startTimeOffset += CalculationExtensions.GetRelativeDuration(fixedRotation, newRotation, bpm);
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
            float previousAngle = CalculationExtensions.GetSafeAngle(tilesInfo.First().Angle - 180);
            float oldCameraRotation = 0;
            float cameraRotation = 0;
            float oldCameraZoom = 0;
            float cameraZoom = 1;

            for (int floor = 0; floor < tilesInfo.Length; floor++)
            {
                bpm = getNewBpm(bpm, floor);
                var (fixedRotation, newRotation) = computeRotation(floor, previousAngle);
                previousAngle = newRotation;

                // Camera
                using (Child.BeginAbsoluteSequence(offset[floor], false))
                    Child.MoveTo(-tilesInfo[floor].Position, 400 + 60 / bpm * 500, Easing.OutSine);

                var angleTimeOffset = 0f;

                foreach (var action in tilesInfo[floor].Action)
                {
                    if (action.EventType == EventType.MoveCamera)
                    {
                        if (action.Rotation.HasValue)
                            cameraRotation = action.Rotation.Value;

                        if (action.Zoom.HasValue)
                            cameraZoom = (float)action.Zoom / 100;

                        angleTimeOffset = CalculationExtensions.GetRelativeDuration(fixedRotation, fixedRotation + action.AngleOffset, bpm);
                    }

                    // Camera zoom
                    if (!Precision.AlmostEquals(oldCameraZoom, cameraZoom))
                    {
                        using (BeginAbsoluteSequence(offset[floor] + angleTimeOffset, false))
                        {
                            this.ScaleTo(cameraZoom, CalculationExtensions.GetRelativeDuration(fixedRotation, newRotation, bpm) * action.Duration, action.Ease);
                        }

                        oldCameraZoom = cameraZoom;
                    }

                    // Camera rotation
                    if (!Precision.AlmostEquals(oldCameraRotation, cameraRotation))
                    {
                        using (BeginAbsoluteSequence(offset[floor] + angleTimeOffset, false))
                        {
                            this.RotateTo(cameraRotation, CalculationExtensions.GetRelativeDuration(fixedRotation, newRotation, bpm) * action.Duration, action.Ease);
                        }

                        oldCameraRotation = cameraRotation;
                    }
                }
            }
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

        private (float fixedRotation, float newRotation) computeRotation(int floor, float prevAngle) => CalculationExtensions.ComputeRotation(tilesInfo, floor, prevAngle);

        private RotationDirection getIsClockwise(int floor) => CalculationExtensions.GetIsClockwise(tilesInfo, floor) ? RotationDirection.Clockwise : RotationDirection.Counterclockwise;

        private float getNewBpm(float current, int floor) => CalculationExtensions.GetNewBpm(tilesInfo, current, floor);
    }
}
