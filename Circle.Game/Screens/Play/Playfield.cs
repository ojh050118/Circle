using System;
using System.Linq;
using Circle.Game.Beatmaps;
using Circle.Game.Rulesets.Extensions;
using Circle.Game.Rulesets.Objects;
using osu.Framework.Allocation;
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

        private TileInfo[] tilesInfo;

        private bool isClockwise;

        public Action OnComplete { get; set; }

        public double EndTime { get; private set; } = double.MaxValue;

        private readonly Beatmap currentBeatmap;

        public Playfield(Beatmap beatmap)
        {
            currentBeatmap = beatmap;
        }

        [BackgroundDependencyLoader]
        private void load()
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
                            redPlanet = new Planet(Color4.Red),
                            bluePlanet = new Planet(Color4.DeepSkyBlue),
                        }
                    }
                }
            };

            tilesInfo = tileContainer.GetTilesInfo().ToArray();
            isClockwise = true;
            redPlanet.Expansion = bluePlanet.Expansion = 0;
            bluePlanet.Rotation = tilesInfo.First().Angle - 180;
        }

        protected override void LoadComplete()
        {
            addTileTransforms(currentBeatmap.Settings.Offset - 60000 / currentBeatmap.Settings.Bpm);
            addTransforms(currentBeatmap.Settings.Offset - 60000 / currentBeatmap.Settings.Bpm);

            base.LoadComplete();
        }

        private void addTransforms(double gameplayStartTime)
        {
            // 회전하고 있는 행성.
            PlanetState planetState = PlanetState.Ice;

            double startTimeOffset = gameplayStartTime;
            float previousAngle;
            float bpm = currentBeatmap.Settings.Bpm;
            int floor = 0;
            Easing easing = Easing.None;

            #region Initial planet rotate

            using (bluePlanet.BeginAbsoluteSequence(startTimeOffset))
            {
                startTimeOffset += GetRelativeDuration(bluePlanet.Rotation, tilesInfo[floor].Angle, bpm);
                bluePlanet.ExpandTo(1, 60000 / bpm, Easing.Out);
                bluePlanet.RotateTo(tilesInfo[floor].Angle, GetRelativeDuration(bluePlanet.Rotation, tilesInfo[floor].Angle, bpm), easing);
            }

            using (bluePlanet.BeginAbsoluteSequence(startTimeOffset))
            {
                previousAngle = tilesInfo[floor].Angle;
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
                // Camera
                using (Child.BeginAbsoluteSequence(startTimeOffset, false))
                    Child.MoveTo(-tilesInfo[floor].Position, 400 + 60 / bpm * 500, Easing.OutSine);

                using (BeginAbsoluteSequence(startTimeOffset, false))
                    this.ScaleTo(1.02f).ScaleTo(1, 500 + 100 / bpm * 500, Easing.OutSine);

                var (fixedRotation, newRotation) = computeRotation(floor, previousAngle);
                bpm = getNewBpm(bpm, floor, tilesInfo[floor].SpeedType);
                previousAngle = newRotation;

                if (tilesInfo[floor].EventType == EventType.SetPlanetRotation)
                    easing = tilesInfo[floor].Easing;

                #region Planet totation

                switch (planetState)
                {
                    case PlanetState.Fire:
                        using (redPlanet.BeginAbsoluteSequence(startTimeOffset, false))
                        {
                            redPlanet.ExpandTo(1);
                            redPlanet.RotateTo(fixedRotation);
                            redPlanet.RotateTo(newRotation, GetRelativeDuration(fixedRotation, newRotation, bpm), easing);
                        }

                        break;

                    case PlanetState.Ice:
                        using (bluePlanet.BeginAbsoluteSequence(startTimeOffset, false))
                        {
                            bluePlanet.ExpandTo(1);
                            bluePlanet.RotateTo(fixedRotation);
                            bluePlanet.RotateTo(newRotation, GetRelativeDuration(fixedRotation, newRotation, bpm), easing);
                        }

                        break;
                }

                #endregion

                // 회전을 마치면 다른 행성으로 회전할 준비를 해야합니다.
                startTimeOffset += GetRelativeDuration(fixedRotation, newRotation, bpm);
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
                    switch (planetState)
                    {
                        case PlanetState.Fire:
                            using (redPlanet.BeginAbsoluteSequence(startTimeOffset, false))
                            {
                                redPlanet.ExpandTo(1);
                                redPlanet.Spin(60000 / bpm * 2, isClockwise ? RotationDirection.Clockwise : RotationDirection.Counterclockwise, previousAngle);
                            }

                            break;

                        case PlanetState.Ice:
                            using (bluePlanet.BeginAbsoluteSequence(startTimeOffset, false))
                            {
                                bluePlanet.ExpandTo(1);
                                bluePlanet.Spin(60000 / bpm * 2, isClockwise ? RotationDirection.Clockwise : RotationDirection.Counterclockwise, previousAngle);
                            }

                            break;
                    }
                }
            }

            EndTime = startTimeOffset;
        }

        private void addTileTransforms(double gameplayStartTime)
        {
            double startTimeOffset = gameplayStartTime;
            float bpm = currentBeatmap.Settings.Bpm;
            float previousAngle = CalculationExtensions.GetSafeAngle(tilesInfo.First().Angle - 180);

            for (int i = 8; i < tilesInfo.Length; i++)
                tileContainer.Children[i].Alpha = 0;

            // Fade in
            for (int i = 8; i < tilesInfo.Length; i++)
            {
                var (fixedRotation, newRotation) = computeRotation(tilesInfo[i - 8].Floor, previousAngle);

                previousAngle = newRotation;
                bpm = getNewBpm(bpm, tilesInfo[i - 8].Floor, tilesInfo[i - 8].SpeedType);

                using (tileContainer.Children[i].BeginAbsoluteSequence(startTimeOffset, false))
                    tileContainer.Children[i].FadeTo(0.45f, 60000 / bpm, Easing.Out);

                startTimeOffset += GetRelativeDuration(fixedRotation, newRotation, bpm);
            }

            startTimeOffset = gameplayStartTime;
            bpm = currentBeatmap.Settings.Bpm;
            previousAngle = CalculationExtensions.GetSafeAngle(tilesInfo.First().Angle - 180);
            isClockwise = true;

            // Fade out
            for (int i = 0; i < tilesInfo.Length; i++)
            {
                var (fixedRotation, newRotation) = computeRotation(tilesInfo[i].Floor, previousAngle);

                previousAngle = newRotation;
                bpm = getNewBpm(bpm, tilesInfo[i].Floor, tilesInfo[i].SpeedType);

                if (i > 3)
                {
                    using (tileContainer.Children[i - 4].BeginAbsoluteSequence(startTimeOffset, false))
                        tileContainer.Children[i - 4].FadeOut(60000 / bpm, Easing.Out).Then().Expire();
                }

                startTimeOffset += GetRelativeDuration(fixedRotation, newRotation, bpm);
            }

            isClockwise = true;
        }

        /// <summary>
        /// 현재 타일로 자연스럽게 회전할 수 있는 각도를 제공합니다.
        /// </summary>
        /// <param name="floor">현재 타일 번호.</param>
        /// <param name="previousAngle">이전 타일 각도.</param>
        /// <returns>변환된 각도. (회전 전 각도, 회전 후 각도)</returns>
        private (float fixedRotation, float newRotation) computeRotation(int floor, float previousAngle)
        {
            var currentAngle = CalculationExtensions.GetSafeAngle(tilesInfo[floor].Angle);

            // 소용돌이 적용.
            if (tilesInfo[floor].Twirl)
                isClockwise = !isClockwise;

            float fixedRotation = CalculationExtensions.GetSafeAngle(previousAngle);

            // 현재와 이전 타일이 미드스핀 타일일 때 회전각을 반전하면 안됩니다.
            if (floor > 0)
            {
                if (tilesInfo[floor].TileType != TileType.Midspin && tilesInfo[floor - 1].TileType != TileType.Midspin)
                    fixedRotation -= 180;
                else if (tilesInfo[floor].TileType == TileType.Midspin && tilesInfo[floor - 1].TileType == TileType.Midspin)
                    fixedRotation -= 180;
            }

            if (tilesInfo[floor].TileType == TileType.Midspin)
                return (fixedRotation, fixedRotation);

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

        private float getNewBpm(float current, int floor, SpeedType? speedType)
        {
            switch (speedType)
            {
                case SpeedType.Multiplier:
                    return current * tilesInfo[floor].BpmMultiplier;

                case SpeedType.Bpm:
                    return tilesInfo[floor].Bpm;

                default:
                    return current;
            }
        }

        public float GetRelativeDuration(float oldRotation, float newRotation, float bpm)
        {
            return 60000 / bpm * Math.Abs(oldRotation - newRotation) / 180;
        }
    }
}
