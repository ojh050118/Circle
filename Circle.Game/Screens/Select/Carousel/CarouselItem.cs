using System;
using System.Collections.Generic;
using System.Text;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics;
using osuTK.Graphics;
using osu.Framework.Graphics.Effects;
using Circle.Game.Graphics.UserInterface;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Bindables;
using osuTK;
using osu.Framework.Input.Events;

namespace Circle.Game.Screens.Select.Carousel
{
    public class CarouselItem : Container
    {
        public Container BorderContainer;

        protected override Container<Drawable> Content { get; } = new Container { RelativeSizeAxes = Axes.Both };

        public readonly Bindable<CarouselItemState> State = new Bindable<CarouselItemState>(CarouselItemState.NotSelected);

        public CarouselItem()
        {
            Size = new Vector2(1, 250);
            RelativeSizeAxes = Axes.X;
            Anchor = Anchor.TopCentre;
            Origin = Anchor.TopCentre;
            InternalChild = BorderContainer = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                CornerRadius = 10,
                BorderColour = Color4.SkyBlue,
                Child = new Background(textureName: "Duelyst")

            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            State.BindValueChanged(updateState, true);
        }

        private void updateState(ValueChangedEvent<CarouselItemState> state)
        {
            switch (state.NewValue)
            {
                case CarouselItemState.NotSelected:
                case CarouselItemState.Collapsed:
                    BorderContainer.BorderThickness = 0;
                    BorderContainer.EdgeEffect = new EdgeEffectParameters
                    {
                        Type = EdgeEffectType.Shadow,
                        Radius = 10,
                        Colour = Color4.Black.Opacity(100),
                    };
                    break;
                case CarouselItemState.Selected:
                    BorderContainer.BorderThickness = 2.5f;
                    BorderContainer.EdgeEffect = new EdgeEffectParameters
                    {
                        Type = EdgeEffectType.Glow,
                        Colour = Color4.SkyBlue.Opacity(0.6f),
                        Radius = 20,
                        Roundness = 10,
                    };
                    break;
            }
        }

        protected override bool OnClick(ClickEvent e)
        {
            State.Value = CarouselItemState.Selected;

            return base.OnClick(e);
        }
    }
}
