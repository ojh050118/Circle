#nullable disable

using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK.Graphics;

namespace Circle.Game.Graphics.UserInterface
{
    public partial class CircleButton : ClickableContainer
    {
        private readonly Box box;

        private readonly bool useBackground;
        private Sample clickSample;

        private CircleColour colours;

        protected new Container Content;
        private Sample hoverSample;

        public CircleButton(bool useBackground = true)
        {
            this.useBackground = useBackground;
            Child = Content = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                CornerRadius = 5,
                Child = box = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                }
            };

            Enabled.BindValueChanged(enabledChanged, true);
        }

        public new float CornerRadius
        {
            get => Content.CornerRadius;
            set => Content.CornerRadius = value;
        }

        [BackgroundDependencyLoader]
        private void load(AudioManager audio, CircleColour colours)
        {
            this.colours = colours;
            hoverSample = audio.Samples.Get("button-hover");
            clickSample = audio.Samples.Get("button-click");
            box.Colour = useBackground ? colours.TransparentBlack : Color4.Transparent;

            Content.CornerRadius = CornerRadius;
        }

        protected override void Update()
        {
            base.Update();

            Content.CornerRadius = Height / 6;
        }

        protected override bool OnHover(HoverEvent e)
        {
            if (!Enabled.Value)
                return base.OnHover(e);

            hoverSample?.Play();
            box.FadeColour(colours.TransparentGray, 250, Easing.OutQuint);

            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            base.OnHoverLost(e);

            box.FadeColour(useBackground ? colours.TransparentBlack : Color4.Transparent, 500, Easing.OutQuint);
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            if (!Enabled.Value)
                return base.OnMouseDown(e);

            Child.ScaleTo(0.9f, 1500, Easing.OutPow10);

            return base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            if (!Enabled.Value)
                return;

            Child.ScaleTo(1, 1500, Easing.OutPow10);

            base.OnMouseUp(e);
        }

        protected override bool OnClick(ClickEvent e)
        {
            if (!Enabled.Value)
                return base.OnClick(e);

            clickSample?.Play();

            return base.OnClick(e);
        }

        private void enabledChanged(ValueChangedEvent<bool> e)
        {
            this.FadeTo(e.NewValue ? 1f : 0.33f, 500, Easing.OutPow10);
            Child.ScaleTo(e.NewValue ? 1f : 0.9f, 500, Easing.OutPow10);
        }
    }
}
