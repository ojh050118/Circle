#nullable disable

using System;
using System.Collections.Generic;

namespace Circle.Game.Graphics.UserInterface
{
    public partial class EnumStepper<T> : Stepper<T> where T : struct, Enum
    {
        public EnumStepper()
        {
            List<StepperItem<T>> items = new List<StepperItem<T>>();

            foreach (T e in Enum.GetValues(typeof(T)))
            {
                items.Add(new StepperItem<T>(e));
            }

            Items = items;
        }
    }
}
