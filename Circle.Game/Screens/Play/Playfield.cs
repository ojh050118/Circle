using System;
using System.Linq;
using Circle.Game.Beatmaps;
using Circle.Game.Graphics.UserInterface;
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
            float prevAngle;
            float bpm = currentBeatmap.Settings.Bpm;
            int floor = 0;
            Easing easing = Easing.None;

            #region Initial planet rotate

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
                // Camera
                using (Child.BeginAbsoluteSequence(startTimeOffset, false))
                    Child.MoveTo(-tilesInfo[floor].Position, 400 + 60 / bpm * 500, Easing.OutSine);

                // Camera shaking
                using (BeginAbsoluteSequence(startTimeOffset, false))
                    this.ScaleTo(1.02f).ScaleTo(1, 500 + 100 / bpm * 500, Easing.OutSine);

                var (fixedRotation, newRotation) = computeRotation(floor, prevAngle);
                bpm = getNewBpm(bpm, floor);
                prevAngle = newRotation;

                if (tilesInfo[floor].EventType == EventType.SetPlanetRotation)
                    easing = tilesInfo[floor].Easing;

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
            }

            startTimeOffset -= CalculationExtensions.GetRelativeDuration(prevAngle, prevAngle - 180, bpm);
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
                var (fixedRotation, newRotation) = computeRotation(i - 8, previousAngle);

                previousAngle = newRotation;
                bpm = getNewBpm(bpm, i - 8);

                using (tileContainer.Children[i].BeginAbsoluteSequence(startTimeOffset, false))
                    tileContainer.Children[i].FadeTo(0.45f, 60000 / bpm, Easing.Out);

                startTimeOffset += CalculationExtensions.GetRelativeDuration(fixedRotation, newRotation, bpm);
            }

            startTimeOffset = gameplayStartTime;
            bpm = currentBeatmap.Settings.Bpm;
            previousAngle = CalculationExtensions.GetSafeAngle(tilesInfo.First().Angle - 180);

            // Fade out
            for (int i = 0; i < tilesInfo.Length; i++)
            {
                var (fixedRotation, newRotation) = computeRotation(i, previousAngle);

                previousAngle = newRotation;
                bpm = getNewBpm(bpm, i);

                if (i > 3)
                {
                    using (tileContainer.Children[i - 4].BeginAbsoluteSequence(startTimeOffset, false))
                        tileContainer.Children[i - 4].FadeOut(60000 / bpm, Easing.Out).Then().Expire();
                }

                startTimeOffset += CalculationExtensions.GetRelativeDuration(fixedRotation, newRotation, bpm);
            }
        }

        /// <summary>
        /// 현재 타일로 자연스럽게 회전할 수 있는 각도를 제공합니다.
        /// </summary>
        /// <param name="floor">현재 타일 번호.</param>
        /// <param name="previousAngle">이전 타일 각도.</param>
        /// <returns>변환된 각도. (회전 전 각도, 회전 후 각도)</returns>
        private (float fixedRotation, float newRotation) computeRotation(int floor, float prevAngle) => CalculationExtensions.ComputeRotation(tilesInfo, floor, prevAngle);

        private RotationDirection getIsClockwise(int floor) => CalculationExtensions.GetIsClockwise(tilesInfo, floor) ? RotationDirection.Clockwise : RotationDirection.Counterclockwise;

        private float getNewBpm(float current, int floor) => CalculationExtensions.GetNewBpm(tilesInfo, current, floor);
    }
}
