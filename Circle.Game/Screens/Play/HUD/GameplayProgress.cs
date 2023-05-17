#nullable disable

using Circle.Game.Graphics;
using Circle.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Circle.Game.Screens.Play.HUD
{
    public partial class GameplayProgress : Container
    {
        private readonly int floorCount;
        private CircleSpriteText percent;
        private GameProgressBar progressBar;

        public GameplayProgress(int floorCount)
        {
            this.floorCount = floorCount;
        }

        public int CurrentFloor
        {
            get => progressBar.CurrentFloor;
            set => progressBar.CurrentFloor = value;
        }

        [BackgroundDependencyLoader]
        private void load(CircleColour colours)
        {
            AutoSizeAxes = Axes.Y;
            RelativeSizeAxes = Axes.X;
            Children = new Drawable[]
            {
                progressBar = new GameProgressBar(20, 10)
                {
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    Duration = 200,
                    StartFloor = 0,
                    EndFloor = floorCount,
                },
                percent = new CircleSpriteText
                {
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    Margin = new MarginPadding { Bottom = 30 },
                    Text = "0%",
                    Font = CircleFont.Default.With(weight: FontWeight.Bold, size: 32),
                    ShadowColour = colours.TransparentBlack
                }
            };
        }

        public void Increase(int amount = 1)
        {
            if (progressBar.Current.Value + amount > progressBar.EndFloor)
                return;

            progressBar.Current.Value += amount;
            updateCurrent();
        }

        public void Decrease(int amount = 1)
        {
            if (progressBar.Current.Value - 1 < progressBar.StartFloor)
                return;

            progressBar.Current.Value -= amount;
            updateCurrent();
        }

        public void ProgressTo(int progress)
        {
            progressBar.Current.Value = progress;
            updateCurrent();
        }

        private void updateCurrent()
        {
            percent.Text = $"{(float)progressBar.CurrentFloor / floorCount * 100:0.#}%";
            percent.ScaleTo(1.15f).ScaleTo(1, 500, Easing.OutQuint);
        }
    }
}
