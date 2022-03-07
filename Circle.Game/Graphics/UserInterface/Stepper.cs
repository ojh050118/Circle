using System.Linq;
using Circle.Game.Configuration;
using Circle.Game.Graphics.Containers;
using osu.Framework.Allocation;
using osu.Framework.Configuration.Tracking;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Graphics.UserInterface
{
    public class Stepper<T> : Container
    {
        [Resolved]
        private CircleConfigManager localConfig { get; set; }

        [Resolved]
        private TrackedSettings trackedSettings { get; set; }

        /// <summary>
        /// 이 설정이 무엇인지 표시합니다.
        /// </summary>
        public string Text { get; set; }

        private Container<StepperItem<T>> items;

        /// <summary>
        /// 아이템들.
        /// </summary>
        public StepperItem<T>[] Items;

        private SpriteText text;

        private readonly T initialCurrent;

        private int? selectedIndex;

        public T Selected => (selectedIndex >= 0 && selectedIndex < items.Count) ? Items[selectedIndex.Value].Value : default;

        /// <summary>
        /// 방향으로 값을 바꿀 수있는 컨트롤.
        /// </summary>
        public Stepper(T current = default)
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            CornerRadius = 5;
            initialCurrent = current;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Masking = true;
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            Children = new Drawable[]
            {
                new Box
                {
                    Colour = Color4.Black,
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0.2f
                },
                new SpriteText
                {
                    Text = Text,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Margin = new MarginPadding { Left = 20 },
                    Font = FontUsage.Default.With(size: 22)
                },
                new Container
                {
                    Margin = new MarginPadding { Right = 20 },
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    RelativeSizeAxes = Axes.X,
                    Height = 40,
                    Width = 0.5f,
                    Children = new Drawable[]
                    {
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
                        text = new SpriteText
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Font = FontUsage.Default.With(size: 22),
                        }
                    }
                },
                items = new Container<StepperItem<T>>
                {
                    Name = "Stepper items",
                    Children = Items
                }
            };
            Select(initialCurrent);
        }

        public void Select(T value)
        {
            var item = items.FirstOrDefault(v => v.Value.Equals(value));
            if (item == null)
                return;

            int newIndex = items.IndexOf(item);

            if (newIndex < 0)
                setSelected(null);
            else
                setSelected(newIndex);
        }

        public void SelectPrevious()
        {
            if (!selectedIndex.HasValue || selectedIndex == 0)
                setSelected(items.Count - 1);
            else
                setSelected(selectedIndex - 1);
        }

        public void SelectNext()
        {
            if (!selectedIndex.HasValue || selectedIndex == items.Count - 1)
                setSelected(0);
            else
                setSelected(selectedIndex + 1);
        }

        private void setSelected(int? index)
        {
            if (selectedIndex == index)
                return;

            if (selectedIndex.HasValue)
                items[selectedIndex.Value].State = SelectionState.NotSelected;

            selectedIndex = index;
            localConfig.LoadInto(trackedSettings);

            if (selectedIndex.HasValue)
            {
                items[selectedIndex.Value].State = SelectionState.Selected;
                text.Text = Items[selectedIndex.Value].Text;
            }
        }
    }
}
