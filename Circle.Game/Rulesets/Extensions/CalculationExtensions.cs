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
        public static Vector2 GetComputedTilePosition(float angle)
        {
            var x = (float)Math.Cos(MathHelper.DegreesToRadians(angle)) * 100;
            var y = (float)Math.Sin(MathHelper.DegreesToRadians(angle)) * 100;

            return new Vector2(x, y);
        }

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

        public static IReadOnlyList<double> GetTileHitTime(Beatmap beatmap, double gameplayStartTime)
        {
            var tilesInfo = GetTilesInfo(beatmap).ToArray();
            List<double> hitStartTimes = new List<double> { tilesInfo[0].Angle - 180 };
            double startTimeOffset = gameplayStartTime;
            float bpm = beatmap.Settings.Bpm;
            float prevAngle = GetSafeAngle(tilesInfo[0].Angle - 180);

            for (int floor = 1; floor < tilesInfo.Length; floor++)
            {
                var (fixedRotation, newRotation) = ComputeRotation(tilesInfo, floor, prevAngle);

                prevAngle = newRotation;
                bpm = GetNewBpm(tilesInfo, bpm, floor);
                startTimeOffset += GetRelativeDuration(fixedRotation, newRotation, bpm);
                hitStartTimes.Add(startTimeOffset);
            }

            return hitStartTimes;
        }

        public static (float fixedRotation, float newRotation) ComputeRotation(TileInfo[] tilesInfo, int floor, float prevAngle)
        {
            var currentAngle = GetSafeAngle(tilesInfo[floor].Angle);

            // 소용돌이에 대한 아무런 설정이 없으면 시계방향으로 회전합니다.
            bool isClockwise = GetIsClockwise(tilesInfo, floor + 1);

            float fixedRotation = GetSafeAngle(prevAngle);

            // 현재와 이전 타일이 미드스핀 타일일 때 회전각을 반전하면 안됩니다.
            if (floor > 0)
            {
                if (tilesInfo[floor].TileType != TileType.Midspin && tilesInfo[floor - 1].TileType != TileType.Midspin)
                    fixedRotation -= 180;
                else if (tilesInfo[floor].TileType == TileType.Midspin && tilesInfo[floor - 1].TileType == TileType.Midspin)
                    fixedRotation -= 180;
            }

            if (tilesInfo[floor].TileType == TileType.Midspin)
                return (fixedRotation, fixedRotation);

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

        public static bool GetIsClockwise(TileInfo[] tilesInfo, int floor)
        {
            bool isClockwise = true;

            // 소용돌이를 적용합니다.
            for (int i = 0; i < floor; i++)
            {
                if (tilesInfo[i].Twirl)
                    isClockwise = !isClockwise;
            }

            return isClockwise;
        }

        public static float GetNewBpm(TileInfo[] tilesInfo, float bpm, int floor)
        {
            switch (tilesInfo[floor].SpeedType)
            {
                case SpeedType.Multiplier:
                    return bpm * tilesInfo[floor].BpmMultiplier;

                case SpeedType.Bpm:
                    return tilesInfo[floor].Bpm;

                default:
                    return bpm;
            }
        }

        public static float GetRelativeDuration(float oldRotation, float newRotation, float bpm)
        {
            return 60000 / bpm * Math.Abs(oldRotation - newRotation) / 180;
        }

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

        public static IReadOnlyList<TileInfo> GetTilesInfo(Beatmap beatmap)
        {
            TileInfo[] infos = new TileInfo[beatmap.AngleData.Length];
            var convertedAngleData = ConvertAngles(beatmap.AngleData);
            var types = GetTileType(convertedAngleData);
            var tilePositions = GetTilePositions(convertedAngleData);

            for (int floor = 0; floor < beatmap.AngleData.Length; floor++)
            {
                infos[floor] = new TileInfo
                {
                    TileType = types[floor],
                    Floor = floor,
                    Angle = types[floor] == TileType.Midspin ? GetAvailableAngle(convertedAngleData, floor) : convertedAngleData[floor],
                    Position = tilePositions[floor],
                };
            }

            foreach (var action in beatmap.Actions)
            {
                infos[action.Floor].EventType = action.EventType;

                switch (action.EventType)
                {
                    case EventType.Twirl:
                        infos[action.Floor].Twirl = true;
                        break;

                    case EventType.SetSpeed:
                        switch (action.SpeedType)
                        {
                            case SpeedType.Multiplier:
                                infos[action.Floor].SpeedType = SpeedType.Multiplier;
                                infos[action.Floor].BpmMultiplier = action.BpmMultiplier;
                                break;

                            case SpeedType.Bpm:
                                infos[action.Floor].SpeedType = SpeedType.Bpm;
                                infos[action.Floor].Bpm = action.BeatsPerMinute;
                                break;
                        }

                        break;

                    case EventType.SetPlanetRotation:
                        infos[action.Floor].Easing = action.Ease;
                        break;

                    case EventType.MoveCamera:
                        // Todo: 카메라 기능 추가
                        break;

                    case EventType.Other:
                        break;
                }
            }

            return infos;
        }

        public static float GetAvailableAngle(float[] angleData, int index)
        {
            var availableAngle = 0f;

            for (int i = index - 1; i >= 0; i--)
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
