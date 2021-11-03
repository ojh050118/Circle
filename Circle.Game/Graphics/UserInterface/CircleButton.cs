using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK.Graphics;

namespace Circle.Game.Graphics.UserInterface
{
    public class CircleButton : ClickableContainer
    {
        private Box background;
        private Box hover;

        protected new Container Content;

        public new float CornerRadius { get; set; } = 5;

        public CircleButton(bool useBackground = true)
        {
            Masking = true;
            Child = Content = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                CornerRadius = CornerRadius,
                Children = new Drawable[]
                {
                    background = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.Black,
                        Alpha = useBackground ? 0.4f : 0
                    },
                    hover = new Box
                    {
                        Colour = Color4.White,
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0
                    }
                }
            };
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Content.CornerRadius = CornerRadius;
        }

        protected override bool OnHover(HoverEvent e)
        {
            hover.FadeTo(0.25f, 500, Easing.OutQuint);

            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            base.OnHoverLost(e);

            hover.FadeTo(0, 500, Easing.OutQuint);
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            Child.ScaleTo(0.9f, 1500, Easing.OutPow10);

            return base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            Child.ScaleTo(1, 1500, Easing.OutPow10);

            base.OnMouseUp(e);
        }
    }
}
