using System;
using Circle.Game.Graphics.Containers;
using osu.Framework;

namespace Circle.Game.Graphics.UserInterface
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T">CS0453</typeparam>
    public class StepperItem<T> : IStateful<SelectionState>
    {
        public T Value { get; set; }

        /// <summary>
        /// 해당 아이템이 선택되었을 때 실행됩니다.
        /// </summary>
        public Action Action { get; }

        /// <summary>
        /// <see cref="Stepper{T}"/>에 표시할 텍스트 입니다.
        /// </summary>
        public string Text { get; set; }

        private SelectionState state;

        public SelectionState State
        {
            get => state;
            set
            {
                if (state == value)
                    return;

                state = value;
                stateChanged(value);
                StateChanged?.Invoke(value);
            }
        }

        public event Action<SelectionState> StateChanged;

        public StepperItem(T value, Action selected = null)
        {
            Value = value;
            Action += selected;
            Text = value.ToString();
        }

        public StepperItem(string text, T value, Action selected = null)
        {
            Text = text;
            Value = value;
            Action += selected;
        }

        private void stateChanged(SelectionState state)
        {
            switch (state)
            {
                case SelectionState.NotSelected:
                    break;

                case SelectionState.Selected:
                    Action?.Invoke();
                    break;
            }
        }
    }
}
