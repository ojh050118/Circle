#nullable disable

using Circle.Game.Beatmaps;
using Circle.Game.Rulesets.Graphics;
using Circle.Game.Rulesets.Objects;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Transforms;

namespace Circle.Game.Rulesets.Extensions
{
    public static class CircleTransformableExtensions
    {
        public static TransformSequence<T> ExpandTo<T>(this T planet, float value, double duration = 0, Easing easing = Easing.None)
            where T : Planet
        {
            return planet.TransformTo(nameof(planet.Expansion), value, duration, easing);
        }

        public static TransformSequence<T> FilterTo<T>(this T postprocessing, FilterType filterType, float intensity = 100, double duration = 0, Easing easing = Easing.None)
            where T : PostProcessingContainer
        {
            switch (filterType)
            {
                case FilterType.Grayscale:
                    return postprocessing.TransformTo(nameof(postprocessing.Grayscale.Intensity), intensity, duration, easing);

                case FilterType.Sepia:
                    return postprocessing.TransformTo(nameof(postprocessing.Sepia.Intensity), intensity, duration, easing);

                case FilterType.Invert:
                    return postprocessing.TransformTo(nameof(postprocessing.Invert.Enabled), intensity != 0, duration, easing);

                case FilterType.Vhs:
                    return postprocessing.TransformTo(nameof(postprocessing.Vhs.Intensity), intensity, duration, easing);

                case FilterType.EightiesTv:
                    return postprocessing.TransformTo(nameof(postprocessing.EightiesTv.Enabled), intensity != 0, duration, easing);

                case FilterType.FiftiesTv:
                    return postprocessing.TransformTo(nameof(postprocessing.FiftiesTv.Enabled), intensity != 0, duration, easing);

                case FilterType.Arcade:
                    return postprocessing.TransformTo(nameof(postprocessing.Arcade.Enabled), intensity != 0, duration, easing);

                case FilterType.Led:
                    return postprocessing.TransformTo(nameof(postprocessing.Led.Intensity), intensity, duration, easing);

                case FilterType.Rain:
                    return postprocessing.TransformTo(nameof(postprocessing.Rain.Intensity), intensity, duration, easing);

                case FilterType.Blizzard:
                    return postprocessing.TransformTo(nameof(postprocessing.Blizzard.Intensity), intensity, duration, easing);

                case FilterType.PixelSnow:
                    return postprocessing.TransformTo(nameof(postprocessing.PixelSnow.Intensity), intensity, duration, easing);

                case FilterType.Compression:
                    return postprocessing.TransformTo(nameof(postprocessing.Compression.Intensity), intensity, duration, easing);

                case FilterType.Glitch:
                    return postprocessing.TransformTo(nameof(postprocessing.Glitch.Enabled), intensity != 0, duration, easing);

                case FilterType.Pixelate:
                    return postprocessing.TransformTo(nameof(postprocessing.Pixelate.Intensity), intensity, duration, easing);

                case FilterType.Waves:
                    return postprocessing.TransformTo(nameof(postprocessing.Waves.Intensity), intensity, duration, easing);

                case FilterType.Static:
                    return postprocessing.TransformTo(nameof(postprocessing.Static.Intensity), intensity, duration, easing);

                case FilterType.Grain:
                    return postprocessing.TransformTo(nameof(postprocessing.Grain.Intensity), intensity, duration, easing);

                case FilterType.MotionBlur:
                    break;

                case FilterType.Fisheye:
                    return postprocessing.TransformTo(nameof(postprocessing.Fisheye.Intensity), intensity, duration, easing);

                case FilterType.Aberration:
                    return postprocessing.TransformTo(nameof(postprocessing.Aberration.Intensity), intensity, duration, easing);

                case FilterType.Drawing:
                    return postprocessing.TransformTo(nameof(postprocessing.Drawing.Intensity), intensity, duration, easing);

                case FilterType.Neon:
                    return postprocessing.TransformTo(nameof(postprocessing.Neon), intensity != 0, duration, easing);

                case FilterType.Handheld:
                    return postprocessing.TransformTo(nameof(postprocessing.Handheld.Enabled), intensity != 0, duration, easing);

                case FilterType.NightVision:
                    return postprocessing.TransformTo(nameof(postprocessing.NightVision), intensity != 0, duration, easing);

                case FilterType.Funk:
                    return postprocessing.TransformTo(nameof(postprocessing.Funk.Enabled), intensity != 0, duration, easing);

                case FilterType.Tunnel:
                    // TODO: 아직 잘 작동하지 않음
                    break;

                case FilterType.Weird3D:
                    return postprocessing.TransformTo(nameof(postprocessing.Weird3D), intensity != 0, duration, easing);

                case FilterType.Blur:
                    return postprocessing.TransformTo(nameof(postprocessing.Blur.Intensity), intensity, duration, easing);

                case FilterType.BlurFocus:
                    return postprocessing.TransformTo(nameof(postprocessing.BlurFocus.Intensity), intensity, duration, easing);

                case FilterType.GaussianBlur:
                    return postprocessing.TransformTo(nameof(postprocessing.GaussianBlur.Intensity), intensity, duration, easing);

                case FilterType.HexagonBlack:
                    return postprocessing.TransformTo(nameof(postprocessing.HexagonBlack.Intensity), intensity, duration, easing);

                case FilterType.Posterize:
                    return postprocessing.TransformTo(nameof(postprocessing.Posterize.Intensity), intensity, duration, easing);

                case FilterType.Sharpen:
                    return postprocessing.TransformTo(nameof(postprocessing.Sharpen.Intensity), intensity, duration, easing);

                case FilterType.Contrast:
                    return postprocessing.TransformTo(nameof(postprocessing.Contrast.Intensity), intensity, duration, easing);

                case FilterType.EdgeBlackLine:
                    return postprocessing.TransformTo(nameof(postprocessing.EdgeBlackLine), intensity != 0, duration, easing);

                case FilterType.OilPaint:
                    return postprocessing.TransformTo(nameof(postprocessing.OilPaint.Intensity), intensity, duration, easing);

                case FilterType.SuperDot:
                    return postprocessing.TransformTo(nameof(postprocessing.SuperDot.Enabled), intensity != 0, duration, easing);

                case FilterType.WaterDrop:
                    break;

                case FilterType.LightWater:
                    return postprocessing.TransformTo(nameof(postprocessing.LightWater.Intensity), intensity, duration, easing);
            }

            return postprocessing.TransformTo(nameof(postprocessing.Empty), true, duration, easing);
        }
    }
}
