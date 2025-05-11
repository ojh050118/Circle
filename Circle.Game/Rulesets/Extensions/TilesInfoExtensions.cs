#nullable disable

using System.Collections.Generic;
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
        public static float ComputeRotation(this IReadOnlyList<Tile> tilesInfo, int floor, float prevAngle)
        {
            var prevTileType = floor > 0 ? tilesInfo[floor - 1].TileType : TileType.Normal;

            return CalculationExtensions.ComputeStartRotation(prevTileType, prevAngle, tilesInfo[floor].TileType, tilesInfo[floor].Angle, tilesInfo[floor].Clockwise);
        }

        public static float GetRelativeDuration(this IReadOnlyList<Tile> tilesInfo, float oldRotation, int floor, float bpm)
        {
            if (tilesInfo[floor].TileType == TileType.Midspin)
                floor++;

            return CalculationExtensions.GetRelativeDuration(oldRotation, tilesInfo[floor].Angle, bpm);
        }
    }
}
