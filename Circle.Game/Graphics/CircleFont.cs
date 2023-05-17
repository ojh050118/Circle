#nullable disable

using osu.Framework.Graphics.Sprites;

namespace Circle.Game.Graphics
{
    public class CircleFont
    {
        public const float DEFAULT_FONT_SIZE = 24;

        public static FontUsage Default => GetFont();

        public static FontUsage GetFont(Typeface typeface = Typeface.OpenSans, float size = DEFAULT_FONT_SIZE, FontWeight weight = FontWeight.Medium, bool italics = false, bool fixedWidth = false)
        {
            string familyString = GetFamilyString(typeface);
            return new FontUsage(familyString, size, GetWeightString(familyString, weight), getItalics(italics), fixedWidth);
        }

        private static bool getItalics(in bool italicsRequested)
        {
            return false;
        }

        public static string GetFamilyString(Typeface typeface)
        {
            switch (typeface)
            {
                case Typeface.OpenSans:
                    return @"OpenSans";
            }

            return null;
        }

        public static string GetWeightString(string family, FontWeight weight)
        {
            return weight.ToString();
        }
    }

    public static class TetrisFontExtensions
    {
        public static FontUsage With(this FontUsage usage, Typeface? typeface = null, float? size = null, FontWeight? weight = null, bool? italics = null, bool? fixedWidth = null)
        {
            string familyString = typeface != null ? CircleFont.GetFamilyString(typeface.Value) : usage.Family;
            string weightString = weight != null ? CircleFont.GetWeightString(familyString, weight.Value) : usage.Weight;

            return usage.With(familyString, size, weightString, italics, fixedWidth);
        }
    }

    public enum Typeface
    {
        OpenSans
    }

    public enum FontWeight
    {
        Light = 300,
        Regular = 400,
        Medium = 500,
        SemiBold = 600,
        Bold = 700,
        Black = 900
    }
}
