#nullable disable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;

namespace Circle.Game.Graphics.UserInterface
{
    [Cached(typeof(IStepperControl))]
    public abstract partial class StepperControl<T> : CompositeDrawable, IStepperControl, IHasCurrentValue<T>, IEnumerator<T>
    {
        public Bindable<T> Current
        {
            get => current.Current;
            set => current.Current = value;
        }

        private readonly BindableWithCurrent<T> current = new BindableWithCurrent<T>();

        protected Bindable<StepperControlItem<T>> CurrentItem = new Bindable<StepperControlItem<T>>();

        public BindableBool Enabled = new BindableBool(true);

        public bool AllowValueCycling
        {
            get => allowValueCycling;
            set
            {
                if (allowValueCycling == value)
                    return;

                allowValueCycling = value;
                UpdateControlState();
            }
        }

        private bool allowValueCycling = true;

        protected abstract StepperControlPanel CreateControlPanel();

        private readonly StepperControlPanel controlPanel;

        private readonly Dictionary<T, StepperControlItem<T>> itemMap = new Dictionary<T, StepperControlItem<T>>();

        public IEnumerable<StepperControlItem<T>> Items
        {
            get => items;
            set => setItems(value);
        }

        private readonly List<StepperControlItem<T>> items = new List<StepperControlItem<T>>();

        private int selectedIndex => items.IndexOf(itemMap[Current.Value]);

        private void setItems(IEnumerable<StepperControlItem<T>> value)
        {
            itemMap.Clear();
            items.Clear();

            if (value == null)
                return;

            // AddItem()을 통해 하나씩 추가하면 처음 추가된 값과 현재 설정 값이 일치하지않아 UpdateControlState()에서 Current 값을 수정합니다.
            // 설절 값을 강제로 바꿀 수 있고, 잠재적으로 게임이 크래시가 발생할 가능성이 있습니다.
            // 문제를 피하기위해, 아이템을 한꺼번에 추가될 때는 UpdateControlState()를 마지막에 실행합니다.
            foreach (var entry in value)
            {
                itemMap[entry.Value] = entry;
                items.Add(entry);
            }

            UpdateControlState();
        }

        public void AddItem(StepperControlItem<T> value)
        {
            if (items.Contains(value))
                return;

            itemMap[value.Value] = value;
            items.Add(value);

            UpdateControlState();
        }

        public bool RemoveItem(T value)
        {
            if (value == null || !itemMap.TryGetValue(value, out var item))
                return false;

            return RemoveItem(item);
        }

        public bool RemoveItem(StepperControlItem<T> value)
        {
            if (value == null)
                return false;

            // Current값과 동일한 값을 삭제하면 바로 앞의 값으로 설정하려 시도합니다.
            if (CurrentItem.Value == value)
                MovePrevious();

            bool removed = itemMap.Remove(value.Value) & items.Remove(value);
            UpdateControlState();

            return removed;
        }

        public void ClearItems() => setItems(null);

        protected Container Background;
        protected Container Foreground;

        public event Action SelectionChanged;

        protected StepperControl()
        {
            InternalChildren = new Drawable[]
            {
                Background = new Container { RelativeSizeAxes = Axes.Both },
                Foreground = new Container { RelativeSizeAxes = Axes.Both },
                controlPanel = CreateControlPanel()
            };
        }

        private void currentValueChanged(ValueChangedEvent<T> e)
        {
            var oldItem = CurrentItem.Value;

            // CurrentItem의 값을 설정합니다.
            // Current가 값 범위에 없는 경우는 item이 비어있거나, 값 범위 내의 값으로 설정되지 않은 것입니다.
            if (itemMap.TryGetValue(Current.Value, out var item))
                CurrentItem.Value = item;
            else
                CurrentItem.Value = null;

            OnSelectionChanged();

            controlPanel.OnValueChanged(new ValueChangedEvent<StepperControlItem<T>>(oldItem, CurrentItem.Value));
        }

        protected override void LoadComplete()
        {
            Current.ValueChanged += currentValueChanged;
            Current.TriggerChange();
        }

        public bool MoveTo(int step)
        {
            if (step == 0)
                return true;

            if (selectedIndex + step > items.Count - 1 || selectedIndex + step < 0)
            {
                if (!AllowValueCycling)
                    return false;

                Select(items[step >= 0 ? step - 1 : items.Count + selectedIndex + step]);
                return true;
            }

            Select(items[selectedIndex + step]);

            return true;
        }

        public bool MoveNext() => MoveTo(1);

        public void Reset()
        {
            if (items.Count == 0)
                return;

            Select(items[0]);
        }

        public void MovePrevious() => MoveTo(-1);

        public void Select(T value) => Select(itemMap[value]);

        protected void Select(StepperControlItem<T> item)
        {
            if (!items.Contains(item))
                return;

            if (EqualityComparer<StepperControlItem<T>>.Default.Equals(CurrentItem.Value, item))
                return;

            Current.Value = item.Value;
            UpdateControlState();
        }

        protected virtual void UpdateControlState()
        {
            // 만약 Current가 값 범위에 들지 않았다면, 범위값중 최소값으로 설정합니다.
            if (!itemMap.ContainsKey(Current.Value))
                Current.Value = itemMap.FirstOrDefault().Key;

            if (items.Count <= 1)
            {
                controlPanel.NextEnabled.Value = false;
                controlPanel.PreviousEnabled.Value = false;
                return;
            }

            if (AllowValueCycling)
            {
                controlPanel.NextEnabled.Value = true;
                controlPanel.PreviousEnabled.Value = true;
                return;
            }

            var rangeState = CheckValueRangeReached();

            if (rangeState == ValueRangeReachedState.Minimum)
            {
                controlPanel.PreviousEnabled.Value = false;
                controlPanel.NextEnabled.Value = true;
            }
            else if (rangeState == ValueRangeReachedState.Maximum)
            {
                controlPanel.NextEnabled.Value = false;
                controlPanel.PreviousEnabled.Value = true;
            }
            else
            {
                controlPanel.PreviousEnabled.Value = true;
                controlPanel.NextEnabled.Value = true;
            }
        }

        protected virtual void OnSelectionChanged()
        {
            SelectionChanged?.Invoke();
        }

        protected virtual ValueRangeReachedState CheckValueRangeReached()
        {
            if (selectedIndex == 0)
                return ValueRangeReachedState.Minimum;

            if (selectedIndex == items.Count - 1)
                return ValueRangeReachedState.Maximum;

            return ValueRangeReachedState.None;
        }

        #region IEnumerator

        T IEnumerator<T>.Current => Current.Value;

        object IEnumerator.Current => Current.Value;

        bool IEnumerator.MoveNext() => MoveNext();

        #endregion

        public enum ValueRangeReachedState
        {
            None,
            Maximum,
            Minimum
        }
    }
}
