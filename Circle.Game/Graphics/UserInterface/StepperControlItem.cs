using System;
using osu.Framework.Graphics.UserInterface;

namespace Circle.Game.Graphics.UserInterface
{
    public class StepperControlItem<T> : MenuItem
    {
        public readonly T Value;

        public StepperControlItem(string text, T value)
            : base(text)
        {
            Value = value;
        }

        public StepperControlItem(T value, Action action)
            : base(action)
        {
            Value = value;
        }
    }
}
