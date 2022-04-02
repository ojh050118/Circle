using System;
using System.Collections.Generic;
using System.Linq;
using Circle.Game.Beatmaps;
using Circle.Game.Rulesets.Objects;
using osuTK;

namespace Circle.Game.Rulesets.Extensions
{
    public static class CalculationExtensions
    {
        /// <summary>
        /// 각도와 타일의 폭으로 다음 타일의 위치를 계산합니다.
        /// </summary>
        /// <param name="angle">각도.</param>
        /// <returns>다음 타일의 위치.</returns>
        public static Vector2 GetComputedTilePosition(float angle)
        {
            var x = (float)Math.Cos(MathHelper.DegreesToRadians(angle)) * (Tile.WIDTH - Tile.HEIGHT);
            var y = (float)Math.Sin(MathHelper.DegreesToRadians(angle)) * (Tile.WIDTH - Tile.HEIGHT);

            return new Vector2(x, y);
        }

        /// <summary>
        /// 60분법 범위사이의 각도로 클램프 합니다.
        /// </summary>
        /// <param name="angle">각도.</param>
        /// <returns>60분법 범위의 각도</returns>
        public static float GetSafeAngle(float angle)
        {
            if (angle < 0)
            {
                while (angle < 0)
                    angle += 360;

                return angle;
            }

            if (angle <= 360)
                return angle;

            while (angle > 360)
                angle -= 360;

            return angle;
        }

        /// <summary>
        /// 행성이 타일에 도착하는 시간을 계산합니다.
        /// </summary>
        /// <param name="beatmap">비트맵.</param>
        /// <param name="gameplayStartTime">게임 시작 시간.</param>
        /// <returns>행성이 타일이 도착하는 시간의 집합.</returns>
        public static IReadOnlyList<double> GetTileStartTime(Beatmap beatmap, double gameplayStartTime, double startOffset)
        {
            var tilesInfo = GetTilesInfo(beatmap).ToArray();
            double startTimeOffset = gameplayStartTime;
            float bpm = beatmap.Settings.Bpm;
            float prevAngle = tilesInfo[0].Angle;
            List<double> hitStartTimes = new List<double> { gameplayStartTime - startOffset };
            startTimeOffset += GetRelativeDuration(prevAngle - GetTimebasedRotation(gameplayStartTime, gameplayStartTime - startOffset, bpm), tilesInfo[0].Angle, bpm);
            hitStartTimes.Add(startTimeOffset);

            for (int floor = 1; floor < tilesInfo.Length - 1; floor++)
            {
                var fixedRotation = ComputeRotation(tilesInfo, floor, prevAngle);

                prevAngle = tilesInfo[floor].Angle;
                bpm = GetNewBpm(tilesInfo, bpm, floor);
                startTimeOffset += GetRelativeDuration(fixedRotation, tilesInfo[floor].Angle, bpm);
                hitStartTimes.Add(startTimeOffset);
            }

            return hitStartTimes;
        }

        /// <summary>
        /// 행성이 공전하는 것처럼 보이기 위해 각도를 계산합니다.
        /// </summary>
        /// <param name="tilesInfo">타일 정보들.</param>
        /// <param name="floor">현재 타일.</param>
        /// <param name="prevAngle">이전 타일 각도.</param>
        /// <returns>행성이 회전하기 전에 위치하는 각도.</returns>
        public static float ComputeRotation(TileInfo[] tilesInfo, int floor, float prevAngle)
        {
            var newRotation = GetSafeAngle(tilesInfo[floor].Angle);

            // 소용돌이에 대한 아무런 설정이 없으면 시계방향으로 회전합니다.
            bool isClockwise = GetIsClockwise(tilesInfo, floor + 1);

            float fixedRotation = GetSafeAngle(prevAngle);

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
        public static bool GetIsClockwise(TileInfo[] tilesInfo, int floor)
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
        public static float GetNewBpm(TileInfo[] tilesInfo, float bpm, int floor)
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

        /// <summary>
        /// 회전 각도와 상관없이 일정한 속도로 회전할 수 있는 시간을 계산합니다.
        /// </summary>
        /// <param name="oldRotation">이전 각도.</param>
        /// <param name="newRotation">새로운 각도.</param>
        /// <param name="bpm">현재 bpm.</param>
        /// <returns>계산된 시간.</returns>
        public static float GetRelativeDuration(float oldRotation, float newRotation, float bpm)
        {
            return 60000 / bpm * Math.Abs(oldRotation - newRotation) / 180;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset">회전하기 전 각도.</param>
        /// <param name="goalTime">다음각도까지 회전해야하는 시간.</param>
        /// <param name="bpm">현재 bpm.</param>
        /// <returns></returns>
        public static float GetTimebasedRotation(double offset, double goalTime, float bpm)
        {
            return (float)Math.Abs(offset - goalTime) / (60000 / bpm) * 180;
        }

        /// <summary>
        /// <paramref name="angleData"/>로 타일 타입을 구분합니다.
        /// 마지막 타일 타입은 <see cref="TileType.Circular"/>입니다.
        /// </summary>
        /// <param name="angleData">각도 데이터.</param>
        /// <returns>타일 타입의 집합.</returns>
        public static IReadOnlyList<TileType> GetTileType(float[] angleData)
        {
            List<TileType> tileTypes = new List<TileType>();

            for (int floor = 0; floor < angleData.Length; floor++)
            {
                if (floor + 1 < angleData.Length)
                {
                    if (angleData[floor + 1] == 999 && angleData[floor] != 999)
                    {
                        tileTypes.Add(TileType.Short);
                        continue;
                    }
                    else if (Math.Abs(angleData[floor] - angleData[floor + 1]) == 180)
                    {
                        tileTypes.Add(TileType.Short);
                        continue;
                    }
                    else if (floor > 0)
                    {
                        if (Math.Abs(angleData[floor] - angleData[floor - 1]) == 180)
                        {
                            tileTypes.Add(TileType.Circular);
                            continue;
                        }
                    }
                }
                else
                {
                    // 마지막 타일을 의미합니다.
                    tileTypes.Add(TileType.Circular);
                    break;
                }

                if (angleData[floor] == 999)
                {
                    tileTypes.Add(TileType.Midspin);
                    continue;
                }

                tileTypes.Add(TileType.Normal);
            }

            return tileTypes;
        }

        /// <summary>
        /// 타일의 위치를 계산합니다.
        /// </summary>
        /// <param name="angleData">각도 데이터.</param>
        /// <returns>각 타일의 위치의 집합.</returns>
        public static IReadOnlyList<Vector2> GetTilePositions(float[] angleData)
        {
            var types = GetTileType(angleData);
            var positions = new List<Vector2> { Vector2.Zero };
            Vector2 offset = Vector2.Zero;

            for (int i = 0; i < angleData.Length; i++)
            {
                var newTilePosition = GetComputedTilePosition(angleData[i]);

                switch (types[i])
                {
                    case TileType.Normal:
                        offset += newTilePosition;
                        break;

                    case TileType.Midspin:
                        offset -= GetComputedTilePosition(GetAvailableAngle(angleData, i));
                        break;

                    case TileType.Short:
                        offset += newTilePosition;
                        break;

                    case TileType.Circular:
                        if (i + 1 < angleData.Length)
                            offset += newTilePosition;

                        break;
                }

                positions.Add(offset);
            }

            return positions;
        }

        /// <summary>
        /// 타일의 정보를 가져옵니다.
        /// </summary>
        /// <param name="beatmap">비트맵.</param>
        /// <returns>각 타일 정보에 대한 집합.</returns>
        public static IReadOnlyList<TileInfo> GetTilesInfo(Beatmap beatmap)
        {
            var convertedAngleData = ConvertAngles(beatmap.AngleData);
            TileInfo[] infos = new TileInfo[convertedAngleData.Length];
            var types = GetTileType(convertedAngleData);
            var tilePositions = GetTilePositions(convertedAngleData);

            for (int floor = 0; floor < convertedAngleData.Length; floor++)
            {
                infos[floor] = new TileInfo
                {
                    TileType = types[floor],
                    Angle = types[floor] == TileType.Midspin ? GetAvailableAngle(convertedAngleData, floor) : convertedAngleData[floor],
                    Position = tilePositions[floor]
                };

                // 타일에 액션들을 추가합니다.
                List<Actions> floorAction = new List<Actions>();

                foreach (var action in beatmap.Actions)
                {
                    if (floor == action.Floor)
                        floorAction.Add(action);
                }

                infos[floor].Action = floorAction.ToArray();
            }

            return infos;
        }

        /// <summary>
        /// 각도 값이 999(미드스핀)일 때 이전의 각도로 수정합니다.
        /// </summary>
        /// <param name="angleData">각도 데이터.</param>
        /// <param name="floor">현재 타일.</param>
        /// <returns>수정된 각도.</returns>
        public static float GetAvailableAngle(float[] angleData, int floor)
        {
            var availableAngle = 0f;

            for (int i = floor - 1; i >= 0; i--)
            {
                if (angleData[i] != 999)
                {
                    availableAngle += angleData[i];
                    break;
                }
                else
                    availableAngle += 180;
            }

            return availableAngle;
        }

        /// <summary>
        /// Adofai에서 사용하는 각도 방향을 우리가 원하는 방향으로 반전합니다.
        /// 마지막 타일이 추가로 있어야 하기때문에 하나 더 추가됩니다.
        /// </summary>
        /// <param name="targetAngleData">Adofai 각도 데이터.</param>
        /// <returns>반전된 값의 각도 데이터.</returns>
        public static float[] ConvertAngles(float[] targetAngleData)
        {
            List<float> newAngleData = new List<float>();

            for (int i = 0; i < targetAngleData.Length; i++)
            {
                if (targetAngleData[i] == 999 || targetAngleData[i] <= 0)
                {
                    if (targetAngleData[i] >= 0)
                        newAngleData.Add(targetAngleData[i]);
                    else
                        newAngleData.Add(newAngleData[i - 1] - 180);
                    continue;
                }

                newAngleData.Add(360 - targetAngleData[i]);
            }

            newAngleData.Add(360 - targetAngleData.Last());

            return newAngleData.ToArray();
        }
    }
}
