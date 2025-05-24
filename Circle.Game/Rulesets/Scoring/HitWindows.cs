using System;
using System.Collections.Generic;

namespace Circle.Game.Rulesets.Scoring
{
    public class HitWindows
    {
        private static readonly DifficultyRange[] base_range =
        {
            new DifficultyRange(HitResult.TooEarly, float.MaxValue),
            new DifficultyRange(HitResult.Early, 60),
            new DifficultyRange(HitResult.EPerfect, 45),
            new DifficultyRange(HitResult.Perfect, 30),
            new DifficultyRange(HitResult.LPerfect, 45),
            new DifficultyRange(HitResult.Late, 60),
            new DifficultyRange(HitResult.TooLate, float.MaxValue)
        };

        private double elPerfect;
        private double perfect;
        private double earlyLate;
        private double tooEarlyLate;

        private float speedTrialMultiplier = 1f;

        private readonly bool protectFail;
        //private readonly JudgementRange judgementRange;

        public HitWindows(bool protectFail, JudgementRange range)
        {
            this.protectFail = protectFail;
            //judgementRange = range;

            // TODO: Overload Mechanic 구현
            // 그리고 protect fail은 여기에 불필요함.
            // 판정 범위에 따른 히트 윈도우 범위 구현
        }

        public IEnumerable<HitResult> GetAvailableResults()
        {
            for (var result = HitResult.TooEarly; result <= HitResult.Overload; result++)
            {
                if (IsHitResultAllowed(result))
                    yield return result;
            }
        }

        public bool IsHitResultAllowed(HitResult result)
        {
            if (protectFail && result == HitResult.Overload)
                return false;

            return true;
        }

        public void SetDifficulty(float timingWindowScale, float speedTrialMultiplier = 1f)
        {
            this.speedTrialMultiplier = speedTrialMultiplier;

            foreach (var range in base_range)
            {
                double value = range.Range * timingWindowScale;

                switch (range.Result)
                {
                    case HitResult.EPerfect:
                    case HitResult.LPerfect:
                        elPerfect = value;
                        break;

                    case HitResult.Perfect:
                        perfect = value;
                        break;

                    case HitResult.Early:
                    case HitResult.Late:
                        earlyLate = value;
                        break;

                    case HitResult.TooEarly:
                    case HitResult.TooLate:
                        tooEarlyLate = value;
                        break;
                }
            }
        }

        public HitResult GetResultFor(float bpm, double timeOffset)
        {
            timeOffset = Math.Abs(timeOffset);

            for (var result = HitResult.TooEarly; result <= HitResult.Overload; result++)
            {
                if (IsHitResultAllowed(result) && timeOffset <= WindowFor(result, bpm))
                    return result;
            }

            return HitResult.Miss;
        }

        public double WindowFor(HitResult result, float bpm)
        {
            if (!IsHitResultAllowed(result))
                throw new ArgumentOutOfRangeException(nameof(result), result, $@"{result} is not an allowed result.");

            bpm = 1000 / (bpm * speedTrialMultiplier) / 3;

            switch (result)
            {
                case HitResult.Early:
                case HitResult.Late:
                    return earlyLate * bpm;

                case HitResult.EPerfect:
                case HitResult.LPerfect:
                    return elPerfect * bpm;

                case HitResult.Perfect:
                    return perfect * bpm;

                case HitResult.TooEarly:
                case HitResult.TooLate:
                    return tooEarlyLate * bpm;

                default:
                    return double.MaxValue;
            }
        }

        public struct DifficultyRange
        {
            public readonly HitResult Result;
            public readonly double Range;

            public DifficultyRange(HitResult result, double range)
            {
                Result = result;
                Range = range;
            }
        }
    }
}
