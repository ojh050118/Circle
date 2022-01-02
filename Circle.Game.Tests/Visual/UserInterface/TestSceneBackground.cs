using Circle.Game.Graphics.UserInterface;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Tests.Visual.UserInterface
{
    public class TestSceneBackground : CircleTestScene
    {
        public TestSceneBackground()
        {
            Background background;
            BasicTextBox textBox;
            double fadeDuration = 0;

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
            Add(textBox = new BasicTextBox
            {
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                PlaceholderText = "External texture name",
                Size = new Vector2(200, 30)
            });

            AddLabel("Scale");
            AddStep("Scale to 0.6", () => background.Scale = new Vector2(0.6f));
            AddStep("Scale to 1", () => background.Scale = new Vector2(1));
            AddLabel("Blur");
            AddSliderStep("BlurSIgma", 0, 10f, 0, v => background.BlurSigma = new Vector2(v));
            AddStep("BlurTo 10", () => background.BlurTo(new Vector2(10), 500));
            AddStep("BlurTo 0", () => background.BlurTo(Vector2.Zero, 500));
            AddLabel("Fade texture");
            AddSliderStep<double>("Duration", 0, 2000, 1000, v => fadeDuration = v);
            AddStep("Fade texture to Duelyst", () => background.FadeTextureTo(TextureSource.Internal, "Duelyst", fadeDuration));
            AddStep("Fade texture to external texture", () => background.FadeTextureTo(TextureSource.External, textBox.Text, fadeDuration));
        }
    }
}
