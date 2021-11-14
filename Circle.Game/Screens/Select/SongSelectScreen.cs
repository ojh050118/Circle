using Circle.Game.Graphics.UserInterface;
using Circle.Game.Screens.Setting.Sections;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Screens.Select
{
    public class SongSelectScreen : CircleScreen
    {
        public override string Header => "Play";

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
    }
}
