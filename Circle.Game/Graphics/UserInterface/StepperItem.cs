using System;
using Circle.Game.Configuration;
using Circle.Game.Graphics.Containers;
using osu.Framework;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;

namespace Circle.Game.Graphics.UserInterface
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T">CS0453</typeparam>
    public class StepperItem<T> : Component, IStateful<SelectionState>
    {
        /// <summary>
        /// 값을 올바르게 할당해야합니다. 값이 같은 아이템이 있으면 올바른 작동을 하지 않을 수 있습니다.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// 해당 아이템이 선택되었을 때 실행됩니다.
        /// </summary>
        public Action Action { get; }

        /// <summary>
        /// 임의로 값을 표시할 수 있습니다.
        /// </summary>
        public string Text;

        [Resolved]
        private CircleConfigManager localConfig { get; set; }

        [Resolved]
        private FrameworkConfigManager config { get; set; }

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

        public StepperItem(T value, Action selected = null, string text = null)
        {
            Value = value;
            Action += selected;
            Text = text ?? value.ToString();
        }

        public StepperItem(CircleSetting lookup, T value, string text = null)
        {
            Value = value;
            Action += () => localConfig.SetValue(lookup, value);
            Text = text ?? value.ToString();
        }

        public StepperItem(FrameworkSetting lookup, T value, string text = null)
        {
            Value = value;
            Action += () => config.SetValue(lookup, value);
            Text = text ?? value.ToString();
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
