namespace Circle.Game.Graphics.UserInterface
{
    public interface IStepperControl
    {
        bool MoveNext();
        void MovePrevious();
        void Reset();
    }
}
