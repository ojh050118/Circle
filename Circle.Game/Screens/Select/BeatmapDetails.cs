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
        private readonly SpriteText title;
        private readonly SpriteText artist;

        private readonly SpriteText author;
        private readonly SpriteText bpm;
        private readonly SpriteText difficulty;
        private readonly TextFlowContainer description;

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
                                Child = new FillFlowContainer
                                {
                                    Direction = FillDirection.Vertical,
                                    Spacing = new Vector2(5),
                                    Padding = new MarginPadding { Horizontal = 10, Vertical = 5 },
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Children = new Drawable[]
                                    {
                                        new SpriteText
                                        {
                                            Text = "Details",
                                            Margin = new MarginPadding { Bottom = 10 },
                                            Font = FontUsage.Default.With("OpenSans-Bold", 30)
                                        },
                                        author = new SpriteText
                                        {
                                            Text = "Author: ",
                                            Font = FontUsage.Default.With(size: 24)
                                        },
                                        bpm = new SpriteText
                                        {
                                            Text = "BPM: ",
                                            Font = FontUsage.Default.With(size: 24)
                                        },
                                        difficulty = new SpriteText
                                        {
                                            Text = "Difficulty: ",
                                            Font = FontUsage.Default.With(size: 24)
                                        },
                                        description = new TextFlowContainer
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                        }
                                    }
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

            description.AddText("Description: ", t => t.Font = FontUsage.Default.With(size: 24));
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            workingBeatmap.BindValueChanged(onBeatmapChanged);
            workingBeatmap.TriggerChange();
        }

        public void ChangeBeatmap(BeatmapInfo beatmap) => workingBeatmap.Value = beatmap;

        private void onBeatmapChanged(ValueChangedEvent<BeatmapInfo> beatmap)
        {
            if (string.IsNullOrEmpty(beatmap.NewValue.Settings.BgImage))
                background.ChangeTexture(TextureSource.Internal, "Duelyst", 500, Easing.Out);
            else
                background.ChangeTexture(TextureSource.External, beatmap.NewValue.Settings.BgImage, 500, Easing.Out);

            if (!string.IsNullOrEmpty(beatmap.NewValue.Settings.Song) && !string.IsNullOrEmpty(beatmap.NewValue.Settings.Artist))
            {
                title.Text = beatmap.NewValue.Settings.Song;
                artist.Text = beatmap.NewValue.Settings.Artist;
            }

            author.Text = $"Author: {beatmap.NewValue.Settings.Author}";
            bpm.Text = $"BPM: {beatmap.NewValue.Settings.Bpm}";
            difficulty.Text = $"Difficulty: {beatmap.NewValue.Settings.Difficulty}";
            description.Clear();
            description.AddText($"Description: {beatmap.NewValue.Settings.BeatmapDesc}", t => t.Font = FontUsage.Default.With(size: 24));
        }
    }
}
