using System;
using System.Collections.Generic;
using System.Linq;
using Circle.Game.Beatmap;
using Circle.Game.Rulesets.Extensions;
using Circle.Game.Rulesets.Objects;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK.Graphics;

namespace Circle.Game.Screens.Play
{
    public class Playfield : Container
    {
        private ObjectContainer tileContainer;
        private Planet redPlanet;
        private Planet bluePlanet;
        private Container<Planet> planetContainer;

        [Resolved]
        private Bindable<BeatmapInfo> beatmap { get; set; }

        private int currentFloor;

        private TileInfo[] tileInfos;

        private float prevAngle;

        private bool isClockwise;

        public float CurrentBpm { get; private set; }

        public Action OnComplete { get; set; }

        public double EndTime { get; private set; } = double.MaxValue;

        [BackgroundDependencyLoader]
        private void load()
        {
            AutoSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Children = new Drawable[]
            {
                tileContainer = new ObjectContainer(),
                planetContainer = new Container<Planet>
                {
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Children = new[]
                    {
                        redPlanet = new Planet(Color4.Red),
                        bluePlanet = new Planet(Color4.DeepSkyBlue),
                    }
                }
            };

            tileInfos = tileContainer.GetTileInfos().ToArray();
            CurrentBpm = beatmap.Value.Settings.Bpm;
            isClockwise = true;
            redPlanet.Expansion = bluePlanet.Expansion = 0;
            bluePlanet.Rotation = tileInfos.First().Angle - 180;
            Scheduler.Add(addTransforms);

            for (int i = 9; i < tileInfos.Length; i++)
                tileContainer.Children[i].Alpha = 0;
        }

        private void addTransforms()
        {
            double startTimeOffset = beatmap.Value.Settings.Offset - 60000 / beatmap.Value.Settings.Bpm;
            PlanetState planetState = PlanetState.Ice;

            #region Initial transform

            using (bluePlanet.BeginAbsoluteSequence(startTimeOffset))
            {
                startTimeOffset += GetRelativeDuration(bluePlanet.Rotation, tileInfos[currentFloor].Angle);
                bluePlanet.ExpandTo(1, 60000 / CurrentBpm, Easing.Out);
                bluePlanet.RotateTo(tileInfos[currentFloor].Angle, GetRelativeDuration(bluePlanet.Rotation, tileInfos[currentFloor].Angle));
            }

            using (bluePlanet.BeginAbsoluteSequence(startTimeOffset))
            {
                prevAngle = CalculationExtensions.GetSafeAngle(tileInfos[currentFloor].Angle);
                currentFloor++;

                if (currentFloor < tileInfos.Length)
                {
                    bluePlanet.ExpandTo(0);
                    using (planetContainer.BeginAbsoluteSequence(startTimeOffset))
                        planetContainer.MoveTo(tileInfos[currentFloor].Position);

                    if (tileInfos[currentFloor].TileType != TileType.Midspin)
                        planetState = PlanetState.Fire;
                }
            }

            #endregion

            while (currentFloor < tileInfos.Length)
            {
                using (BeginAbsoluteSequence(startTimeOffset))
                    this.MoveTo(-tileInfos[currentFloor].Position, 500, Easing.OutSine);

                setSpeed(currentFloor, tileInfos[currentFloor].SpeedType);
                addTileTransform(startTimeOffset);

                if (tileInfos[currentFloor].TileType == TileType.Midspin)
                {
                    currentFloor++;
                    setSpeed(currentFloor, tileInfos[currentFloor].SpeedType);
                    addTileTransform(startTimeOffset);
                }

                var (fixedRotation, newRotation) = computeRotation();

                if (planetState == PlanetState.Fire)
                {
                    using (redPlanet.BeginAbsoluteSequence(startTimeOffset))
                    {
                        startTimeOffset += GetRelativeDuration(fixedRotation, newRotation);
                        redPlanet.ExpandTo(1);
                        redPlanet.RotateTo(fixedRotation);
                        redPlanet.RotateTo(newRotation, GetRelativeDuration(fixedRotation, newRotation));
                    }

                    using (redPlanet.BeginAbsoluteSequence(startTimeOffset))
                    {
                        prevAngle = CalculationExtensions.GetSafeAngle(newRotation);
                        currentFloor++;
                        redPlanet.ExpandTo(0);
                    }
                }
                else
                {
                    using (bluePlanet.BeginAbsoluteSequence(startTimeOffset))
                    {
                        startTimeOffset += GetRelativeDuration(fixedRotation, newRotation);
                        bluePlanet.ExpandTo(1);
                        bluePlanet.RotateTo(fixedRotation);
                        bluePlanet.RotateTo(newRotation, GetRelativeDuration(fixedRotation, newRotation));
                    }

                    using (bluePlanet.BeginAbsoluteSequence(startTimeOffset))
                    {
                        prevAngle = CalculationExtensions.GetSafeAngle(newRotation);
                        currentFloor++;
                        bluePlanet.ExpandTo(0);
                    }
                }

                if (currentFloor < tileInfos.Length)
                {
                    using (planetContainer.BeginAbsoluteSequence(startTimeOffset))
                        planetContainer.MoveTo(tileInfos[currentFloor].Position);

                    if (tileInfos[currentFloor].TileType != TileType.Midspin)
                        planetState = planetState == PlanetState.Fire ? PlanetState.Ice : PlanetState.Fire;
                }
                else
                {
                    switch (planetState)
                    {
                        case PlanetState.Fire:
                            using (redPlanet.BeginAbsoluteSequence(startTimeOffset))
                            {
                                redPlanet.ExpandTo(1);
                                redPlanet.Spin(60000 / CurrentBpm * 2, isClockwise ? RotationDirection.Clockwise : RotationDirection.Counterclockwise, prevAngle);
                            }

                            break;

                        case PlanetState.Ice:
                            using (bluePlanet.BeginAbsoluteSequence(startTimeOffset))
                            {
                                bluePlanet.ExpandTo(1);
                                bluePlanet.Spin(60000 / CurrentBpm * 2, isClockwise ? RotationDirection.Clockwise : RotationDirection.Counterclockwise, prevAngle);
                            }

                            break;
                    }
                }
            }

            EndTime = startTimeOffset;
        }

        private (float fixedRotation, float newRotation) computeRotation()
        {
            var currentAngle = CalculationExtensions.GetSafeAngle(tileInfos[currentFloor].Angle);

            // 소용돌이 적용.
            if (tileInfos[currentFloor].Twirl)
                isClockwise = !isClockwise;

            float fixedRotation = prevAngle;

            // 현재 타일이 미드스핀 타일일 때 계산 스킵.
            if (tileInfos[currentFloor].TileType != TileType.Midspin && tileInfos[currentFloor - 1].TileType != TileType.Midspin)
                fixedRotation -= 180;

            float newRotation = currentAngle >= 180 ? currentAngle - 360 : currentAngle;

            // 회전방향에 따라 새로운 각도 계산
            if (isClockwise)
            {
                while (fixedRotation >= newRotation)
                    newRotation += 360;
            }
            else
            {
                newRotation = currentAngle >= 180 ? currentAngle : newRotation;

                while (fixedRotation <= newRotation)
                    newRotation -= 360;
            }

            return (fixedRotation, newRotation);
        }

        private void setSpeed(int floor, SpeedType? speedType)
        {
            switch (speedType)
            {
                case SpeedType.Multiplier:
                    CurrentBpm *= tileInfos[floor].BpmMultiplier;
                    break;

                case SpeedType.Bpm:
                    CurrentBpm = tileInfos[floor].Bpm;
                    break;

                default:
                    return;
            }
        }

        private void addTileTransforms(double gameplayStartTime)
        {
            double startTimeOffset = gameplayStartTime;

            foreach (var tile in tileContainer.Children)
            {
                using (tile.BeginAbsoluteSequence(startTimeOffset))
                {
                    //tile.FadeTo(0.6f, GetRelativeDuration())
                    List<IHasTileInfo> list = new List<IHasTileInfo>
                    {
                        tile
                    };
                    setSpeed(0, list[0].SpeedType);
                }
            }
        }

        private void addTileTransform(double startTimeOffset)
        {
            if (currentFloor + 8 < tileInfos.Length)
            {
                using (tileContainer.Children[currentFloor + 8].BeginAbsoluteSequence(startTimeOffset))
                    tileContainer.Children[currentFloor + 8].FadeTo(0.6f, 60000 / CurrentBpm, Easing.Out);
            }
            else if (currentFloor < tileInfos.Length)
            {
                using (tileContainer.Children[currentFloor].BeginAbsoluteSequence(startTimeOffset))
                    tileContainer.Children[currentFloor].FadeTo(0.6f, 60000 / CurrentBpm, Easing.Out);
            }

            if (currentFloor >= 4)
            {
                using (tileContainer.Children[currentFloor - 4].BeginAbsoluteSequence(startTimeOffset))
                    tileContainer.Children[currentFloor - 4].FadeOut(60000 / CurrentBpm, Easing.Out);
            }
        }

        public float GetRelativeDuration(float oldRotation, float newRotation)
        {
            return 60000 / CurrentBpm * Math.Abs(oldRotation - newRotation) / 180;
        }
    }
}
