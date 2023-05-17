#nullable disable

using System.Collections.Generic;
using Circle.Game.Graphics;
using Circle.Game.Graphics.Sprites;
using Circle.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Screens.Setting
{
    public partial class SettingsDropdown<T> : Container
        where T : struct
    {
        private readonly CircleDropdown<T> dropdown;

        private readonly CircleSpriteText text;

        public Bindable<T> Current;

        public SettingsDropdown()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            Masking = true;
            CornerRadius = 5;
            Children = new Drawable[]
            {
                new Box
                {
                    Colour = Color4.Black,
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0.2f
                },
                new FillFlowContainer
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Direction = FillDirection.Vertical,
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Padding = new MarginPadding(10),
                    Spacing = new Vector2(10),
                    Children = new Drawable[]
                    {
                        text = new CircleSpriteText
                        {
                            Font = CircleFont.Default.With(size: 22),
                            Truncate = true,
                        },
                        dropdown = new CircleDropdown<T>
                        {
                            RelativeSizeAxes = Axes.X,
                            MaxHeight = 200,
                        }
                    }
                }
            };
        }

        public string Text
        {
            get => text.Text.ToString();
            set => text.Text = value;
        }

        public IEnumerable<T> Items
        {
            get => dropdown?.Items;
            set => dropdown.Items = value;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            if (Current != null)
                dropdown.Current.BindTo(Current);
        }

        public void AddDropdownItem(T value) => dropdown.AddDropdownItem(value);
    }
}
