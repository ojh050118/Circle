#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using Circle.Game.Graphics.Containers;
using Circle.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Graphics.UserInterface
{
    /// <summary>
    /// 방향으로 값을 바꿀 수있는 컨트롤.
    /// </summary>
    [Obsolete]
    public partial class Stepper<T> : Container, IHasCurrentValue<T>
    {
        /// <summary>
        /// <see cref="Stepper{T}"/>의 모든 <see cref="StepperItem{T}"/>의 리스트입니다.
        /// </summary>
        public IReadOnlyList<StepperItem<T>> Items;

        private int? selectedIndex;

        private SpriteText text;

        public Stepper()
        {
            // TODO: 로직과 디자인 구현 분리
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            CornerRadius = 5;
        }

        /// <summary>
        /// 이 설정이 무엇인지 표시합니다.
        /// </summary>
        public string LabelText { get; set; }

        public Bindable<T> Current
        {
            get => current.Current;
            set => current.Current = value;
        }

        private readonly BindableWithCurrent<T> current = new BindableWithCurrent<T>();

        public T Selected => (selectedIndex >= 0 && selectedIndex < Items.Count) ? Items[selectedIndex.Value].Value : default;

        [BackgroundDependencyLoader]
        private void load()
        {
            Masking = true;
            RelativeSizeAxes = Axes.X;
            Height = 40;
            Children = new Drawable[]
            {
                new Box
                {
                    Colour = Color4.Black,
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0.2f
                },
                new Container
                {
                    Padding = new MarginPadding { Horizontal = 20 },
                    RelativeSizeAxes = Axes.Both,
                    Child = new GridContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        RowDimensions = new[]
                        {
                            new Dimension(),
                            new Dimension(GridSizeMode.Relative)
                        },
                        Content = new[]
                        {
                            new Drawable[]
                            {
                                new CircleSpriteText
                                {
                                    Text = LabelText,
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Padding = new MarginPadding { Right = 20 },
                                    Font = CircleFont.Default.With(size: 22),
                                    RelativeSizeAxes = Axes.X,
                                    Truncate = true,
                                },
                                new Container
                                {
                                    Anchor = Anchor.CentreRight,
                                    Origin = Anchor.CentreRight,
                                    RelativeSizeAxes = Axes.Both,
                                    Children = new Drawable[]
                                    {
                                        text = new CircleSpriteText
                                        {
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Font = CircleFont.Default.With(size: 22),
                                            Truncate = true,
                                        },
                                        new IconButton
                                        {
                                            Anchor = Anchor.CentreLeft,
                                            Origin = Anchor.CentreLeft,
                                            Icon = FontAwesome.Solid.AngleLeft,
                                            Action = SelectPrevious,
                                            Size = new Vector2(30)
                                        },
                                        new IconButton
                                        {
                                            Anchor = Anchor.CentreRight,
                                            Origin = Anchor.CentreRight,
                                            Icon = FontAwesome.Solid.AngleRight,
                                            Action = SelectNext,
                                            Size = new Vector2(30)
                                        },
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Select(Current.Value);
        }

        public void Select(T value)
        {
            var item = Items?.FirstOrDefault(v => EqualityComparer<T>.Default.Equals(Current.Value, value));
            if (item == null)
                return;

            int newIndex = Items.ToList().IndexOf(item);

            if (newIndex < 0)
                setSelected(null);
            else
                setSelected(newIndex);
        }

        public void SelectPrevious()
        {
            if (!selectedIndex.HasValue || selectedIndex == 0)
                setSelected(Items?.Count - 1);
            else
                setSelected(selectedIndex - 1);
        }

        public void SelectNext()
        {
            if (!selectedIndex.HasValue || selectedIndex == Items.Count - 1)
                setSelected(0);
            else
                setSelected(selectedIndex + 1);
        }

        private void setSelected(int? index)
        {
            if (selectedIndex == index || Items == null)
                return;

            if (selectedIndex.HasValue)
                Items[selectedIndex.Value].State = SelectionState.NotSelected;

            selectedIndex = index;

            if (selectedIndex != null)
            {
                Current.Value = Items[selectedIndex.Value].Value;

                Items[selectedIndex.Value].State = SelectionState.Selected;
                text.Text = Items[selectedIndex.Value].Text;
            }
        }
    }
}
