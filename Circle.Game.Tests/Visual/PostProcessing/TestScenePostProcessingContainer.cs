using Circle.Game.Graphics.UserInterface;
using Circle.Game.Rulesets.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osuTK;

namespace Circle.Game.Tests.Visual.PostProcessing
{
    public partial class TestScenePostProcessingContainer : CircleTestScene
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            PostProcessingContainer postProcessing;

            Add(postProcessing = new PostProcessingContainer
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(0.8f),
                Children = new Drawable[]
                {
                    new Background(textureName: "bg1")
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                        Size = new Vector2(0.8f),
                    }
                }
            });

            AddLabel("Aberration filter");
            AddStep("Enable LED filter", () => postProcessing.Aberration.Enabled = true);
            AddSliderStep("LED filter Intensity", 0, 1, 1f, v => postProcessing.Aberration.Intensity = v);
            AddStep("Disable LED filter", () => postProcessing.Aberration.Enabled = false);

            AddLabel("Arcade filter");
            AddStep("Enable Arcade filter", () => postProcessing.Arcade.Enabled = true);
            AddStep("Disable Arcade filter", () => postProcessing.Arcade.Enabled = false);

            AddLabel("Bloom filter");
            AddStep("Enable Bloom filter", () => postProcessing.Bloom.Enabled = true);
            AddSliderStep("LED filter Intensity", 0, 1, 1f, v => postProcessing.Bloom.Intensity = v);
            AddSliderStep("LED filter Threshold", 0, 1, 1f, v => postProcessing.Bloom.Threshold = v);
            AddSliderStep("LED filter R", 0, 1, 1f, v => postProcessing.Bloom.Color = postProcessing.Bloom.Color with { R = v });
            AddSliderStep("LED filter G", 0, 1, 1f, v => postProcessing.Bloom.Color = postProcessing.Bloom.Color with { G = v });
            AddSliderStep("LED filter B", 0, 1, 1f, v => postProcessing.Bloom.Color = postProcessing.Bloom.Color with { B = v });
            AddStep("Disable Bloom filter", () => postProcessing.Bloom.Enabled = false);

            AddLabel("Compression filter");
            AddStep("Enable Compression filter", () => postProcessing.Compression.Enabled = true);
            AddSliderStep("Compression filter Intensity", 0, 1, 1f, v => postProcessing.Compression.Intensity = v);
            AddStep("Disable Compression filter", () => postProcessing.Compression.Enabled = false);

            AddLabel("Funk filter");
            AddStep("Enable Funk filter", () => postProcessing.Funk.Enabled = true);
            AddStep("Disable Funk filter", () => postProcessing.Funk.Enabled = false);

            AddLabel("Glitch filter");
            AddStep("Enable Glitch filter", () => postProcessing.Glitch.Enabled = true);
            AddStep("Disable Glitch filter", () => postProcessing.Glitch.Enabled = false);

            AddLabel("Grain filter");
            AddStep("Enable Grain filter", () => postProcessing.Grain.Enabled = true);
            AddSliderStep("Grain filter Intensity", 0, 1, 1f, v => postProcessing.Grain.Intensity = v);
            AddStep("Disable Grain filter", () => postProcessing.Grain.Enabled = false);

            AddLabel("Grayscale filter");
            AddStep("Enable Grayscale filter", () => postProcessing.Grayscale.Enabled = true);
            AddSliderStep("Grayscale filter Intensity", 0, 1, 1f, v => postProcessing.Grayscale.Intensity = v);
            AddStep("Disable Grayscale filter", () => postProcessing.Grayscale.Enabled = false);

            AddLabel("Invert filter");
            AddStep("Enable Invert filter", () => postProcessing.Invert.Enabled = true);
            AddStep("Disable Invert filter", () => postProcessing.Invert.Enabled = false);

            AddLabel("LED filter");
            AddStep("Enable LED filter", () => postProcessing.Led.Enabled = true);
            AddSliderStep("LED filter Intensity", 0, 1, 1f, v => postProcessing.Led.Intensity = v);
            AddStep("Disable LED filter", () => postProcessing.Led.Enabled = false);

            AddLabel("Neon filter");
            AddStep("Enable Neon filter", () => postProcessing.Neon.Enabled = true);
            AddStep("Disable Neon filter", () => postProcessing.Neon.Enabled = false);

            AddLabel("Night Vision filter");
            AddStep("Enable Night Vision filter", () => postProcessing.NightVision.Enabled = true);
            AddStep("Disable Night Vision filter", () => postProcessing.NightVision.Enabled = false);

            AddLabel("Sepia filter");
            AddStep("Enable Sepia filter", () => postProcessing.Sepia.Enabled = true);
            AddSliderStep("Sepia filter Intensity", 0, 1, 1f, v => postProcessing.Sepia.Intensity = v);
            AddStep("Disable Sepia filter", () => postProcessing.Sepia.Enabled = false);

            AddLabel("Waves filter");
            AddStep("Enable Waves filter", () => postProcessing.Waves.Enabled = true);
            AddSliderStep("Waves filter Intensity", 0, 1, 1f, v => postProcessing.Waves.Intensity = v);
            AddStep("Disable Waves filter", () => postProcessing.Waves.Enabled = false);

            AddLabel("Weird 3D filter");
            AddStep("Enable Weird 3D filter", () => postProcessing.Weird3D.Enabled = true);
            AddStep("Disable Weird 3D filter", () => postProcessing.Weird3D.Enabled = false);
        }
    }
}
