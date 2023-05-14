#nullable disable

using System;

namespace Circle.Game.Screens.Setting
{
    public partial class SettingsEnumDropdown<T> : SettingsDropdown<T> where T : struct, Enum
    {
        public SettingsEnumDropdown()
        {
            Items = (T[])Enum.GetValues(typeof(T));
        }
    }
}
