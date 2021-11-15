using Circle.Game.Graphics.UserInterface;
using Circle.Game.Screens.Play;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace Circle.Game.Screens.Select
{
    public class SongSelectScreen : CircleScreen
    {
        public override string Header => "Play";

        [Resolved]
        private Background background { get; set; }

        private BeatmapCarousel carousel;

        public SongSelectScreen()
        {
            InternalChildren = new Drawable[]
            {
                new ScreenHeader(this),
                carousel = new BeatmapCarousel
                {
                    Width = 0.4f,
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            carousel.PlayRequested.ValueChanged += v =>
            {
                if (v.NewValue)
                    this.Push(new PlayerLoader());
            };
        }

        public override bool OnExiting(IScreen next)
        {
            background.FadeTextureTo(TextureSource.Internal, "Duelyst", 1000, Easing.OutPow10);

            return base.OnExiting(next);
        }

        public override void OnSuspending(IScreen next)
        {
            base.OnSuspending(next);

            carousel.FadeOut(500, Easing.OutPow10);
        }

        public override void OnResuming(IScreen last)
        {
            base.OnResuming(last);

            carousel.FadeIn(1000, Easing.OutPow10);
            carousel.PlayRequested.Value = false;
        }
    }
}
