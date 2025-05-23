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

        public static TransformSequence<T> FilterToggleTo<T>(this T postprocessing, FilterType filterType, bool enabled)
            where T : PostProcessingContainer
        {
            switch (filterType)
            {
                case FilterType.Grayscale:
                    return postprocessing.TransformTo(nameof(postprocessing.GrayscaleEnabled), enabled ? 1f : 0);

                case FilterType.Sepia:
                    return postprocessing.TransformTo(nameof(postprocessing.SepiaEnabled), enabled ? 1f : 0);

                case FilterType.Invert:
                    return postprocessing.TransformTo(nameof(postprocessing.InvertEnabled), enabled ? 1f : 0);

                case FilterType.Vhs:
                    return postprocessing.TransformTo(nameof(postprocessing.VhsEnabled), enabled ? 1f : 0);

                case FilterType.EightiesTv:
                    return postprocessing.TransformTo(nameof(postprocessing.EightiesTvEnabled), enabled ? 1f : 0);

                case FilterType.FiftiesTv:
                    return postprocessing.TransformTo(nameof(postprocessing.FiftiesTvEnabled), enabled ? 1f : 0);

                case FilterType.Arcade:
                    return postprocessing.TransformTo(nameof(postprocessing.ArcadeEnabled), enabled ? 1f : 0);

                case FilterType.Led:
                    return postprocessing.TransformTo(nameof(postprocessing.LedEnabled), enabled ? 1f : 0);

                case FilterType.Rain:
                    return postprocessing.TransformTo(nameof(postprocessing.RainEnabled), enabled ? 1f : 0);

                case FilterType.Blizzard:
                    return postprocessing.TransformTo(nameof(postprocessing.BlizzardEnabled), enabled ? 1f : 0);

                case FilterType.PixelSnow:
                    return postprocessing.TransformTo(nameof(postprocessing.PixelSnowEnabled), enabled ? 1f : 0);

                case FilterType.Compression:
                    return postprocessing.TransformTo(nameof(postprocessing.CompressionEnabled), enabled ? 1f : 0);

                case FilterType.Glitch:
                    return postprocessing.TransformTo(nameof(postprocessing.GlitchEnabled), enabled ? 1f : 0);

                case FilterType.Pixelate:
                    return postprocessing.TransformTo(nameof(postprocessing.PixelateEnabled), enabled ? 1f : 0);

                case FilterType.Waves:
                    return postprocessing.TransformTo(nameof(postprocessing.WavesEnabled), enabled ? 1f : 0);

                case FilterType.Static:
                    return postprocessing.TransformTo(nameof(postprocessing.StaticEnabled), enabled ? 1f : 0);

                case FilterType.Grain:
                    return postprocessing.TransformTo(nameof(postprocessing.GrainEnabled), enabled ? 1f : 0);

                case FilterType.MotionBlur:
                    break;

                case FilterType.Fisheye:
                    return postprocessing.TransformTo(nameof(postprocessing.FisheyeEnabled), enabled ? 1f : 0);

                case FilterType.Aberration:
                    return postprocessing.TransformTo(nameof(postprocessing.AberrationEnabled), enabled ? 1f : 0);

                case FilterType.Drawing:
                    return postprocessing.TransformTo(nameof(postprocessing.DrawingEnabled), enabled ? 1f : 0);

                case FilterType.Neon:
                    return postprocessing.TransformTo(nameof(postprocessing.NeonEnabled), enabled ? 1f : 0);

                case FilterType.Handheld:
                    return postprocessing.TransformTo(nameof(postprocessing.HandheldEnabled), enabled ? 1f : 0);

                case FilterType.NightVision:
                    return postprocessing.TransformTo(nameof(postprocessing.NightVisionEnabled), enabled ? 1f : 0);

                case FilterType.Funk:
                    return postprocessing.TransformTo(nameof(postprocessing.FunkEnabled), enabled ? 1f : 0);

                case FilterType.Tunnel:
                    return postprocessing.TransformTo(nameof(postprocessing.TunnelEnabled), enabled ? 1f : 0);

                case FilterType.Weird3D:
                    return postprocessing.TransformTo(nameof(postprocessing.Weird3DEnabled), enabled ? 1f : 0);

                case FilterType.Blur:
                    return postprocessing.TransformTo(nameof(postprocessing.BlurEnabled), enabled ? 1f : 0);

                case FilterType.BlurFocus:
                    return postprocessing.TransformTo(nameof(postprocessing.BlurFocusEnabled), enabled ? 1f : 0);

                case FilterType.GaussianBlur:
                    return postprocessing.TransformTo(nameof(postprocessing.GaussianBlurEnabled), enabled ? 1f : 0);

                case FilterType.HexagonBlack:
                    return postprocessing.TransformTo(nameof(postprocessing.HexagonBlackEnabled), enabled ? 1f : 0);

                case FilterType.Posterize:
                    return postprocessing.TransformTo(nameof(postprocessing.PosterizeEnabled), enabled ? 1f : 0);

                case FilterType.Sharpen:
                    return postprocessing.TransformTo(nameof(postprocessing.SharpenEnabled), enabled ? 1f : 0);

                case FilterType.Contrast:
                    return postprocessing.TransformTo(nameof(postprocessing.ContrastEnabled), enabled ? 1f : 0);

                case FilterType.EdgeBlackLine:
                    return postprocessing.TransformTo(nameof(postprocessing.EdgeBlackLineEnabled), enabled ? 1f : 0);

                case FilterType.OilPaint:
                    return postprocessing.TransformTo(nameof(postprocessing.OilPaintEnabled), enabled ? 1f : 0);

                case FilterType.SuperDot:
                    return postprocessing.TransformTo(nameof(postprocessing.SuperDotEnabled), enabled ? 1f : 0);

                case FilterType.WaterDrop:
                    return postprocessing.TransformTo(nameof(postprocessing.WaterDropEnabled), enabled ? 1f : 0);

                case FilterType.LightWater:
                    return postprocessing.TransformTo(nameof(postprocessing.LightWaterEnabled), enabled ? 1f : 0);
            }

            return null;
        }

        public static TransformSequence<T> FilterTo<T>(this T postprocessing, FilterType filterType, float intensity = 100, double duration = 0, Easing easing = Easing.None)
            where T : PostProcessingContainer
        {
            switch (filterType)
            {
                case FilterType.Grayscale:
                    return postprocessing.TransformTo(nameof(postprocessing.GrayscaleIntensity), intensity, duration, easing);

                case FilterType.Sepia:
                    return postprocessing.TransformTo(nameof(postprocessing.SepiaIntensity), intensity, duration, easing);

                case FilterType.Invert:
                    break;

                case FilterType.Vhs:
                    return postprocessing.TransformTo(nameof(postprocessing.VhsIntensity), intensity, duration, easing);

                case FilterType.EightiesTv:
                    break;

                case FilterType.FiftiesTv:
                    break;

                case FilterType.Arcade:
                    break;

                case FilterType.Led:
                    return postprocessing.TransformTo(nameof(postprocessing.LedIntensity), intensity, duration, easing);

                case FilterType.Rain:
                    return postprocessing.TransformTo(nameof(postprocessing.RainIntensity), intensity, duration, easing);

                case FilterType.Blizzard:
                    return postprocessing.TransformTo(nameof(postprocessing.BlizzardIntensity), intensity, duration, easing);

                case FilterType.PixelSnow:
                    return postprocessing.TransformTo(nameof(postprocessing.PixelSnowIntensity), intensity, duration, easing);

                case FilterType.Compression:
                    return postprocessing.TransformTo(nameof(postprocessing.CompressionIntensity), intensity, duration, easing);

                case FilterType.Glitch:
                    break;

                case FilterType.Pixelate:
                    return postprocessing.TransformTo(nameof(postprocessing.PixelateIntensity), intensity, duration, easing);

                case FilterType.Waves:
                    return postprocessing.TransformTo(nameof(postprocessing.WavesIntensity), intensity, duration, easing);

                case FilterType.Static:
                    return postprocessing.TransformTo(nameof(postprocessing.StaticIntensity), intensity, duration, easing);

                case FilterType.Grain:
                    return postprocessing.TransformTo(nameof(postprocessing.GrainIntensity), intensity, duration, easing);

                case FilterType.MotionBlur:
                    break;

                case FilterType.Fisheye:
                    return postprocessing.TransformTo(nameof(postprocessing.FisheyeIntensity), intensity, duration, easing);

                case FilterType.Aberration:
                    return postprocessing.TransformTo(nameof(postprocessing.AberrationIntensity), intensity, duration, easing);

                case FilterType.Drawing:
                    return postprocessing.TransformTo(nameof(postprocessing.DrawingIntensity), intensity, duration, easing);

                case FilterType.Neon:
                    break;

                case FilterType.Handheld:
                    break;

                case FilterType.NightVision:
                    break;

                case FilterType.Funk:
                    break;

                case FilterType.Tunnel:
                    break;

                case FilterType.Weird3D:
                    break;

                case FilterType.Blur:
                    return postprocessing.TransformTo(nameof(postprocessing.BlurIntensity), intensity, duration, easing);

                case FilterType.BlurFocus:
                    return postprocessing.TransformTo(nameof(postprocessing.BlurFocusIntensity), intensity, duration, easing);

                case FilterType.GaussianBlur:
                    return postprocessing.TransformTo(nameof(postprocessing.GaussianBlurIntensity), intensity, duration, easing);

                case FilterType.HexagonBlack:
                    return postprocessing.TransformTo(nameof(postprocessing.HexagonBlackIntensity), intensity, duration, easing);

                case FilterType.Posterize:
                    return postprocessing.TransformTo(nameof(postprocessing.PosterizeIntensity), intensity, duration, easing);

                case FilterType.Sharpen:
                    return postprocessing.TransformTo(nameof(postprocessing.SharpenIntensity), intensity, duration, easing);

                case FilterType.Contrast:
                    return postprocessing.TransformTo(nameof(postprocessing.ContrastIntensity), intensity, duration, easing);

                case FilterType.EdgeBlackLine:
                    break;

                case FilterType.OilPaint:
                    return postprocessing.TransformTo(nameof(postprocessing.OilPaintIntensity), intensity, duration, easing);

                case FilterType.SuperDot:
                    break;

                case FilterType.WaterDrop:
                    return postprocessing.TransformTo(nameof(postprocessing.WaterDropIntensity), intensity, duration, easing);

                case FilterType.LightWater:
                    return postprocessing.TransformTo(nameof(postprocessing.LightWaterIntensity), intensity, duration, easing);
            }

            return null;
        }
    }
}
