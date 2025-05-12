#nullable disable

using System;
using Circle.Game.Beatmaps;
using Circle.Game.Rulesets.Objects;
using JetBrains.Annotations;
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
        public static Vector2 GetNextTilePosition(float angle)
        {
            float x = (float)Math.Cos(MathHelper.DegreesToRadians(angle)) * (DrawableTile.WIDTH - DrawableTile.HEIGHT);
            float y = (float)Math.Sin(MathHelper.DegreesToRadians(angle)) * (DrawableTile.WIDTH - DrawableTile.HEIGHT);

            return new Vector2(x, y);
        }

        /// <summary>
        /// 0~360도 범위사이의 각도로 변환 합니다.
        /// 각도값이 매우 커지거나 작아지는 것을 방지하는데 사용합니다.
        /// </summary>
        /// <param name="angle">각도.</param>
        /// <returns>0~360도 범위사이의 각도</returns>
        public static float NormalizeAngle(float angle)
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
        /// 각도를 반대방향으로 반전합니다.
        /// </summary>
        /// <param name="angle">각도.</param>
        /// <returns>반전된 0~360도 사이의 각도.</returns>
        public static float InvertAngle(float angle) => NormalizeAngle(angle + 180);

        /// <summary>
        /// 현재 타일의 방향으로 회전할 행성의 시작 각도를 계산합니다.
        /// 일반적으로 각도를 반전합니다.
        /// </summary>
        /// <param name="prevType">이전 타일의 타입</param>
        /// <param name="prevAngle">이전 타일의 각도</param>
        /// <param name="targetType">현재 타일의 타입</param>
        /// <param name="targetAngle">현재 타일의 각도</param>
        /// <param name="clockwise">현재 타일의 회전방향</param>
        /// <returns>회전방향을 고려한 시작 각도 값.</returns>
        public static float ComputeStartRotation(TileType prevType, float prevAngle, TileType targetType, float targetAngle, bool clockwise)
        {
            float destRotation = NormalizeAngle(targetAngle);
            float startRotation = NormalizeAngle(prevAngle);

            // 각도를 이전 타일의 반대 방향으로 설정합니다.
            // 미드스핀 타일에 행성이 머물지 않기때문에, 각도를 변경하지 않습니다.
            if (targetType != TileType.Midspin && prevType != TileType.Midspin)
                startRotation = InvertAngle(startRotation);
            else if (targetType == TileType.Midspin && prevType == TileType.Midspin) // TODO: 이전과 현재가 미드스핀일 때 예상치 못한 문제가 생기는 듯. 타일 정보에 미드스핀 타일은 이전 각도의 값을 가지고 있도록 하기.
                startRotation += InvertAngle(startRotation);

            if (targetType == TileType.Midspin)
                return startRotation;

            // 올바른 방향으로 회전할 수 있도록 합니다.
            if (clockwise)
            {
                while (startRotation >= destRotation)
                    startRotation -= 360;
            }
            else
            {
                while (startRotation <= destRotation)
                    startRotation += 360;
            }

            return startRotation;
        }

        /// <summary>
        /// 타일의 bpm을 적용합니다.
        /// </summary>
        /// <param name="bpm">현재 bpm.</param>
        /// <param name="actionEvent">이벤트 타입이 SetSpeed인 액션.</param>
        /// <returns>현재bpm에서 승수가 적용된 값 또는 새로운 bpm.</returns>
        public static float GetNewBpm(float bpm, ActionEvents actionEvent)
        {
            switch (actionEvent.SpeedType)
            {
                case SpeedType.Multiplier:
                    return bpm * actionEvent.BpmMultiplier;

                case SpeedType.Bpm:
                    return actionEvent.BeatsPerMinute;

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
        /// 시간 차로 회전할 수 있는 각도의 양을 구합니다.
        /// </summary>
        /// <param name="startTime">회전을 시작할 시간.</param>
        /// <param name="endTime">회전이 끝나는 시간.</param>
        /// <param name="bpm">bpm.</param>
        /// <returns>회전가능한 각도.</returns>
        public static float GetTimeBasedRotation(double startTime, double endTime, float bpm)
        {
            return (float)Math.Abs(endTime - startTime) / (60000 / bpm) * 180;
        }

        /// <summary>
        /// 타일의 종류를 구합니다.
        /// 미드스핀 타일은 <see cref="TileType.Midspin"/>, 라운드 타일과 마지막 타일은 <see cref="TileType.Circular"/>,
        /// 미드스핀과 라운드 타일의 이전 타일은 <see cref="TileType.Short"/>, 그 외는 <see cref="TileType.Normal"/>입니다.
        /// </summary>
        /// <param name="prevAngle">이전 타일 각도.</param>
        /// <param name="targetAngle">종류를 구할 타일의 각도.</param>
        /// <param name="nextAngle">다음 타일 각도.</param>
        /// <returns>타일의 종류</returns>
        public static TileType GetTileType(float prevAngle, float targetAngle, float? nextAngle = null)
        {
            // 각도값이 999인 타일은 미드스핀 타일이며, 미드스핀의 타일 각도는 앞 타일이 결정합니다.
            if (targetAngle == 999)
                return TileType.Midspin;

            // 다음 타일이 없으면 원형 타일입니다.
            // 이전타일과 반대방향이면 원형 타일입니다.
            if (!nextAngle.HasValue || Math.Abs(targetAngle - prevAngle) == 180)
                return TileType.Circular;

            // 미드스핀 타일의 앞 타일은 항상 길이가 짧은 타일입니다.
            // 라운드 타일의 앞 타일은 항상 길이가 짧은 타일입니다.
            if (nextAngle.Value == 999 || Math.Abs(nextAngle.Value - targetAngle) == 180)
                return TileType.Short;

            return TileType.Normal;
        }

        /// <summary>
        /// Adofai에서 사용하는 각도 방향을 우리가 원하는 방향으로 반전합니다.
        /// 마지막 타일이 추가로 있어야 하기때문에 하나 더 추가됩니다.
        /// </summary>
        /// <param name="targetAngleData">Adofai 각도 데이터.</param>
        /// <returns>반전된 값의 각도 데이터.</returns>
        public static float[] ConvertAngles(float[] targetAngleData)
        {
            float[] convertedData = new float[targetAngleData.Length + 1];

            for (int i = 0; i < targetAngleData.Length; i++)
            {
                if (targetAngleData[i] == 999)
                {
                    convertedData[i] = targetAngleData[i];
                    continue;
                }

                convertedData[i] = NormalizeAngle(targetAngleData[i] * -1);
            }

            convertedData[^1] = convertedData[^2];
            return convertedData;
        }

        public static Vector2 ToVector2(this float?[] arr)
        {
            if (arr == null)
                return Vector2.Zero;

            return new Vector2(-arr[0] ?? 0, arr[1] ?? 0);
        }

        public static Vector2 ToVector2([CanBeNull] this float[] arr)
        {
            if (arr != null)
                return new Vector2(-arr[0], arr[1]);

            return Vector2.Zero;
        }
    }
}
