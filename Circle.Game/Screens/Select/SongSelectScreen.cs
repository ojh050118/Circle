using Circle.Game.Graphics.UserInterface;
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

        public SongSelectScreen()
        {
            InternalChildren = new Drawable[]
            {
                new ScreenHeader(this),
                new BeatmapCarousel
                {
                    Width = 0.4f,
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight
                }
            };
        }

        public override bool OnExiting(IScreen next)
        {
            background.FadeTextureTo(TextureSource.Internal, "Duelyst", 1000, Easing.OutPow10);

            return base.OnExiting(next);
        }
    }
}
