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

        private readonly BeatmapCarousel carousel;
        private readonly BeatmapDetails details;

        public SongSelectScreen()
        {
            InternalChildren = new Drawable[]
            {
                new ScreenHeader(this),
                details = new BeatmapDetails
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Padding = new MarginPadding { Top = 130, Bottom = 65 },
                    Margin = new MarginPadding { Left = 80 },
                    Width = 0.5f
                },
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
                    this.Push(new Player());
            };
        }

        public override void OnEntering(IScreen last)
        {
            base.OnEntering(last);

            details.RotateTo(-45).Then().RotateTo(0, 1000, Easing.OutPow10);
            details.MoveToY(500).Then().MoveToY(0, 1000, Easing.OutPow10);
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
            details.MoveToX(-500, 500, Easing.OutPow10);
        }

        public override void OnResuming(IScreen last)
        {
            base.OnResuming(last);

            carousel.FadeIn(1000, Easing.OutPow10);
            carousel.PlayRequested.Value = false;
            details.MoveToX(0, 500, Easing.OutPow10);
        }
    }
}
