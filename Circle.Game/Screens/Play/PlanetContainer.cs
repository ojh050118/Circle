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

namespace Circle.Game.Screens.Play
{
    public class PlanetContainer : Container<Planet>
    {
        public Planet RedPlanet { get; private set; }
        public Planet BluePlanet { get; private set; }

        [BackgroundDependencyLoader]
        private void load(CircleConfigManager config)
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            AlwaysPresent = true;
            Children = new[]
            {
                RedPlanet = new Planet(Color4Utils.GetColor4(config.Get<Color4Enum>(CircleSetting.PlanetRed)))
                {
                    PlanetColour = config.GetBindable<Color4Enum>(CircleSetting.PlanetRed)
                },
                BluePlanet = new Planet(Color4Utils.GetColor4(config.Get<Color4Enum>(CircleSetting.PlanetBlue)))
                {
                    PlanetColour = config.GetBindable<Color4Enum>(CircleSetting.PlanetBlue)
                },
            };
        }

        public void AddPlanetTransform(Beatmap beatmap, double gameplayStartTime)
        {
            // 회전하고 있는 행성.
            PlanetState planetState = PlanetState.Ice;

            double startTimeOffset = gameplayStartTime;
            float prevAngle;
            float bpm = beatmap.Settings.Bpm;
            int floor = 0;
            Easing easing = Easing.None;

            #region Initial planet rotation

            using (BluePlanet.BeginAbsoluteSequence(startTimeOffset))
            {
                var duration = CalculationExtensions.GetRelativeDuration(BluePlanet.Rotation, beatmap.TilesInfo[floor].Angle, bpm);
                startTimeOffset += duration;
                BluePlanet.ExpandTo(1, 60000 / bpm, Easing.Out);
                BluePlanet.RotateTo(beatmap.TilesInfo[floor].Angle, duration, easing);
            }

            using (BluePlanet.BeginAbsoluteSequence(startTimeOffset))
            {
                prevAngle = beatmap.TilesInfo[floor].Angle;
                floor++;

                if (floor < beatmap.TilesInfo.Length)
                {
                    BluePlanet.ExpandTo(0);
                    using (BeginAbsoluteSequence(startTimeOffset))
                        this.MoveTo(beatmap.TilesInfo[floor].Position);

                    if (beatmap.TilesInfo[floor].TileType != TileType.Midspin)
                        planetState = PlanetState.Fire;
                }
            }

            #endregion

            while (floor < beatmap.TilesInfo.Length)
            {
                var fixedRotation = beatmap.TilesInfo.ComputeRotation(floor, prevAngle);
                bpm = beatmap.TilesInfo.GetNewBpm(bpm, floor);
                prevAngle = beatmap.TilesInfo[floor].Angle;

                // Apply easing
                var easingAction = beatmap.TilesInfo[floor].Action.FirstOrDefault(action => action.EventType == EventType.SetPlanetRotation);
                easing = easingAction.Ease;

                #region Planet rotation

                switch (planetState)
                {
                    case PlanetState.Fire:
                        using (RedPlanet.BeginAbsoluteSequence(startTimeOffset, false))
                        {
                            RedPlanet.ExpandTo(1);
                            RedPlanet.RotateTo(fixedRotation);
                            RedPlanet.RotateTo(beatmap.TilesInfo[floor].Angle, CalculationExtensions.GetRelativeDuration(fixedRotation, beatmap.TilesInfo[floor].Angle, bpm), easing);
                        }

                        break;

                    case PlanetState.Ice:
                        using (BluePlanet.BeginAbsoluteSequence(startTimeOffset, false))
                        {
                            BluePlanet.ExpandTo(1);
                            BluePlanet.RotateTo(fixedRotation);
                            BluePlanet.RotateTo(beatmap.TilesInfo[floor].Angle, CalculationExtensions.GetRelativeDuration(fixedRotation, beatmap.TilesInfo[floor].Angle, bpm), easing);
                        }

                        break;
                }

                #endregion

                // 회전을 마치면 다른 행성으로 회전할 준비를 해야합니다.
                startTimeOffset += CalculationExtensions.GetRelativeDuration(fixedRotation, beatmap.TilesInfo[floor].Angle, bpm);
                floor++;

                #region Planet reducation

                switch (planetState)
                {
                    case PlanetState.Fire:
                        using (RedPlanet.BeginAbsoluteSequence(startTimeOffset, false))
                            RedPlanet.ExpandTo(0);

                        break;

                    case PlanetState.Ice:
                        using (BluePlanet.BeginAbsoluteSequence(startTimeOffset, false))
                            BluePlanet.ExpandTo(0);

                        break;
                }

                #endregion

                #region Move PlanetContainer

                if (floor < beatmap.TilesInfo.Length)
                {
                    using (BeginAbsoluteSequence(startTimeOffset, false))
                        this.MoveTo(beatmap.TilesInfo[floor].Position);

                    if (beatmap.TilesInfo[floor].TileType != TileType.Midspin && beatmap.TilesInfo[floor - 1].TileType != TileType.Midspin)
                        planetState = planetState == PlanetState.Fire ? PlanetState.Ice : PlanetState.Fire;
                    else if (beatmap.TilesInfo[floor].TileType == TileType.Midspin && beatmap.TilesInfo[floor - 1].TileType == TileType.Midspin)
                        planetState = planetState == PlanetState.Fire ? PlanetState.Ice : PlanetState.Fire;
                }
                else
                {
                    // 마지막 타일에 도달했음을 의미합니다.
                    switch (planetState)
                    {
                        case PlanetState.Fire:
                            using (RedPlanet.BeginAbsoluteSequence(startTimeOffset, false))
                            {
                                RedPlanet.ExpandTo(1);
                                RedPlanet.Spin(60000 / bpm * 2, getIsClockwise(beatmap.TilesInfo, floor), prevAngle);
                            }

                            break;

                        case PlanetState.Ice:
                            using (BluePlanet.BeginAbsoluteSequence(startTimeOffset, false))
                            {
                                BluePlanet.ExpandTo(1);
                                BluePlanet.Spin(60000 / bpm * 2, getIsClockwise(beatmap.TilesInfo, floor), prevAngle);
                            }

                            break;
                    }
                }

                #endregion
            }
        }

        private RotationDirection getIsClockwise(IReadOnlyList<TileInfo> tilesInfo, int floor) => tilesInfo.GetIsClockwise(floor) ? RotationDirection.Clockwise : RotationDirection.Counterclockwise;
    }

    public enum PlanetState
    {
        Fire,
        Ice
    }
}
