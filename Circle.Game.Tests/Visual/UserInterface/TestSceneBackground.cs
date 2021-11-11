using Circle.Game.Graphics.UserInterface;
using osuTK;

namespace Circle.Game.Tests.Visual.UserInterface
{
    public class TestSceneBackground : CircleTestScene
    {
        public TestSceneBackground()
        {
            Background background;

            Add(background = new Background(textureName: "Duelyst"));
            AddSliderStep("BlurSIgma", 0, 10f, 0, v => background.BlurSigma.Value = new Vector2(v));
            AddStep("BlurTo 10", () => background.BlurTo(new Vector2(10), 500));
            AddStep("BlurTo 0", () => background.BlurTo(Vector2.Zero, 500));
        }
    }
}
