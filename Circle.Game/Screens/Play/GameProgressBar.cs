#nullable disable

using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Threading;
using osuTK.Graphics;

namespace Circle.Game.Screens.Play
{
    public class GameProgressBar : SliderBar<int>
    {
        private readonly Box fill;
        public float Duration;
        public Action<double> OnSeek;

        private ScheduledDelegate scheduledSeek;

        public bool Seekable;

        public GameProgressBar(float barHeight, float handleBarHeight)
        {
            CurrentNumber.MinValue = 0;
            CurrentNumber.MaxValue = 1;
            RelativeSizeAxes = Axes.X;
            Height = barHeight + handleBarHeight;

            Children = new Drawable[]
            {
                new Box
                {
                    Name = "Background",
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    RelativeSizeAxes = Axes.X,
                    Height = barHeight,
                    Colour = ColourInfo.GradientVertical(Color4.Transparent, Color4.Black),
                    Alpha = 0.4f,
                    Depth = 1,
                },
                fill = new Box
                {
                    Name = "Fill",
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    Height = barHeight,
                    Colour = ColourInfo.GradientVertical(Color4.Transparent, Color4.White)
                },
            };
        }

        public Color4 FillColour
        {
            set => fill.Colour = value;
        }

        public int StartFloor
        {
            get => CurrentNumber.MinValue;
            set => CurrentNumber.MinValue = value;
        }

        public int EndFloor
        {
            get => CurrentNumber.MaxValue;
            set => CurrentNumber.MaxValue = value;
        }

        public int CurrentFloor
        {
            get => CurrentNumber.Value;
            set => CurrentNumber.Value = value;
        }

        protected override void UpdateValue(float value)
        {
            // handled in update
        }

        protected override void Update()
        {
            base.Update();

            fill.ResizeWidthTo(UsableWidth / CurrentNumber.MaxValue * CurrentNumber.Value, Duration, Easing.OutSine);
        }

        protected override void OnUserChange(int value)
        {
            scheduledSeek?.Cancel();
            scheduledSeek = Schedule(() =>
            {
                if (Seekable)
                    OnSeek?.Invoke(value);
            });
        }
    }
}
