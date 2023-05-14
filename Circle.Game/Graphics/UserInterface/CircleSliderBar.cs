#nullable disable

using System;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Graphics.UserInterface
{
    public class CircleSliderBar<T> : SliderBar<T>
        where T : struct, IEquatable<T>, IComparable<T>, IConvertible
    {
        protected readonly Nub CircleNub;
        private readonly Box leftBox;
        private readonly Container nubContainer;
        private readonly Box rightBox;

        public CircleSliderBar()
        {
            Height = 30;
            RangePadding = 15;
            Children = new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Padding = new MarginPadding { Horizontal = 2 },
                    Child = new CircularContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Masking = true,
                        CornerRadius = 5f,
                        Children = new Drawable[]
                        {
                            leftBox = new Box
                            {
                                Height = 6,
                                EdgeSmoothness = new Vector2(0, 0.5f),
                                RelativeSizeAxes = Axes.None,
                                Colour = Color4.DeepSkyBlue,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                            },
                            rightBox = new Box
                            {
                                Height = 6,
                                EdgeSmoothness = new Vector2(0, 0.5f),
                                RelativeSizeAxes = Axes.None,
                                Anchor = Anchor.CentreRight,
                                Origin = Anchor.CentreRight,
                                Alpha = 0.5f,
                            },
                        },
                    },
                },
                nubContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = CircleNub = new Nub
                    {
                        Origin = Anchor.TopCentre,
                        RelativePositionAxes = Axes.X,
                    },
                },
            };

            Current.DisabledChanged += disabled => { Alpha = disabled ? 0.3f : 1; };
        }

        public void Commit(bool increase)
        {
            float step = KeyboardStep != 0 ? KeyboardStep : (Convert.ToSingle(CurrentNumber.MaxValue) - Convert.ToSingle(CurrentNumber.MinValue)) / 20;
            if (CurrentNumber.IsInteger)
                step = MathF.Ceiling(step);

            if (increase)
                CurrentNumber.Add(step);
            else
                CurrentNumber.Add(-step);
        }

        protected override void Update()
        {
            base.Update();

            nubContainer.Padding = new MarginPadding { Horizontal = RangePadding };
        }

        protected override void UpdateValue(float value)
        {
            CircleNub.MoveToX(value, 250, Easing.OutQuint);
        }

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();
            leftBox.Scale = new Vector2(Math.Clamp(
                RangePadding + CircleNub.DrawPosition.X - CircleNub.DrawWidth / 2, 0, DrawWidth), 1);
            rightBox.Scale = new Vector2(Math.Clamp(
                DrawWidth - CircleNub.DrawPosition.X - RangePadding - CircleNub.DrawWidth / 2, 0, DrawWidth), 1);
        }

        protected class Nub : CircularContainer
        {
            public Nub()
            {
                Masking = true;
                Size = new Vector2(30);
                InternalChild = new Box
                {
                    Colour = Color4.White,
                    RelativeSizeAxes = Axes.Both,
                    AlwaysPresent = true
                };
                EdgeEffect = new EdgeEffectParameters
                {
                    Colour = Color4.Black.Opacity(0.2f),
                    Type = EdgeEffectType.Shadow,
                    Radius = 8,
                    Roundness = 5
                };
            }
        }
    }
}
