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

            float prevAngle = angles.FirstOrDefault();
            float prevRawAngle = prevAngle; // 미드스핀 타일과 다음 타일이 반대방향일 때 잘못된 타일타입 반환을 막기위해 필요합니다.
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

                var tileType = CalculationExtensions.GetTileType(prevRawAngle, angle, nextAngle);
                var actions = Array.FindAll(beatmap.Actions, a => a.Floor == floor);

                if (actions.Any(a => a.EventType == EventType.Twirl))
                    clockwise = !clockwise;

                tiles[floor] = new Tile
                {
                    Floor = floor,
                    Angle = angle = tileType == TileType.Midspin ? prevAngle : angle,
                    Position = offset,
                    TileType = tileType,
                    Actions = actions,
                    Clockwise = clockwise,
                    Bpm = bpm = CalculationExtensions.GetNewBpm(bpm, actions.FirstOrDefault(a => a.EventType == EventType.SetSpeed)),
                    HitTime = hitTimeOffset
                };

                float resolvedAngle = CalculationExtensions.ComputeStartRotation(prevTileType, prevAngle, tileType, angle, clockwise);
                double pauseDuration = actions.FirstOrDefault(a => a.EventType == EventType.Pause).Duration * (60000 / bpm);

                // 첫 타일은 오프셋 - 카운트다운 시간, 게임시작시간은 오프셋, 두번째 타일은 오프셋 + 카운트다운 시간입니다.
                // 때문에, 두번째 타일의 적중시간을 카운트다운 시간의 두배를 적용합니다.
                if (floor == 0)
                    resolvedAngle = prevAngle - CalculationExtensions.GetTimeBasedRotation(hitTimeOffset, hitTimeOffset + countdownDuration * 2, bpm);

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
