#nullable disable

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
    public partial class PlanetContainer : Container<Planet>
    {
        public Planet RedPlanet { get; private set; }
        public Planet BluePlanet { get; private set; }

        private readonly Beatmap beatmap;

        public PlanetContainer(Beatmap beatmap)
        {
            this.beatmap = beatmap;
        }

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

            RedPlanet.Expansion = BluePlanet.Expansion = 0;
            BluePlanet.Rotation = beatmap.Tiles.First().Angle - CalculationExtensions.GetTimeBasedRotation(beatmap.Metadata.Offset, beatmap.Tiles.First().HitTime, beatmap.Metadata.Bpm);
        }

        public void AddPlanetTransform()
        {
            // 회전하고 있는 행성.
            PlanetState planetState = PlanetState.Ice;

            // TODO: 시작시간 오프셋 변수 제거 고려
            double startTimeOffset = beatmap.Metadata.Offset;
            TileType prevTileType;
            float prevAngle;
            float bpm = beatmap.Metadata.Bpm;
            int floor = 0;
            Easing easing = Easing.None;
            var tiles = beatmap.Tiles.ToArray();

            #region Initial planet rotation

            using (BluePlanet.BeginAbsoluteSequence(startTimeOffset))
            {
                float duration = CalculationExtensions.GetRelativeDuration(BluePlanet.Rotation, tiles[floor].Angle, bpm);
                startTimeOffset += duration;
                BluePlanet.ExpandTo(1, 60000 / bpm, Easing.Out);
                BluePlanet.RotateTo(tiles[floor].Angle, duration, easing);
            }

            using (BluePlanet.BeginAbsoluteSequence(startTimeOffset))
            {
                prevAngle = tiles[floor].Angle;
                prevTileType = tiles[floor].TileType;
                floor++;

                if (floor < tiles.Length)
                {
                    BluePlanet.ExpandTo(0);
                    using (BeginAbsoluteSequence(startTimeOffset))
                        this.MoveTo(tiles[floor].Position);

                    if (tiles[floor].TileType != TileType.Midspin)
                        planetState = PlanetState.Fire;
                }
            }

            #endregion

            while (floor < tiles.Length)
            {
                float fixedRotation = tiles[floor].ComputeStartRotation(prevTileType, prevAngle);
                bpm = tiles[floor].Bpm;
                prevAngle = tiles[floor].Angle;
                prevTileType = tiles[floor].TileType;

                // Apply easing
                var easingAction = tiles[floor].Actions.FirstOrDefault(action => action.EventType == EventType.SetPlanetRotation);
                easing = easingAction.Ease;

                double pause = tiles[floor].Actions.FirstOrDefault(action => action.EventType == EventType.Pause).Duration * (60000 / bpm);

                #region Planet rotation

                switch (planetState)
                {
                    case PlanetState.Fire:
                        using (RedPlanet.BeginAbsoluteSequence(startTimeOffset, false))
                        {
                            RedPlanet.ExpandTo(1);
                            RedPlanet.RotateTo(fixedRotation);
                            RedPlanet.RotateTo(tiles[floor].Angle, CalculationExtensions.GetRelativeDuration(fixedRotation, tiles[floor].Angle, bpm), easing);
                        }

                        break;

                    case PlanetState.Ice:
                        using (BluePlanet.BeginAbsoluteSequence(startTimeOffset, false))
                        {
                            BluePlanet.ExpandTo(1);
                            BluePlanet.RotateTo(fixedRotation);
                            BluePlanet.RotateTo(tiles[floor].Angle, CalculationExtensions.GetRelativeDuration(fixedRotation, tiles[floor].Angle, bpm), easing);
                        }

                        break;
                }

                #endregion

                // 회전을 마치면 다른 행성으로 회전할 준비를 해야합니다.
                startTimeOffset += CalculationExtensions.GetRelativeDuration(fixedRotation, tiles[floor].Angle, bpm) + pause;
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

                if (floor < tiles.Length)
                {
                    using (BeginAbsoluteSequence(startTimeOffset, false))
                        this.MoveTo(tiles[floor].Position);

                    if (tiles[floor].TileType != TileType.Midspin && tiles[floor - 1].TileType != TileType.Midspin)
                        planetState = planetState == PlanetState.Fire ? PlanetState.Ice : PlanetState.Fire;
                    else if (tiles[floor].TileType == TileType.Midspin && tiles[floor - 1].TileType == TileType.Midspin)
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
                                RedPlanet.Spin(60000 / bpm * 2, tiles[^1].Clockwise ? RotationDirection.Clockwise : RotationDirection.Counterclockwise, prevAngle);
                            }

                            break;

                        case PlanetState.Ice:
                            using (BluePlanet.BeginAbsoluteSequence(startTimeOffset, false))
                            {
                                BluePlanet.ExpandTo(1);
                                BluePlanet.Spin(60000 / bpm * 2, tiles[^1].Clockwise ? RotationDirection.Clockwise : RotationDirection.Counterclockwise, prevAngle);
                            }

                            break;
                    }
                }

                #endregion
            }
        }
    }

    public enum PlanetState
    {
        Fire,
        Ice
    }
}
