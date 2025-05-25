using System;
using System.Collections.Generic;
using System.Linq;
using Circle.Game.Beatmaps;
using Circle.Game.Rulesets.Objects;
using osu.Framework.Graphics;
using osuTK;

namespace Circle.Game.Rulesets.Extensions
{
    public static class TileExtensions
    {
        public static IEnumerable<Tile> GetTiles(Beatmap beatmap)
        {
            float[] angles = CalculationExtensions.ConvertAngles(beatmap.AngleData);
            var tiles = new Tile[angles.Length];

            float bpm = beatmap.Metadata.Bpm;
            double countdownDuration = beatmap.Metadata.CountdownDuration;

            var positionOffset = Vector2.Zero;
            double startTimeOffset = beatmap.Metadata.Offset;
            double hitTimeOffset = beatmap.Metadata.Offset + countdownDuration; // 첫 타일은 따로 처리

            var prevTileType = TileType.Normal;
            bool clockwise = true;
            var easing = Easing.None;

            float prevAngle = angles.FirstOrDefault() - CalculationExtensions.GetTimeBasedRotation(startTimeOffset, hitTimeOffset, bpm);
            float prevRawAngle = beatmap.AngleData.FirstOrDefault(); // 미드스핀 타일과 다음 타일이 반대방향일 때 잘못된 타일타입 반환을 막기위해 필요합니다.

            for (int floor = 0; floor < angles.Length; floor++)
            {
                float angle = angles[floor];
                float? nextAngle = floor + 1 < angles.Length ? angles[floor + 1] : null;

                var tileType = CalculationExtensions.GetTileType(prevRawAngle, angle, nextAngle);
                var actions = Array.FindAll(beatmap.Actions, a => a.Floor == floor);

                if (tileType == TileType.Midspin)
                    positionOffset -= CalculationExtensions.GetNextTilePosition(prevAngle);

                // Apply twirl
                if (actions.Any(a => a.EventType == EventType.Twirl))
                    clockwise = !clockwise;

                // Apply ease
                if (actions.Any(a => a.EventType == EventType.SetPlanetRotation))
                    easing = actions.FirstOrDefault(a => a.EventType == EventType.SetPlanetRotation).Ease;

                angle = tileType == TileType.Midspin ? prevAngle : angle;
                bpm = CalculationExtensions.GetNewBpm(bpm, actions.FirstOrDefault(a => a.EventType == EventType.SetSpeed));

                float resolvedAngle = CalculationExtensions.ComputeStartRotation(prevTileType, prevAngle, tileType, angle, clockwise);
                double pauseDuration = actions.FirstOrDefault(a => a.EventType == EventType.Pause).Duration * (60000 / bpm);
                double duration = CalculationExtensions.GetRelativeDuration(resolvedAngle, angle, bpm) + pauseDuration;

                if (floor != 0)
                    hitTimeOffset += duration;

                tiles[floor] = new Tile
                {
                    Floor = floor,
                    Angle = angle,
                    Position = positionOffset,
                    TileType = tileType,
                    Actions = actions,
                    Clockwise = clockwise,
                    Bpm = bpm,
                    StartTime = startTimeOffset,
                    HitTime = hitTimeOffset,
                    Easing = easing
                };

                switch (tileType)
                {
                    case TileType.Normal:
                    case TileType.Short:
                    case TileType.Circular:
                        if (floor + 1 < angles.Length)
                            positionOffset += CalculationExtensions.GetNextTilePosition(angle);

                        break;
                }

                startTimeOffset = hitTimeOffset;
                prevAngle = angle;
                prevRawAngle = angles[floor];
                prevTileType = tileType;
            }

            return tiles;
        }

        public static float ComputeStartRotation(this Tile tile, TileType prevTileType, float prevAngle)
        {
            return CalculationExtensions.ComputeStartRotation(prevTileType, prevAngle, tile.TileType, tile.Angle, tile.Clockwise);
        }

        public static float GetRelativeDuration(this IReadOnlyList<Tile> tiles, float oldRotation, int floor, float bpm)
        {
            while (tiles[floor].TileType == TileType.Midspin)
                floor++;

            return CalculationExtensions.GetRelativeDuration(oldRotation, tiles[floor].Angle, bpm);
        }
    }
}
