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
            AddStep("Enable Aberration filter", () => postProcessing.Aberration.Enabled = true);
            AddSliderStep("Aberration filter Intensity", 0, 1, 1f, v => postProcessing.Aberration.Intensity = v);
            AddStep("Disable Aberration filter", () => postProcessing.Aberration.Enabled = false);

            AddLabel("Arcade filter");
            AddStep("Enable Arcade filter", () => postProcessing.Arcade.Enabled = true);
            AddStep("Disable Arcade filter", () => postProcessing.Arcade.Enabled = false);

            AddLabel("Blizzard filter");
            AddStep("Enable Blizzard filter", () => postProcessing.Blizzard.Enabled = true);
            AddSliderStep("Blizzard filter Intensity", 0, 1, 1f, v => postProcessing.Blizzard.Intensity = v);
            AddStep("Disable Blizzard filter", () => postProcessing.Blizzard.Enabled = false);

            AddLabel("Bloom filter");
            AddStep("Enable Bloom filter", () => postProcessing.Bloom.Enabled = true);
            AddSliderStep("Bloom filter Intensity", 0, 1, 1f, v => postProcessing.Bloom.Intensity = v);
            AddSliderStep("Bloom filter Threshold", 0, 1, 1f, v => postProcessing.Bloom.Threshold = v);
            AddSliderStep("Bloom filter R", 0, 1, 1f, v => postProcessing.Bloom.Color = postProcessing.Bloom.Color with { R = v });
            AddSliderStep("Bloom filter G", 0, 1, 1f, v => postProcessing.Bloom.Color = postProcessing.Bloom.Color with { G = v });
            AddSliderStep("Bloom filter B", 0, 1, 1f, v => postProcessing.Bloom.Color = postProcessing.Bloom.Color with { B = v });
            AddStep("Disable Bloom filter", () => postProcessing.Bloom.Enabled = false);

            AddLabel("Blur filter");
            AddStep("Enable Blur filter", () => postProcessing.Blur.Enabled = true);
            AddSliderStep("Blur filter Intensity", 0, 1, 1f, v => postProcessing.Blur.Intensity = v);
            AddStep("Disable Blur filter", () => postProcessing.Blur.Enabled = false);

            AddLabel("Blur Focus filter");
            AddStep("Enable Blur Focus filter", () => postProcessing.BlurFocus.Enabled = true);
            AddSliderStep("Blur Focus filter Intensity", 0, 1, 1f, v => postProcessing.BlurFocus.Intensity = v);
            AddStep("Disable Blur Focus filter", () => postProcessing.BlurFocus.Enabled = false);

            AddLabel("Compression filter");
            AddStep("Enable Compression filter", () => postProcessing.Compression.Enabled = true);
            AddSliderStep("Compression filter Intensity", 0, 1, 1f, v => postProcessing.Compression.Intensity = v);
            AddStep("Disable Compression filter", () => postProcessing.Compression.Enabled = false);

            AddLabel("Contrast filter");
            AddStep("Enable Contrast filter", () => postProcessing.Contrast.Enabled = true);
            AddSliderStep("Contrast filter Intensity", 0, 1, 1f, v => postProcessing.Contrast.Intensity = v);
            AddStep("Disable Contrast filter", () => postProcessing.Contrast.Enabled = false);

            AddLabel("Drawing filter");
            AddStep("Enable Drawing filter", () => postProcessing.Drawing.Enabled = true);
            AddSliderStep("Drawing filter Intensity", 0, 1, 1f, v => postProcessing.Drawing.Intensity = v);
            AddStep("Disable Drawing filter", () => postProcessing.Drawing.Enabled = false);

            AddLabel("Edge Black Line TV filter");
            AddStep("Enable Edge Black Line TV filter", () => postProcessing.EdgeBlackLine.Enabled = true);
            AddStep("Disable Edge Black Line TV filter", () => postProcessing.EdgeBlackLine.Enabled = false);

            AddLabel("Eighties TV filter");
            AddStep("Enable Eighties TV filter", () => postProcessing.EightiesTv.Enabled = true);
            AddStep("Disable Eighties TV filter", () => postProcessing.EightiesTv.Enabled = false);

            AddLabel("Fifties TV filter");
            AddStep("Enable Fifties TV filter", () => postProcessing.FiftiesTv.Enabled = true);
            AddStep("Disable Fifties TV filter", () => postProcessing.FiftiesTv.Enabled = false);

            AddLabel("Fisheye filter");
            AddStep("Enable Fisheye filter", () => postProcessing.Fisheye.Enabled = true);
            AddSliderStep("Fisheye filter Intensity", 0, 1, 1f, v => postProcessing.Fisheye.Intensity = v);
            AddStep("Disable Fisheye filter", () => postProcessing.Fisheye.Enabled = false);

            AddLabel("Funk filter");
            AddStep("Enable Funk filter", () => postProcessing.Funk.Enabled = true);
            AddStep("Disable Funk filter", () => postProcessing.Funk.Enabled = false);

            AddLabel("Gaussian Blur filter");
            AddStep("Enable Gaussian Blur filter", () => postProcessing.GaussianBlur.Enabled = true);
            AddSliderStep("Gaussian Blur filter Intensity", 0, 1, 1f, v => postProcessing.GaussianBlur.Intensity = v);
            AddStep("Disable Gaussian Blur filter", () => postProcessing.GaussianBlur.Enabled = false);

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

            AddLabel("Handheld filter");
            AddStep("Enable Handheld filter", () => postProcessing.Handheld.Enabled = true);
            AddStep("Disable Handheld filter", () => postProcessing.Handheld.Enabled = false);

            AddLabel("Hexagon Black filter");
            AddStep("Enable Hexagon Black filter", () => postProcessing.HexagonBlack.Enabled = true);
            AddSliderStep("Hexagon Black filter Intensity", 0, 1, 1f, v => postProcessing.HexagonBlack.Intensity = v);
            AddStep("Disable Hexagon Black filter", () => postProcessing.HexagonBlack.Enabled = false);

            AddLabel("Invert filter");
            AddStep("Enable Invert filter", () => postProcessing.Invert.Enabled = true);
            AddStep("Disable Invert filter", () => postProcessing.Invert.Enabled = false);

            AddLabel("LED filter");
            AddStep("Enable LED filter", () => postProcessing.Led.Enabled = true);
            AddSliderStep("LED filter Intensity", 0, 1, 1f, v => postProcessing.Led.Intensity = v);
            AddStep("Disable LED filter", () => postProcessing.Led.Enabled = false);

            AddLabel("Light Water filter");
            AddStep("Enable Light Water filter", () => postProcessing.LightWater.Enabled = true);
            AddSliderStep("Light Water filter Intensity", 0, 1, 1f, v => postProcessing.LightWater.Intensity = v);
            AddStep("Disable Light Water filter", () => postProcessing.LightWater.Enabled = false);

            AddLabel("Neon filter");
            AddStep("Enable Neon filter", () => postProcessing.Neon.Enabled = true);
            AddStep("Disable Neon filter", () => postProcessing.Neon.Enabled = false);

            AddLabel("Night Vision filter");
            AddStep("Enable Night Vision filter", () => postProcessing.NightVision.Enabled = true);
            AddStep("Disable Night Vision filter", () => postProcessing.NightVision.Enabled = false);

            AddLabel("Oil Paint filter");
            AddStep("Enable Oil Paint filter", () => postProcessing.OilPaint.Enabled = true);
            AddSliderStep("Oil Paint filter Intensity", 0, 1, 1f, v => postProcessing.OilPaint.Intensity = v);
            AddStep("Disable Oil Paint filter", () => postProcessing.Pixelate.Enabled = false);

            AddLabel("Pixelate filter");
            AddStep("Enable Pixelate filter", () => postProcessing.Pixelate.Enabled = true);
            AddSliderStep("Pixelate filter Intensity", 0, 1, 1f, v => postProcessing.Pixelate.Intensity = v);
            AddStep("Disable Pixelate filter", () => postProcessing.Pixelate.Enabled = false);

            AddLabel("Pixel Snow filter");
            AddStep("Enable Pixel Snow filter", () => postProcessing.PixelSnow.Enabled = true);
            AddSliderStep("Pixel Snow filter Intensity", 0, 1, 1f, v => postProcessing.PixelSnow.Intensity = v);
            AddStep("Disable Pixel Snow filter", () => postProcessing.PixelSnow.Enabled = false);

            AddLabel("Posterize filter");
            AddStep("Enable Posterize filter", () => postProcessing.Posterize.Enabled = true);
            AddSliderStep("Posterize filter Intensity", 0, 1, 1f, v => postProcessing.Posterize.Intensity = v);
            AddStep("Disable Posterize filter", () => postProcessing.Posterize.Enabled = false);

            AddLabel("Rain filter");
            AddStep("Enable Rain filter", () => postProcessing.Rain.Enabled = true);
            AddSliderStep("Rain filter Intensity", 0, 1, 1f, v => postProcessing.Rain.Intensity = v);
            AddStep("Disable Rain filter", () => postProcessing.Rain.Enabled = false);

            AddLabel("Screen Scroll filter");
            AddStep("Enable Screen Scroll filter", () => postProcessing.ScreenScroll.Enabled = true);
            AddSliderStep("Screen Scroll filter Start.X", 0, 1, 1f, v => postProcessing.ScreenScroll.Start = postProcessing.ScreenScroll.Start with { X = v });
            AddSliderStep("Screen Scroll filter Start.Y", 0, 1, 1f, v => postProcessing.ScreenScroll.Start = postProcessing.ScreenScroll.Start with { Y = v });
            AddSliderStep("Screen Scroll filter Speed.X", 0, 1, 1f, v => postProcessing.ScreenScroll.Speed = postProcessing.ScreenScroll.Speed with { X = v });
            AddSliderStep("Screen Scroll filter Speed.Y", 0, 1, 1f, v => postProcessing.ScreenScroll.Speed = postProcessing.ScreenScroll.Speed with { Y = v });
            AddStep("Disable Screen Scroll filter", () => postProcessing.ScreenScroll.Enabled = false);

            AddLabel("Screen Tiling filter");
            AddStep("Enable Screen Tiling filter", () => postProcessing.ScreenScroll.Enabled = true);
            AddSliderStep("Screen Tiling filter Tiling.X", 0, 1, 1f, v => postProcessing.ScreenTiling.Tiling = postProcessing.ScreenTiling.Tiling with { X = v });
            AddSliderStep("Screen Tiling filter Tiling.Y", 0, 1, 1f, v => postProcessing.ScreenTiling.Tiling = postProcessing.ScreenTiling.Tiling with { Y = v });
            AddStep("Disable Screen Tiling filter", () => postProcessing.ScreenScroll.Enabled = false);

            AddLabel("Sepia filter");
            AddStep("Enable Sepia filter", () => postProcessing.Sepia.Enabled = true);
            AddSliderStep("Sepia filter Intensity", 0, 1, 1f, v => postProcessing.Sepia.Intensity = v);
            AddStep("Disable Sepia filter", () => postProcessing.Sepia.Enabled = false);

            AddLabel("Sharpen filter");
            AddStep("Enable Sharpen filter", () => postProcessing.Sharpen.Enabled = true);
            AddSliderStep("Sharpen filter Intensity", 0, 1, 1f, v => postProcessing.Sharpen.Intensity = v);
            AddStep("Disable Sharpen filter", () => postProcessing.Sharpen.Enabled = false);

            AddLabel("Static filter");
            AddStep("Enable Static filter", () => postProcessing.Static.Enabled = true);
            AddSliderStep("Static filter Intensity", 0, 1, 1f, v => postProcessing.Static.Intensity = v);
            AddStep("Disable Static filter", () => postProcessing.Static.Enabled = false);

            AddLabel("Super Dot filter");
            AddStep("Enable Super Dot filter", () => postProcessing.SuperDot.Enabled = true);
            AddStep("Disable Super Dot filter", () => postProcessing.SuperDot.Enabled = false);

            AddLabel("Tunnel filter");
            AddStep("Enable Tunnel filter", () => postProcessing.Tunnel.Enabled = true);
            AddStep("Disable Tunnel filter", () => postProcessing.Tunnel.Enabled = false);

            AddLabel("VHS filter");
            AddStep("Enable VHS filter", () => postProcessing.Vhs.Enabled = true);
            AddSliderStep("VHS filter Intensity", 0, 1, 1f, v => postProcessing.Vhs.Intensity = v);
            AddStep("Disable VHS filter", () => postProcessing.Vhs.Enabled = false);

            AddLabel("Water Drop filter");
            AddStep("Enable Water Drop filter", () => postProcessing.WaterDrop.Enabled = true);
            AddSliderStep("Water Drop filter Intensity", 0, 1, 1f, v => postProcessing.WaterDrop.Intensity = v);
            AddStep("Disable Water Drop filter", () => postProcessing.WaterDrop.Enabled = false);

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
