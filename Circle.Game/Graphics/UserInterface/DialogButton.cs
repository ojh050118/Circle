#nullable disable

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK.Graphics;

namespace Circle.Game.Graphics.UserInterface
{
    public partial class DialogButton : ClickableContainer
    {
        private readonly Box hover;
        private readonly SpriteText sprite;

        public DialogButton()
        {
            RelativeSizeAxes = Axes.X;
            Height = 50;
            Children = new Drawable[]
            {
                hover = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0,
                    Colour = Color4.Black
                },
                sprite = new SpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Font = FontUsage.Default.With(size: 28),
                    Colour = Color4.DodgerBlue
                }
            };
        }

        public string Text
        {
            get => sprite.Text.ToString();
            set => sprite.Text = value;
        }

        public Color4 TextColour
        {
            get => sprite.Colour;
            set => sprite.Colour = value;
        }

        public FontUsage Font
        {
            get => sprite.Font;
            set => sprite.Font = value;
        }

        protected override bool OnClick(ClickEvent e)
        {
            hover.FadeTo(0.5f).Then().FadeTo(0.3f, 1000, Easing.OutPow10);
            return base.OnClick(e);
        }

        protected override bool OnHover(HoverEvent e)
        {
            hover.FadeTo(0.3f, 0, Easing.OutPow10);
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            hover.FadeOut(200, Easing.OutPow10);
            base.OnHoverLost(e);
        }
    }
}
