using Circle.Game.Screens.Select;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;
using osu.Framework.Graphics;

namespace Circle.Game.Tests.Visual.SongSelect
{
    public class TestSceneBeatmapCarousel : CircleTestScene
    {
        public TestSceneBeatmapCarousel()
        {
            Add(new Box
            {
                Colour = Color4.Black,
                Alpha = 0.7f,
                RelativeSizeAxes = Axes.Both,
            });
            Add(new BeatmapCarousel());
        }
    }
}
