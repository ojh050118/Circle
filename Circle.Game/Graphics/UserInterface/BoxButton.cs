#nullable disable

using Circle.Game.Graphics.Sprites;
using osu.Framework.Graphics;

namespace Circle.Game.Graphics.UserInterface
{
    public partial class BoxButton : CircleButton
    {
        private readonly CircleSpriteText sprite;

        public BoxButton(bool useBackground = true)
            : base(useBackground)
        {
            RelativeSizeAxes = Axes.X;
            Height = 40;
            Content.Add(sprite = new CircleSpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Padding = new MarginPadding(10),
                Font = CircleFont.Default.With(size: 22)
            });
        }

        public string Text
        {
            get => sprite.Text.ToString();
            set => sprite.Text = value;
        }
    }
}
