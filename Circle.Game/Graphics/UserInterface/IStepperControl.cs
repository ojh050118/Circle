using System;

namespace Circle.Game.Graphics.UserInterface
{
    public interface IStepperControl
    {
        event Action maxValueReached;
        event Action minValueReached;

        void MoveNext();
        void MovePrevious();
    }
}
