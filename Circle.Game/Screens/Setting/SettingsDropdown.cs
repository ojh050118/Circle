using System.Collections.Generic;
using Circle.Game.Graphics.UserInterface;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Screens.Setting
{
    public class SettingsDropdown<T> : Container
        where T : struct
    {
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

        private readonly SpriteText text;
        private readonly CircleDropdown<T> dropdown;

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
                        text = new SpriteText
                        {
                            Font = FontUsage.Default.With(size: 22),
                        },
                        dropdown = new CircleDropdown<T>
                        {
                            RelativeSizeAxes = Axes.X
                        }
                    }
                }
            };
        }

        public void AddDropdownItem(T value) => dropdown.AddDropdownItem(value);
    }
}
