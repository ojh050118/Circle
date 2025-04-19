using System;
using System.Linq;

namespace Circle.Game.Graphics.UserInterface
{
    public partial class CircleEnumStepperControl<T> : CircleStepperControl<T> where T : struct, Enum
    {
        public CircleEnumStepperControl()
        {
            Items = Enum.GetValues<T>().Select(e => new StepperControlItem<T>(e));
        }
    }
}
