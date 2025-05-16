using System;
using System.Collections.Generic;
using Circle.Game.Beatmaps;
using Circle.Game.Converting.Adofai.Elements;

namespace Circle.Game.Converting
{
    public static class AdofaiParser
    {
        public static Toggle ParseToggle(string toggle)
        {
            foreach (Toggle parsed in Enum.GetValues(typeof(Toggle)))
            {
                if (toggle == parsed.ToString())
                    return parsed;
            }

            return Toggle.Disabled;
        }

        public static Ease ParseEase(string ease)
        {
            foreach (Ease adofaiEase in Enum.GetValues(typeof(Ease)))
            {
                if (ease == adofaiEase.ToString())
                    return adofaiEase;
            }

            return Ease.Unset;
        }

        public static Relativity ParseRelativity(string relativeTo)
        {
            if (Enum.TryParse(relativeTo, out Relativity relativity))
                return relativity;

            return Relativity.Player;
        }

        public static float[] ParseAngleData(string pathData)
        {
            List<float> angleData = new List<float>();

            foreach (char angle in pathData)
            {
                switch (angle)
                {
                    case 'R':
                        angleData.Add(0);
                        break;

                    case 'U':
                        angleData.Add(90);
                        break;

                    case 'D':
                        angleData.Add(270);
                        break;

                    case 'L':
                        angleData.Add(180);
                        break;

                    case 'J':
                        angleData.Add(30);
                        break;

                    case 'T':
                        angleData.Add(60);
                        break;

                    case 'M':
                        angleData.Add(330);
                        break;

                    case 'B':
                        angleData.Add(300);
                        break;

                    case 'F':
                        angleData.Add(240);
                        break;

                    case 'N':
                        angleData.Add(210);
                        break;

                    case 'H':
                        angleData.Add(150);
                        break;

                    case 'G':
                        angleData.Add(120);
                        break;

                    case 'Q':
                        angleData.Add(135);
                        break;

                    case 'E':
                        angleData.Add(45);
                        break;

                    case 'C':
                        angleData.Add(315);
                        break;

                    case 'Z':
                        angleData.Add(225);
                        break;

                    case '!':
                        angleData.Add(999);
                        break;

                    case 'p':
                        angleData.Add(15);
                        break;

                    case 'o':
                        angleData.Add(75);
                        break;

                    case 'q':
                        angleData.Add(105);
                        break;

                    case 'W':
                        angleData.Add(165);
                        break;

                    case 'x':
                        angleData.Add(195);
                        break;

                    case 'V':
                        angleData.Add(255);
                        break;

                    case 'Y':
                        angleData.Add(285);
                        break;

                    case 'A':
                        angleData.Add(345);
                        break;

                    // 처리 할 수 없는 타일.
                    default:
                        angleData.Add(0);
                        break;
                }
            }

            return angleData.ToArray();
        }
    }
}
