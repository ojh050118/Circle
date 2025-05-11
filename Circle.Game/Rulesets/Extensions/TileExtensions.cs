using System;
using System.Collections.Generic;
using System.Linq;
using Circle.Game.Beatmaps;
using Circle.Game.Rulesets.Objects;
using osuTK;

namespace Circle.Game.Rulesets.Extensions
{
    public static class TileExtensions
    {
        public static IEnumerable<Tile> GetTiles(Beatmap beatmap)
        {
            float[] angles = CalculationExtensions.ConvertAngles(beatmap.AngleData);
            var tiles = new Tile[angles.Length];

            float prevAngle = 0;
            var prevTileType = TileType.Normal;

            bool clockwise = true;
            float bpm = beatmap.Metadata.Bpm;
            var offset = Vector2.Zero;
            float countdownDuration = beatmap.Metadata.CountdownTicks * (60000 / bpm);
            double hitTimeOffset = beatmap.Metadata.Offset - countdownDuration;

            for (int floor = 0; floor < angles.Length; floor++)
            {
                float angle = angles[floor];
                float? nextAngle = floor + 1 < angles.Length ? angles[floor + 1] : null;

                var tileType = CalculationExtensions.GetTileType(prevAngle, angle, nextAngle);
                var actions = Array.FindAll(beatmap.Actions, a => a.Floor == floor);

                if (actions.Any(a => a.EventType == EventType.Twirl))
                    clockwise = !clockwise;

                float resolvedAngle = CalculationExtensions.ComputeStartRotation(prevTileType, prevAngle, tileType, angle, clockwise);
                double pauseDuration = actions.FirstOrDefault(a => a.EventType == EventType.Pause).Duration * (60000 / bpm);

                tiles[floor] = new Tile
                {
                    Floor = floor,
                    Angle = tileType == TileType.Midspin ? prevAngle : angle,
                    Position = offset,
                    TileType = tileType,
                    Actions = actions,
                    Clockwise = clockwise,
                    Bpm = bpm = CalculationExtensions.GetNewBpm(bpm, actions.FirstOrDefault(a => a.EventType == EventType.SetSpeed)),
                    HitTime = hitTimeOffset
                };

                if (floor == 0)
                    resolvedAngle = prevAngle - CalculationExtensions.GetTimeBasedRotation(hitTimeOffset, hitTimeOffset + countdownDuration, bpm);

                hitTimeOffset += CalculationExtensions.GetRelativeDuration(resolvedAngle, angle, bpm) + pauseDuration;

                switch (tileType)
                {
                    case TileType.Normal:
                    case TileType.Short:
                    case TileType.Circular:
                        if (floor + 1 < angles.Length)
                            offset += CalculationExtensions.GetNextTilePosition(angle);

                        break;

                    case TileType.Midspin:
                        offset -= CalculationExtensions.GetNextTilePosition(prevAngle);
                        break;
                }

                // 연속된 미드스핀 타일이 있을 때 두번째와 그 이후의 타일의 각도값이 999가 되는 것을 차단합니다.
                if (tileType != TileType.Midspin)
                    prevAngle = angle;

                prevTileType = tileType;
            }

            return tiles;
        }
    }
}
