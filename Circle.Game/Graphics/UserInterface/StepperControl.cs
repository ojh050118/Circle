#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Localisation;

namespace Circle.Game.Graphics.UserInterface
{
    [Cached(typeof(IStepperControl))]
    public abstract partial class StepperControl<T> : CompositeDrawable, IStepperControl, IHasCurrentValue<T>
    {
        public Bindable<T> Current
        {
            get => current.Current;
            set => current.Current = value;
        }

        private readonly BindableWithCurrent<T> current = new BindableWithCurrent<T>();

        public BindableBool Enabled = new BindableBool(true);

        public bool AllowControlCycling;

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

        private void setItems(IEnumerable<T> value)
        {
            itemMap.Clear();

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
        }

        protected Container Background;
        protected Container Foreground;

        protected abstract LocalisableString ValueText { get; set; }

        public event Action maxValueReached;
        public event Action minValueReached;

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

        public abstract void MoveNext();
        public abstract void MovePrevious();

        public void Select(T value)
        {
            if (!itemMap.ContainsKey(value))
                return;

            if (EqualityComparer<T>.Default.Equals(Current.Value, value))
                return;

            Current.Value = value;
            SelectionChanged?.Invoke();

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

            var rangeState = CheckValueRangeReached();

            // TODO: AllowControlCycling 값을 변경하럐도 이 메서드 호출
            if (AllowControlCycling)
                return;

            if (rangeState == ValueRangeReachedState.Minimum)
            {
                minValueReached?.Invoke();
                controlPanel.PreviousEnabled.Value = false;
            }
            else if (rangeState == ValueRangeReachedState.Maximum)
            {
                maxValueReached?.Invoke();
                controlPanel.NextEnabled.Value = false;
            }
            else
            {
                controlPanel.PreviousEnabled.Value = true;
                controlPanel.NextEnabled.Value = true;
            }
        }

        protected abstract ValueRangeReachedState CheckValueRangeReached();

        public enum ValueRangeReachedState
        {
            None,
            Maximum,
            Minimum
        }
    }
}
