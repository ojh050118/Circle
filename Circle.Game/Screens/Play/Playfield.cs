using System;
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
        private ObjectContainer tiles;
        private Planet redPlanet;
        private Planet bluePlanet;
        private Container<Planet> planetContainer;

        [Resolved]
        private Bindable<BeatmapInfo> beatmap { get; set; }

        private int currentFloor;

        private float prevAngle;

        private bool isClockwise;

        public float CurrentBpm { get; private set; }

        public Action OnComplete { get; set; }

        public double EndTime { get; private set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            AutoSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Children = new Drawable[]
            {
                tiles = new ObjectContainer(),
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
            redPlanet.Expansion = bluePlanet.Expansion = 0;
            isClockwise = true;
            CurrentBpm = beatmap.Value.Settings.Bpm;
            bluePlanet.Rotation = tiles.Children[0].Angle - 180;

            for (int i = 9; i < tiles.Children.Count; i++)
                tiles.Children[i].Alpha = 0;

            Scheduler.Add(addTransforms);
        }

        private void addTransforms()
        {
            double startTimeOffset = beatmap.Value.Settings.Offset - 60000 / CurrentBpm;
            PlanetState planetState = PlanetState.Fire;

            #region Initial transform

            using (bluePlanet.BeginAbsoluteSequence(startTimeOffset))
            {
                startTimeOffset += GetRelativeDuration(bluePlanet.Rotation, tiles.Children[currentFloor].Angle);
                bluePlanet.ExpandTo(1, 60000 / CurrentBpm, Easing.Out);
                bluePlanet.RotateTo(tiles.Children[currentFloor].Angle, GetRelativeDuration(bluePlanet.Rotation, tiles.Children[currentFloor].Angle));
            }

            using (bluePlanet.BeginAbsoluteSequence(startTimeOffset))
            {
                prevAngle = CalculationExtensions.GetSafeAngle(tiles.Children[currentFloor].Angle);
                currentFloor++;

                if (currentFloor < tiles.Children.Count)
                {
                    bluePlanet.ExpandTo(0);
                    using (planetContainer.BeginAbsoluteSequence(startTimeOffset))
                        planetContainer.MoveTo(tiles.Children[currentFloor].Position);
                }
            }

            #endregion

            while (currentFloor < tiles.Children.Count)
            {
                using (BeginAbsoluteSequence(startTimeOffset))
                    this.MoveTo(-tiles.Children[currentFloor].Position, 500, Easing.OutSine);

                setSpeed(tiles.Children[currentFloor].SpeedType);
                addTileTransform(startTimeOffset);

                if (tiles.Children[currentFloor].TileType == TileType.Midspin)
                {
                    currentFloor++;
                    setSpeed(tiles.Children[currentFloor].SpeedType);
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

                if (currentFloor < tiles.Children.Count)
                {
                    using (planetContainer.BeginAbsoluteSequence(startTimeOffset))
                        planetContainer.MoveTo(tiles.Children[currentFloor].Position);

                    if (tiles.Children[currentFloor].TileType == TileType.Normal)
                        planetState = planetState == PlanetState.Fire ? PlanetState.Ice : PlanetState.Fire;
                }
                else
                {
                    var lastPosition = tiles.Children[currentFloor - 1].Position + CalculationExtensions.GetComputedTilePosition(tiles.Children[currentFloor - 1].Angle);

                    using (BeginAbsoluteSequence(startTimeOffset))
                        this.MoveTo(-lastPosition, 500, Easing.OutSine);

                    using (planetContainer.BeginAbsoluteSequence(startTimeOffset))
                        planetContainer.MoveTo(lastPosition);

                    switch (planetState)
                    {
                        case PlanetState.Fire:
                            using (bluePlanet.BeginAbsoluteSequence(startTimeOffset))
                            {
                                bluePlanet.ExpandTo(1);
                                bluePlanet.Spin(60000 / CurrentBpm * 2, isClockwise ? RotationDirection.Clockwise : RotationDirection.Counterclockwise, prevAngle - 180);
                            }

                            break;

                        case PlanetState.Ice:
                            using (redPlanet.BeginAbsoluteSequence(startTimeOffset))
                            {
                                redPlanet.ExpandTo(1);
                                redPlanet.Spin(60000 / CurrentBpm * 2, isClockwise ? RotationDirection.Clockwise : RotationDirection.Counterclockwise, prevAngle - 180);
                            }

                            break;
                    }
                }
            }

            EndTime = startTimeOffset;
        }

        private (float fixedRotation, float newRotation) computeRotation()
        {
            var currentAngle = CalculationExtensions.GetSafeAngle(tiles.Children[currentFloor].Angle);

            // 소용돌이 적용.
            if (tiles.Children[currentFloor].Reverse.Value)
                isClockwise = !isClockwise;

            float fixedRotation = prevAngle;

            // 현재 타일이 미드스핀 타일일 때 계산 스킵.
            if (tiles.Children[currentFloor].TileType != TileType.Midspin && tiles.Children[currentFloor - 1].TileType != TileType.Midspin)
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

        private void setSpeed(SpeedType? speedType)
        {
            switch (speedType)
            {
                case SpeedType.Multiplier:
                    CurrentBpm *= tiles.Children[currentFloor].BpmMultiplier.Value;
                    break;

                case SpeedType.Bpm:
                    CurrentBpm = tiles.Children[currentFloor].Bpm.Value;
                    break;

                default:
                    return;
            }
        }

        private void addTileTransform(double startTimeOffset)
        {
            if (currentFloor + 8 < tiles.Children.Count)
            {
                using (tiles.Children[currentFloor + 8].BeginAbsoluteSequence(startTimeOffset))
                    tiles.Children[currentFloor + 8].FadeTo(0.6f, 60000 / CurrentBpm, Easing.Out);
            }
            else if (currentFloor < tiles.Children.Count)
            {
                using (tiles.Children[currentFloor].BeginAbsoluteSequence(startTimeOffset))
                    tiles.Children[currentFloor].FadeTo(0.6f, 60000 / CurrentBpm, Easing.Out);
            }

            if (currentFloor >= 4)
            {
                using (tiles.Children[currentFloor - 4].BeginAbsoluteSequence(startTimeOffset))
                    tiles.Children[currentFloor - 4].FadeOut(60000 / CurrentBpm, Easing.Out);
            }
        }

        public float GetRelativeDuration(float oldRotation, float newRotation)
        {
            return 60000 / CurrentBpm * Math.Abs(oldRotation - newRotation) / 180;
        }
    }
}
