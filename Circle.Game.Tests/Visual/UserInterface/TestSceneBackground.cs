using Circle.Game.Graphics.UserInterface;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Tests.Visual.UserInterface
{
    public class TestSceneBackground : CircleTestScene
    {
        public TestSceneBackground()
        {
            Background background;

            Add(new Box
            {
                Colour = Color4.Black,
                RelativeSizeAxes = Axes.Both
            });
            Add(background = new Background(textureName: "Duelyst")
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            });
            AddSliderStep("BlurSIgma", 0, 10f, 0, v => background.BlurSigma = new Vector2(v));
            AddStep("Scale to 0.6", () => background.Scale = new Vector2(0.6f));
            AddStep("Scale to 1", () => background.Scale = new Vector2(1));
            AddStep("BlurTo 10", () => background.BlurTo(new Vector2(10), 500));
            AddStep("BlurTo 0", () => background.BlurTo(Vector2.Zero, 500));
            AddStep("Fade texture", () => background.FadeTextureTo(TextureSource.Internal, "Duelyst", 1000));
        }
    }
}
