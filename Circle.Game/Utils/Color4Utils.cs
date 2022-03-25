using System;
using osuTK.Graphics;

namespace Circle.Game.Utils
{
    public static class Color4Utils
    {
        public static Color4 GetColor4(Color4Enum color)
        {
            switch (color)
            {
                case Color4Enum.AliceBlue:
                    return Color4.AliceBlue;

                case Color4Enum.AntiqueWhite:
                    return Color4.AntiqueWhite;

                case Color4Enum.Aqua:
                    return Color4.Aqua;

                case Color4Enum.Aquamarine:
                    return Color4.Aquamarine;

                case Color4Enum.Azure:
                    return Color4.Azure;

                case Color4Enum.Beige:
                    return Color4.Beige;

                case Color4Enum.Bisque:
                    return Color4.Bisque;

                case Color4Enum.Black:
                    return Color4.Black;

                case Color4Enum.BlanchedAlmond:
                    return Color4.BlanchedAlmond;

                case Color4Enum.Blue:
                    return Color4.Blue;

                case Color4Enum.BlueViolet:
                    return Color4.BlueViolet;

                case Color4Enum.Brown:
                    return Color4.Brown;

                case Color4Enum.BurlyWood:
                    return Color4.BurlyWood;

                case Color4Enum.CadetBlue:
                    return Color4.CadetBlue;

                case Color4Enum.Chartreuse:
                    return Color4.Chartreuse;

                case Color4Enum.Chocolate:
                    return Color4.Chocolate;

                case Color4Enum.Coral:
                    return Color4.Coral;

                case Color4Enum.CornflowerBlue:
                    return Color4.CornflowerBlue;

                case Color4Enum.Cornsilk:
                    return Color4.Cornsilk;

                case Color4Enum.Crimson:
                    return Color4.Crimson;

                case Color4Enum.Cyan:
                    return Color4.Cyan;

                case Color4Enum.DarkBlue:
                    return Color4.DarkBlue;

                case Color4Enum.DarkCyan:
                    return Color4.DarkCyan;

                case Color4Enum.DarkGoldenrod:
                    return Color4.DarkGoldenrod;

                case Color4Enum.DarkGray:
                    return Color4.DarkGray;

                case Color4Enum.DarkGreen:
                    return Color4.DarkGreen;

                case Color4Enum.DarkKhaki:
                    return Color4.DarkKhaki;

                case Color4Enum.DarkMagenta:
                    return Color4.DarkMagenta;

                case Color4Enum.DarkOliveGreen:
                    return Color4.DarkOliveGreen;

                case Color4Enum.DarkOrange:
                    return Color4.DarkOrange;

                case Color4Enum.DarkOrchid:
                    return Color4.DarkOrchid;

                case Color4Enum.DarkRed:
                    return Color4.DarkRed;

                case Color4Enum.DarkSalmon:
                    return Color4.DarkSalmon;

                case Color4Enum.DarkSeaGreen:
                    return Color4.DarkSeaGreen;

                case Color4Enum.DarkSlateBlue:
                    return Color4.DarkSlateBlue;

                case Color4Enum.DarkSlateGray:
                    return Color4.DarkSlateGray;

                case Color4Enum.DarkTurquoise:
                    return Color4.DarkTurquoise;

                case Color4Enum.DarkViolet:
                    return Color4.DarkViolet;

                case Color4Enum.DeepPink:
                    return Color4.DeepPink;

                case Color4Enum.DeepSkyBlue:
                    return Color4.DeepSkyBlue;

                case Color4Enum.DimGray:
                    return Color4.DimGray;

                case Color4Enum.DodgerBlue:
                    return Color4.DodgerBlue;

                case Color4Enum.Firebrick:
                    return Color4.Firebrick;

                case Color4Enum.FloralWhite:
                    return Color4.FloralWhite;

                case Color4Enum.ForestGreen:
                    return Color4.ForestGreen;

                case Color4Enum.Fuchsia:
                    return Color4.Fuchsia;

                case Color4Enum.Gainsboro:
                    return Color4.Gainsboro;

                case Color4Enum.GhostWhite:
                    return Color4.GhostWhite;

                case Color4Enum.Gold:
                    return Color4.Gold;

                case Color4Enum.Goldenrod:
                    return Color4.Goldenrod;

                case Color4Enum.Gray:
                    return Color4.Gray;

                case Color4Enum.Green:
                    return Color4.Green;

                case Color4Enum.GreenYellow:
                    return Color4.GreenYellow;

                case Color4Enum.Honeydew:
                    return Color4.Honeydew;

                case Color4Enum.HotPink:
                    return Color4.HotPink;

                case Color4Enum.IndianRed:
                    return Color4.IndianRed;

                case Color4Enum.Indigo:
                    return Color4.Indigo;

                case Color4Enum.Ivory:
                    return Color4.Ivory;

                case Color4Enum.Khaki:
                    return Color4.Khaki;

                case Color4Enum.Lavender:
                    return Color4.Lavender;

                case Color4Enum.LavenderBlush:
                    return Color4.LavenderBlush;

                case Color4Enum.LawnGreen:
                    return Color4.LawnGreen;

                case Color4Enum.LemonChiffon:
                    return Color4.LemonChiffon;

                case Color4Enum.LightBlue:
                    return Color4.LightBlue;

                case Color4Enum.LightCoral:
                    return Color4.LightCoral;

                case Color4Enum.LightCyan:
                    return Color4.LightCyan;

                case Color4Enum.LightGoldenrodYellow:
                    return Color4.LightGoldenrodYellow;

                case Color4Enum.LightGray:
                    return Color4.LightGray;

                case Color4Enum.LightGreen:
                    return Color4.LightGreen;

                case Color4Enum.LightPink:
                    return Color4.LightPink;

                case Color4Enum.LightSalmon:
                    return Color4.LightSalmon;

                case Color4Enum.LightSeaGreen:
                    return Color4.LightSeaGreen;

                case Color4Enum.LightSkyBlue:
                    return Color4.LightSkyBlue;

                case Color4Enum.LightSlateGray:
                    return Color4.LightSlateGray;

                case Color4Enum.LightSteelBlue:
                    return Color4.LightSteelBlue;

                case Color4Enum.LightYellow:
                    return Color4.LightYellow;

                case Color4Enum.Lime:
                    return Color4.Lime;

                case Color4Enum.LimeGreen:
                    return Color4.LimeGreen;

                case Color4Enum.Linen:
                    return Color4.Linen;

                case Color4Enum.Magenta:
                    return Color4.Magenta;

                case Color4Enum.Maroon:
                    return Color4.Maroon;

                case Color4Enum.MediumAquamarine:
                    return Color4.MediumAquamarine;

                case Color4Enum.MediumBlue:
                    return Color4.MediumBlue;

                case Color4Enum.MediumOrchid:
                    return Color4.MediumOrchid;

                case Color4Enum.MediumPurple:
                    return Color4.MediumPurple;

                case Color4Enum.MediumSeaGreen:
                    return Color4.MediumSeaGreen;

                case Color4Enum.MediumSlateBlue:
                    return Color4.MediumSlateBlue;

                case Color4Enum.MediumSpringGreen:
                    return Color4.MediumSpringGreen;

                case Color4Enum.MediumTurquoise:
                    return Color4.MediumTurquoise;

                case Color4Enum.MediumVioletRed:
                    return Color4.MediumVioletRed;

                case Color4Enum.MidnightBlue:
                    return Color4.MidnightBlue;

                case Color4Enum.MintCream:
                    return Color4.MintCream;

                case Color4Enum.MistyRose:
                    return Color4.MistyRose;

                case Color4Enum.Moccasin:
                    return Color4.Moccasin;

                case Color4Enum.NavajoWhite:
                    return Color4.NavajoWhite;

                case Color4Enum.Navy:
                    return Color4.Navy;

                case Color4Enum.OldLace:
                    return Color4.OldLace;

                case Color4Enum.Olive:
                    return Color4.Olive;

                case Color4Enum.OliveDrab:
                    return Color4.OliveDrab;

                case Color4Enum.Orange:
                    return Color4.Orange;

                case Color4Enum.OrangeRed:
                    return Color4.OrangeRed;

                case Color4Enum.Orchid:
                    return Color4.Orchid;

                case Color4Enum.PaleGoldenrod:
                    return Color4.PaleGoldenrod;

                case Color4Enum.PaleGreen:
                    return Color4.PaleGreen;

                case Color4Enum.PaleTurquoise:
                    return Color4.PaleTurquoise;

                case Color4Enum.PaleVioletRed:
                    return Color4.PaleVioletRed;

                case Color4Enum.PapayaWhip:
                    return Color4.PapayaWhip;

                case Color4Enum.PeachPuff:
                    return Color4.PeachPuff;

                case Color4Enum.Peru:
                    return Color4.Peru;

                case Color4Enum.Pink:
                    return Color4.Pink;

                case Color4Enum.Plum:
                    return Color4.Plum;

                case Color4Enum.PowderBlue:
                    return Color4.PowderBlue;

                case Color4Enum.Purple:
                    return Color4.Purple;

                case Color4Enum.Red:
                    return Color4.Red;

                case Color4Enum.RosyBrown:
                    return Color4.RosyBrown;

                case Color4Enum.RoyalBlue:
                    return Color4.RoyalBlue;

                case Color4Enum.SaddleBrown:
                    return Color4.SaddleBrown;

                case Color4Enum.Salmon:
                    return Color4.Salmon;

                case Color4Enum.SandyBrown:
                    return Color4.SandyBrown;

                case Color4Enum.SeaGreen:
                    return Color4.SeaGreen;

                case Color4Enum.SeaShell:
                    return Color4.SeaShell;

                case Color4Enum.Sienna:
                    return Color4.Sienna;

                case Color4Enum.Silver:
                    return Color4.Silver;

                case Color4Enum.SkyBlue:
                    return Color4.SkyBlue;

                case Color4Enum.SlateBlue:
                    return Color4.SlateBlue;

                case Color4Enum.SlateGray:
                    return Color4.SlateGray;

                case Color4Enum.Snow:
                    return Color4.Snow;

                case Color4Enum.SpringGreen:
                    return Color4.SpringGreen;

                case Color4Enum.SteelBlue:
                    return Color4.SteelBlue;

                case Color4Enum.Tan:
                    return Color4.Tan;

                case Color4Enum.Teal:
                    return Color4.Teal;

                case Color4Enum.Thistle:
                    return Color4.Thistle;

                case Color4Enum.Tomato:
                    return Color4.Tomato;

                case Color4Enum.Transparent:
                    return Color4.Transparent;

                case Color4Enum.Turquoise:
                    return Color4.Turquoise;

                case Color4Enum.Violet:
                    return Color4.Violet;

                case Color4Enum.Wheat:
                    return Color4.Wheat;

                case Color4Enum.White:
                    return Color4.White;

                case Color4Enum.WhiteSmoke:
                    return Color4.WhiteSmoke;

                case Color4Enum.Yellow:
                    return Color4.Yellow;

                case Color4Enum.YellowGreen:
                    return Color4.YellowGreen;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
