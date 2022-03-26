using System;

namespace Circle.Game.Graphics.UserInterface
{
    public class EnumStepper<T> : Stepper<T> where T : struct, Enum
    {
        public EnumStepper()
        {
            //Items = (T[])Enum.GetValues(typeof(T));
        }
    }
}
