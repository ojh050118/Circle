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
            double duration = 0;

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

            AddSliderStep<double>("Duration", 0, 1000, 500, v => duration = v);

            AddLabel("Dim");
            AddSliderStep<float>("Dim", 0, 1, 0, v => background.Dim = v);
            AddStep("Dim to 0.5", () => background.DimTo(0.5f, duration));
            AddStep("Dim to 0", () => background.DimTo(0, duration));

            AddLabel("Blur");
            AddSliderStep("BlurSigma", 0, 10f, 0, v => background.BlurSigma = new Vector2(v));
            AddStep("Blur to 10", () => background.BlurTo(new Vector2(10), duration));
            AddStep("Blur to 0", () => background.BlurTo(Vector2.Zero, duration));

            AddLabel("Fade texture");
            AddStep("Fade texture to Duelyst", () => background.ChangeTexture(TextureSource.Internal, "Duelyst", duration));
            AddStep("Fade texture to external texture", () => background.ChangeTexture(TextureSource.External, textBox.Text, duration));
        }
    }
}
