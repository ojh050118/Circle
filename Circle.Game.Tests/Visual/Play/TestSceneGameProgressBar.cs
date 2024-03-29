#nullable disable

using Circle.Game.Screens.Play;
using osu.Framework.Graphics;

namespace Circle.Game.Tests.Visual.Play
{
    public partial class TestSceneGameProgressBar : CircleTestScene
    {
        public TestSceneGameProgressBar()
        {
            GameProgressBar bar;
            Add(bar = new GameProgressBar(20, 5)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                StartFloor = 0,
                EndFloor = 20
            });
            AddSliderStep("Progress", 0, 20, 5, v => bar.CurrentFloor = v);
        }
    }
}
