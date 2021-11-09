using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Graphics.UserInterface
{
    /// <summary>
    /// 방향으로 값을 바꿀 수있는 컨트롤.
    /// </summary>
    /// <typeparam name="T">바꿀 값.</typeparam>
    public class RollingControl<T> : Container
    {
        /// <summary>
        /// 이 설정이 무엇인지 표시합니다.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 아이템들.
        /// </summary>
        public RollingItem<T>[] Item;

        /// <summary>
        /// 현재 값.
        /// </summary>
        public Bindable<T> Current { get; set; } = new Bindable<T>();

        /// <summary>
        /// 아이템에 설정되어있는 값을 표시합니다.
        /// </summary>
        private SpriteText text;

        /// <summary>
        /// 현재 아이템의 인덱스. 기본 값은 -1입니다.
        /// </summary>
        private int currentIdx = -1;

        /// <summary>
        /// 방향으로 값을 바꿀 수있는 컨트롤.
        /// </summary>
        public RollingControl()
        {
            CornerRadius = 5;
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
                    Height = 50,
                    Width = 0.4f,
                    Children = new Drawable[]
                    {
                        new IconButton
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Icon = FontAwesome.Solid.AngleLeft,
                            Action = () => changeCurrent(Direction.Backward),
                            Size = new Vector2(32)
                        },
                        new IconButton
                        {
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.CentreRight,
                            Icon = FontAwesome.Solid.AngleRight,
                            Action = () => changeCurrent(Direction.Forward),
                            Size = new Vector2(32)
                        },
                        text = new SpriteText
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Font = FontUsage.Default.With(size: 22)
                        }
                    }
                }
            };
        }

        /// <summary>
        /// 값을 설정하고 동작을 실행합니다.
        /// </summary>
        /// <param name="toValue">바꿀 값.</param>
        public void SetCurrent(T toValue)
        {
            var isExist = false;

            // 값의 존재여부, currentIndex를 바꿉니다.
            foreach (var i in Item)
            {
                if (i.Value.Equals(toValue))
                {
                    isExist = true;
                    currentIdx = Array.FindIndex(Item, r => r.Value.Equals(toValue));
                    break;
                }
            }

            // 값이 존재하지않는다면 아래 코드를 실행하지 않습니다.
            if (!isExist)
                return;

            // 새로운 값으로 바꿉니다.
            Current.Value = toValue;
            text.Text = Item[currentIdx].Text;

            // 동작을 실행합니다.
            if (Item[currentIdx].Action != null)
                Item[currentIdx].Action.Invoke();
        }

        /// <summary>
        /// 방향으로 Item 인덱스를 바꾸고 SetCurrent()를 실행합니다.
        /// </summary>
        /// <param name="direction">방향.</param>
        private void changeCurrent(Direction direction)
        {
            // 아이템들이 없거나 할당이 되어있지 않다면 아래 코드를 실행하지 않습니다.
            if (Item is null)
                return;
            else if (Item.Length == 0)
                return;

            if (direction == Direction.Forward)
                SetCurrent(currentIdx + 1 >= Item.Length ? Item[0].Value : Item[currentIdx + 1].Value);
            else
                SetCurrent(currentIdx <= 0 ? Item[Item.Length - 1].Value : Item[currentIdx - 1].Value);
        }

        private enum Direction
        {
            Forward,
            Backward
        }
    }
}
