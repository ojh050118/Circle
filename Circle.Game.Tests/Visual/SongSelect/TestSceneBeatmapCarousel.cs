using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Circle.Game.Screens.Select;
using Circle.Game.Screens.Select.Carousel;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;
using osu.Framework.Graphics;

namespace Circle.Game.Tests.Visual.SongSelect
{
    public class TestSceneBeatmapCarousel : CircleTestScene
    {
        public TestSceneBeatmapCarousel()
        {
            Add(new Box()
            {
                Colour = Color4.Black,
                Alpha = 0.7f,
                RelativeSizeAxes = Axes.Both,
            });
            Add(new BeatmapCarousel());
        }
    }
}
