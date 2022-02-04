using System;
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

        private TileInfo[] tileInfos;

        private bool isClockwise;

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
            isClockwise = true;
            redPlanet.Expansion = bluePlanet.Expansion = 0;
            bluePlanet.Rotation = tileInfos.First().Angle - 180;

            for (int i = 8; i < tileInfos.Length; i++)
                tileContainer.Children[i].Alpha = 0;
        }

        protected override void LoadComplete()
        {
            addTileTransforms(beatmap.Value.Settings.Offset - 60000 / beatmap.Value.Settings.Bpm);
            addTransforms(beatmap.Value.Settings.Offset - 60000 / beatmap.Value.Settings.Bpm);

            base.LoadComplete();
        }

        private void addTransforms(double gameplayStartTime)
        {
            double startTimeOffset = gameplayStartTime;
            PlanetState planetState = PlanetState.Ice;
            float previousAngle;
            float bpm = beatmap.Value.Settings.Bpm;
            int floor = 0;

            #region Initial planet rotate

            using (bluePlanet.BeginAbsoluteSequence(startTimeOffset))
            {
                startTimeOffset += GetRelativeDuration(bluePlanet.Rotation, tileInfos[floor].Angle, bpm);
                bluePlanet.ExpandTo(1, 60000 / bpm, Easing.Out);
                bluePlanet.RotateTo(tileInfos[floor].Angle, GetRelativeDuration(bluePlanet.Rotation, tileInfos[floor].Angle, bpm));
            }

            using (bluePlanet.BeginAbsoluteSequence(startTimeOffset))
            {
                previousAngle = tileInfos[floor].Angle;
                floor++;

                if (floor < tileInfos.Length)
                {
                    bluePlanet.ExpandTo(0);
                    using (planetContainer.BeginAbsoluteSequence(startTimeOffset))
                        planetContainer.MoveTo(tileInfos[floor].Position);

                    if (tileInfos[floor].TileType != TileType.Midspin)
                        planetState = PlanetState.Fire;
                }
            }

            #endregion

            while (floor < tileInfos.Length)
            {
                // Camera
                using (BeginAbsoluteSequence(startTimeOffset))
                    this.MoveTo(-tileInfos[floor].Position, 500, Easing.OutSine);

                var (fixedRotation, newRotation) = computeRotation(floor, previousAngle);
                bpm = getNewBpm(bpm, floor, tileInfos[floor].SpeedType);
                previousAngle = newRotation;

                switch (planetState)
                {
                    case PlanetState.Fire:
                        using (redPlanet.BeginAbsoluteSequence(startTimeOffset))
                        {
                            redPlanet.ExpandTo(1);
                            redPlanet.RotateTo(fixedRotation);
                            redPlanet.RotateTo(newRotation, GetRelativeDuration(fixedRotation, newRotation, bpm));
                        }

                        break;

                    case PlanetState.Ice:
                        using (bluePlanet.BeginAbsoluteSequence(startTimeOffset))
                        {
                            bluePlanet.ExpandTo(1);
                            bluePlanet.RotateTo(fixedRotation);
                            bluePlanet.RotateTo(newRotation, GetRelativeDuration(fixedRotation, newRotation, bpm));
                        }

                        break;
                }

                startTimeOffset += GetRelativeDuration(fixedRotation, newRotation, bpm);
                floor++;

                switch (planetState)
                {
                    case PlanetState.Fire:
                        using (redPlanet.BeginAbsoluteSequence(startTimeOffset))
                            redPlanet.ExpandTo(0);

                        break;

                    case PlanetState.Ice:
                        using (bluePlanet.BeginAbsoluteSequence(startTimeOffset))
                            bluePlanet.ExpandTo(0);

                        break;
                }

                if (floor < tileInfos.Length)
                {
                    using (planetContainer.BeginAbsoluteSequence(startTimeOffset))
                        planetContainer.MoveTo(tileInfos[floor].Position);

                    if (tileInfos[floor].TileType != TileType.Midspin && tileInfos[floor - 1].TileType != TileType.Midspin)
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
                                redPlanet.Spin(60000 / bpm * 2, isClockwise ? RotationDirection.Clockwise : RotationDirection.Counterclockwise, previousAngle);
                            }

                            break;

                        case PlanetState.Ice:
                            using (bluePlanet.BeginAbsoluteSequence(startTimeOffset))
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
            float bpm = beatmap.Value.Settings.Bpm;
            float previousAngle = CalculationExtensions.GetSafeAngle(tileInfos.First().Angle - 180);

            // Fade in
            for (int i = 8; i < tileInfos.Length; i++)
            {
                var (fixedRotation, newRotation) = computeRotation(tileInfos[i - 8].Floor, previousAngle);

                previousAngle = newRotation;
                bpm = getNewBpm(bpm, tileInfos[i - 8].Floor, tileInfos[i - 8].SpeedType);

                using (tileContainer.Children[i].BeginAbsoluteSequence(startTimeOffset))
                    tileContainer.Children[i].FadeTo(0.6f, 60000 / bpm, Easing.Out);

                startTimeOffset += GetRelativeDuration(fixedRotation, newRotation, bpm);
            }

            startTimeOffset = gameplayStartTime;
            bpm = beatmap.Value.Settings.Bpm;
            previousAngle = CalculationExtensions.GetSafeAngle(tileInfos.First().Angle - 180);
            isClockwise = true;

            // Fade out
            for (int i = 0; i < tileInfos.Length; i++)
            {
                var (fixedRotation, newRotation) = computeRotation(tileInfos[i].Floor, previousAngle);

                previousAngle = newRotation;
                bpm = getNewBpm(bpm, tileInfos[i].Floor, tileInfos[i].SpeedType);

                if (i > 3)
                {
                    using (tileContainer.Children[i - 4].BeginAbsoluteSequence(startTimeOffset))
                        tileContainer.Children[i - 4].FadeOut(60000 / bpm, Easing.Out);
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
            var currentAngle = CalculationExtensions.GetSafeAngle(tileInfos[floor].Angle);

            // 소용돌이 적용.
            if (tileInfos[floor].Twirl)
                isClockwise = !isClockwise;

            float fixedRotation = CalculationExtensions.GetSafeAngle(previousAngle);

            // 현재와 이전 타일이 미드스핀 타일일 때 회전각을 반전하면 안됩니다.
            if (floor > 0)
            {
                if (tileInfos[floor].TileType != TileType.Midspin && tileInfos[floor - 1].TileType != TileType.Midspin)
                    fixedRotation -= 180;
            }

            if (tileInfos[floor].TileType == TileType.Midspin)
                return (fixedRotation, currentAngle);

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
                    return current * tileInfos[floor].BpmMultiplier;

                case SpeedType.Bpm:
                    return tileInfos[floor].Bpm;

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
