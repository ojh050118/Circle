using Circle.Game.Rulesets.Objects;
using Circle.Game.Rulesets.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;

namespace Circle.Game.Tests.Visual.Objects
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
            AddStep("Expansion = 1", () => planet.Expansion = 1);
            AddAssert("Expansion is 1", () => planet.Expansion == 1);
            AddStep("Expansion = 0", () => planet.Expansion = 0);
            AddAssert("Expansion is 0", () => planet.Expansion == 0);
            AddStep("Expansion to 1", () => planet.ExpandTo(1, 1000, Easing.OutQuint));
            AddWaitStep("Waiting 1000ms", 5);
            AddAssert("Expansion is 1", () => planet.Expansion == 1);
        }
    }
}
