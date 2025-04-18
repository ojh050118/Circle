using System;

namespace Circle.Game.Graphics.UserInterface
{
    public partial class CircleEnumStepperControl<T> : CircleStepperControl<T> where T : struct, Enum
    {
        public CircleEnumStepperControl()
        {
            Items = (T[])Enum.GetValues(typeof(T));
        }
    }
}
