#nullable disable

using System.Collections.Generic;
using System.Linq;
using Circle.Game.Beatmaps;
using Circle.Game.Rulesets.Objects;

namespace Circle.Game.Rulesets.Extensions
{
    public static class TilesInfoExtensions
    {
        /// <summary>
        /// 행성이 공전하는 것처럼 보이기 위해 각도를 계산합니다.
        /// </summary>
        /// <param name="tilesInfo">타일 정보들.</param>
        /// <param name="floor">현재 타일.</param>
        /// <param name="prevAngle">이전 타일 각도.</param>
        /// <returns>행성이 회전하기 전에 위치하는 각도.</returns>
        public static float ComputeRotation(this IReadOnlyList<TileInfo> tilesInfo, int floor, float prevAngle)
        {
            var newRotation = CalculationExtensions.GetSafeAngle(tilesInfo[floor].Angle);

            // 소용돌이에 대한 아무런 설정이 없으면 시계방향으로 회전합니다.
            bool isClockwise = tilesInfo.GetIsClockwise(floor + 1);

            float fixedRotation = CalculationExtensions.GetSafeAngle(prevAngle);

            // 현재와 이전 타일이 미드스핀 타일일 때 회전각을 반전하면 안됩니다.
            if (floor > 0)
            {
                if (tilesInfo[floor].TileType != TileType.Midspin && tilesInfo[floor - 1].TileType != TileType.Midspin)
                    fixedRotation = fixedRotation - 180 < 0 ? fixedRotation + 180 : fixedRotation - 180;
                else if (tilesInfo[floor].TileType == TileType.Midspin && tilesInfo[floor - 1].TileType == TileType.Midspin)
                    fixedRotation += fixedRotation - 180 < 0 ? fixedRotation + 180 : fixedRotation - 180;
            }

            if (tilesInfo[floor].TileType == TileType.Midspin)
                return fixedRotation;

            // 회전방향에 따라 새로운 각도 계산
            if (isClockwise)
            {
                while (fixedRotation >= newRotation)
                    fixedRotation -= 360;
            }
            else
            {
                while (fixedRotation <= newRotation)
                    fixedRotation += 360;
            }

            return fixedRotation;
        }

        /// <summary>
        /// 행성이 특정 타일에서 회전방뱡을 구합니다.
        /// </summary>
        /// <param name="tilesInfo">타일 정보.</param>
        /// <param name="floor">현재 타일</param>
        /// <returns>타일이 시계방향으로 회전하는지 여부.</returns>
        public static bool GetIsClockwise(this IReadOnlyList<TileInfo> tilesInfo, int floor)
        {
            bool isClockwise = true;

            // 소용돌이를 적용합니다.
            for (int i = 0; i < floor; i++)
            {
                foreach (var action in tilesInfo[i].Action)
                {
                    if (action.EventType == EventType.Twirl)
                        isClockwise = !isClockwise;
                }
            }

            return isClockwise;
        }

        /// <summary>
        /// 타일의 bpm을 적용합니다.
        /// </summary>
        /// <param name="tilesInfo">타일 정보.</param>
        /// <param name="bpm">현재 bpm.</param>
        /// <param name="floor">현재 타일.</param>
        /// <returns>현재bpm에서 승수가 적용된 값 또는 새로운 bpm.</returns>
        public static float GetNewBpm(this IReadOnlyList<TileInfo> tilesInfo, float bpm, int floor)
        {
            var speedAction = tilesInfo[floor].Action.FirstOrDefault(action => action.SpeedType.HasValue);

            switch (speedAction.SpeedType)
            {
                case SpeedType.Multiplier:
                    return bpm * speedAction.BpmMultiplier;

                case SpeedType.Bpm:
                    return speedAction.BeatsPerMinute;

                default:
                    return bpm;
            }
        }

        public static float GetRelativeDuration(this IReadOnlyList<TileInfo> tilesInfo, float oldRotation, int floor, float bpm)
        {
            if (tilesInfo[floor].TileType == TileType.Midspin)
                floor++;

            return CalculationExtensions.GetRelativeDuration(oldRotation, tilesInfo[floor].Angle, bpm);
        }
    }
}
