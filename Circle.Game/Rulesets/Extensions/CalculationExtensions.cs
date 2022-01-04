using System;
using System.Collections.Generic;
using System.Text;
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
    }
}
