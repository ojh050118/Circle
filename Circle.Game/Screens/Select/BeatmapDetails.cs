using Circle.Game.Beatmap;
using Circle.Game.Graphics.Containers;
using Circle.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Screens.Select
{
    public class BeatmapDetails : Container
    {
        [Resolved]
        private Bindable<BeatmapInfo> workingBeatmap { get; set; }

        private readonly Background background;
        private readonly FillFlowContainer details;
        private readonly SpriteText title;
        private readonly SpriteText artist;

        public BeatmapDetails()
        {
            RelativeSizeAxes = Axes.Both;
            Child = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                CornerRadius = 10,
                Children = new Drawable[]
                {
                    new Box
                    {
                        Colour = Color4.White.Opacity(0.2f),
                        RelativeSizeAxes = Axes.Both,
                    },
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Direction = FillDirection.Vertical,
                        Masking = true,
                        Children = new Drawable[]
                        {
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Height = 0.4f,
                                Masking = true,
                                CornerRadius = 5,
                                Children = new Drawable[]
                                {
                                    background = new Background(),
                                    new FillFlowContainer
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Direction = FillDirection.Horizontal,
                                        // 이렇게 하면 그래디언트가 수평이 아니라 ~40° 각도로 대각선이 됩니다.
                                        Shear = new Vector2(0.8f, 0),
                                        Alpha = 0.5f,
                                        Children = new[]
                                        {
                                            new Box
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                Colour = Color4.Black,
                                                Width = 0.4f,
                                            },
                                            new Box
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                Colour = ColourInfo.GradientHorizontal(Color4.Black, new Color4(0f, 0f, 0f, 0.9f)),
                                                Width = 0.05f,
                                            },
                                            new Box
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                Colour = ColourInfo.GradientHorizontal(new Color4(0f, 0f, 0f, 0.9f), new Color4(0f, 0f, 0f, 0.1f)),
                                                Width = 0.2f,
                                            },
                                            new Box
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                Colour = ColourInfo.GradientHorizontal(new Color4(0f, 0f, 0f, 0.1f), new Color4(0, 0, 0, 0)),
                                                Width = 0.05f,
                                            },
                                        }
                                    },
                                    new FillFlowContainer
                                    {
                                        AutoSizeAxes = Axes.Both,
                                        Direction = FillDirection.Vertical,
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        Margin = new MarginPadding { Left = 10 },
                                        Y = 28,
                                        Children = new Drawable[]
                                        {
                                            title = new SpriteText
                                            {
                                                Text = "no beatmaps available!",
                                                Font = FontUsage.Default.With(size: 28)
                                            },
                                            artist = new SpriteText
                                            {
                                                Text = "please load a beatmap!",
                                                Font = FontUsage.Default.With(size: 18)
                                            }
                                        }
                                    }
                                }
                            },
                            new CircleScrollContainer
                            {
                                RelativeSizeAxes = Axes.Both,
                                Height = 0.6f,
                                Child = details = new FillFlowContainer
                                {
                                    Direction = FillDirection.Vertical,
                                    Spacing = new Vector2(5),
                                    Padding = new MarginPadding { Horizontal = 10, Vertical = 5 },
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                }
                            }
                        }
                    }
                },
                EdgeEffect = new EdgeEffectParameters
                {
                    Type = EdgeEffectType.Shadow,
                    Radius = 10,
                    Colour = Color4.Black.Opacity(100)
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            workingBeatmap.ValueChanged += v =>
            {
                if (string.IsNullOrEmpty(v.NewValue.Settings.BgImage))
                    background.FadeTextureTo(TextureSource.Internal, "Duelyst", 500, Easing.Out);
                else
                    background.FadeTextureTo(TextureSource.External, v.NewValue.Settings.BgImage, 500, Easing.Out);

                if (!string.IsNullOrEmpty(v.NewValue.Settings.Song) && !string.IsNullOrEmpty(v.NewValue.Settings.Artist))
                {
                    title.Text = v.NewValue.Settings.Song;
                    artist.Text = v.NewValue.Settings.Artist;
                }

                details.Children = new Drawable[]
                {
                    new SpriteText
                    {
                        Text = "Details",
                        Margin = new MarginPadding { Bottom = 10 },
                        Font = FontUsage.Default.With("OpenSans-Bold", 30)
                    },
                    new SpriteText
                    {
                        Text = $"Author: {v.NewValue.Settings.Author}",
                        Font = FontUsage.Default.With(size: 24)
                    },
                    new SpriteText
                    {
                        Text = $"BPM: {v.NewValue.Settings.Bpm}",
                        Font = FontUsage.Default.With(size: 24)
                    },
                    new SpriteText
                    {
                        Text = $"Difficulty: {v.NewValue.Settings.Difficulty}",
                        Font = FontUsage.Default.With(size: 24)
                    },
                    new SpriteText
                    {
                        Text = $"Description: {v.NewValue.Settings.BeatmapDesc}",
                        Font = FontUsage.Default.With(size: 24)
                    },
                };
            };

            workingBeatmap.TriggerChange();
        }
    }
}
