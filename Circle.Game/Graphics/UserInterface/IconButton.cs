#nullable disable

using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace Circle.Game.Graphics.UserInterface
{
    public partial class IconButton : CircleButton
    {
        public const float DEFAULT_BUTTON_SIZE = 30;

        private readonly SpriteIcon icon;

        public IconButton(bool useBackground = true)
            : base(useBackground)
        {
            Size = new Vector2(DEFAULT_BUTTON_SIZE);
            Content.Add(icon = new SpriteIcon
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(0.6f)
            });
        }

        public IconUsage Icon
        {
            get => icon.Icon;
            set => icon.Icon = value;
        }
    }
}
