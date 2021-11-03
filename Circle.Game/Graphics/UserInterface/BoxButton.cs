using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace Circle.Game.Graphics.UserInterface
{
    public class BoxButton : CircleButton
    {
        private SpriteText sprite;

        public string Text
        {
            get => sprite.Text.ToString();
            set => sprite.Text = value;
        }

        public BoxButton(bool useBackground = true)
            : base(useBackground)
        {
            RelativeSizeAxes = Axes.X;
            Height = 40;
            Content.Add(sprite = new SpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Padding = new MarginPadding(10),
                Font = FontUsage.Default.With(size: 22)
            });
        }
    }
}
