using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Threading;
using osuTK.Graphics;

namespace Circle.Game.Screens.Play
{
    public class GameProgressBar : SliderBar<int>
    {
        public Action<double> OnSeek;

        public bool Seekable;
        public float Duration;

        private readonly Box fill;

        public Color4 FillColour
        {
            set => fill.Colour = value;
        }

        public int StartFloor
        {
            set => CurrentNumber.MinValue = value;
        }

        public int EndFloor
        {
            set => CurrentNumber.MaxValue = value;
        }

        public int CurrentFloor
        {
            set => CurrentNumber.Value = value;
        }

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
                    Colour = Color4.Black,
                    Alpha = 0.4f,
                    Depth = 1,
                },
                fill = new Box
                {
                    Name = "Fill",
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    Height = barHeight,
                },
            };
        }

        protected override void UpdateValue(float value)
        {
            // handled in update
        }

        protected override void Update()
        {
            base.Update();

            fill.ResizeWidthTo(UsableWidth / CurrentNumber.MaxValue * CurrentNumber.Value, Duration, Easing.OutQuint);
            //fill.Width = newX;
        }

        private ScheduledDelegate scheduledSeek;

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
