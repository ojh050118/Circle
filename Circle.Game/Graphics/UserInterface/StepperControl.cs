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

        private bool allowValueCycling;

        protected abstract StepperControlPanel CreateControlPanel();

        private readonly StepperControlPanel controlPanel;

        private readonly Dictionary<T, StepperControlItem<T>> itemMap = new Dictionary<T, StepperControlItem<T>>();

        protected IEnumerable<StepperControlItem<T>> StepItems => itemMap.Values;

        public IEnumerable<T> Items
        {
            get => StepItems.Select(i => i.Value);
            set
            {
                if (boundItemSource != null)
                    throw new InvalidOperationException($"Cannot manually set {nameof(Items)} when an {nameof(ItemSource)} is bound.");

                setItems(value);
            }
        }

        private readonly IBindableList<T> itemSource = new BindableList<T>();

        /// <summary>
        /// 사용자에게 유효한 아이템입니다. 개발자에 의해 추가된 임의의 아이템이 없다면 null입니다.
        /// </summary>
        private IBindableList<T> boundItemSource;

        /// <summary>
        /// 코드로 임의로 아이템을 추가할 수 있습니다. 실제로 유효한 모든 값입니다.
        /// </summary>
        public IBindableList<T> ItemSource
        {
            get => itemSource;
            set
            {
                ArgumentNullException.ThrowIfNull(value);

                if (boundItemSource != null) itemSource.UnbindFrom(boundItemSource);
                itemSource.BindTo(boundItemSource = value);

                setItems(value);
            }
        }

        private readonly List<T> steps = new List<T>();
        private int selectedIndex => steps.IndexOf(Current.Value);

        private void setItems(IEnumerable<T> value)
        {
            itemMap.Clear();
            steps.Clear();

            if (value == null)
                return;

            foreach (var entry in value)
                AddItem(entry);
        }

        public void AddItem(T value)
        {
            if (itemMap.ContainsKey(value))
                return;

            var item = new StepperControlItem<T>(value, () =>
            {
                if (Enabled.Value)
                    Select(value);
            });

            itemMap[value] = item;
            steps.Add(value);

            UpdateControlState();
        }

        public bool RemoveItem(T value)
        {
            if (value == null)
                return false;

            itemMap.Remove(value);
            steps.Remove(value);

            UpdateControlState();

            return true;
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

            Current.ValueChanged += v => Select(v.NewValue);
            Current.ValueChanged += controlPanel.OnValueChanged;
        }

        protected override void LoadComplete()
        {
            Current.TriggerChange();
            UpdateControlState();
        }

        public bool MoveTo(int step)
        {
            if (step == 0)
                return true;

            if (selectedIndex + step > steps.Count - 1 || selectedIndex + step < 0)
            {
                if (!AllowValueCycling)
                    return false;

                Select(steps[step >= 0 ? step - 1 : steps.Count + selectedIndex + step]);
                return true;
            }

            Select(steps[selectedIndex + step]);

            return true;
        }

        public bool MoveNext() => MoveTo(1);

        public void Reset()
        {
            if (steps.Count == 0)
                return;

            Select(steps[0]);
        }

        public void MovePrevious() => MoveTo(-1);

        public void Select(T value)
        {
            if (!itemMap.ContainsKey(value))
                return;

            if (EqualityComparer<T>.Default.Equals(Current.Value, value))
                return;

            Current.Value = value;

            OnSelectionChanged();
            UpdateControlState();
        }

        protected virtual void UpdateControlState()
        {
            if (itemMap.Count <= 1)
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

            if (selectedIndex == steps.Count - 1)
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
