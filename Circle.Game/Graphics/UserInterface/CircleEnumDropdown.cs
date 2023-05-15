#nullable disable

using System;

namespace Circle.Game.Graphics.UserInterface
{
    public partial class CircleEnumDropdown<T> : CircleDropdown<T> where T : struct, Enum
    {
        public CircleEnumDropdown()
        {
            Items = (T[])Enum.GetValues(typeof(T));
        }
    }
}

