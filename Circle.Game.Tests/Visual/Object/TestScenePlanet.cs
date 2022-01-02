using Circle.Game.Rulesets.Objects;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;

namespace Circle.Game.Tests.Visual.Object
{
    public class TestScenePlanet : CircleTestScene
    {
        public TestScenePlanet()
        {
            Planet planet;

            Add(new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Color4.Black
            });
            Add(planet = new Planet(Color4.DeepSkyBlue)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            });
            AddStep("Toggle Expansion", () => planet.Expansion = planet.Expansion == 1 ? 0 : 1);
        }
    }
}
