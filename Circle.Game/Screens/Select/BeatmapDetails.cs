#nullable disable

using Circle.Game.Beatmaps;
using Circle.Game.Graphics;
using Circle.Game.Graphics.Containers;
using Circle.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Platform;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Screens.Select
{
    public partial class BeatmapDetails : Container
    {
        private SpriteText artist;

        private SpriteText author;
        private SpriteText bpm;
        private TextFlowContainer description;
        private SpriteText difficulty;

        private Storage files;
        private Background preview;
        private SpriteText title;

        [BackgroundDependencyLoader]
        private void load(BeatmapStorage beatmaps, CircleColour colours, Background background)
        {
            files = beatmaps.Storage;
            RelativeSizeAxes = Axes.Both;
            Child = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                CornerRadius = 10,
                Children = new Drawable[]
                {
                    new BackgroundColorContainer(background)
                    {
                        RelativeSizeAxes = Axes.Both,
                        ChangeDuration = 500,
                        ChangeEasing = Easing.Out,
                        Child = new Box
                        {
                            Alpha = 0.7f,
                            Colour = Color4.DarkGray,
                            RelativeSizeAxes = Axes.Both,
                        }
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
                                    preview = new Background(),
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
                                            Font = FontUsage.Default.With("OpenSans-Bold", 30),
                                            Shadow = true,
                                            ShadowColour = colours.TransparentBlack,
                                        },
                                        author = new SpriteText
                                        {
                                            Text = "Author: ",
                                            Font = FontUsage.Default.With(size: 24),
                                            Shadow = true,
                                            ShadowColour = colours.TransparentBlack,
                                        },
                                        bpm = new SpriteText
                                        {
                                            Text = "BPM: ",
                                            Font = FontUsage.Default.With(size: 24),
                                            Shadow = true,
                                            ShadowColour = colours.TransparentBlack,
                                        },
                                        difficulty = new SpriteText
                                        {
                                            Text = "Difficulty: ",
                                            Font = FontUsage.Default.With(size: 24),
                                            Shadow = true,
                                            ShadowColour = colours.TransparentBlack,
                                        },
                                        description = new TextFlowContainer(t =>
                                        {
                                            t.Font = FontUsage.Default.With(size: 24);
                                            t.Shadow = true;
                                            t.ShadowColour = colours.TransparentBlack;
                                        })
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                            Text = "Description: "
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
        }

        public void ChangeBeatmap(BeatmapInfo newBeatmapInfo)
        {
            if (newBeatmapInfo == null)
                return;

            var newBeatmap = newBeatmapInfo.Beatmap;

            if (!files.Exists(newBeatmapInfo.RelativeBackgroundPath))
                preview.ChangeTexture(TextureSource.Internal, "bg1", 500, Easing.Out);
            else
                preview.ChangeTexture(TextureSource.External, newBeatmapInfo.RelativeBackgroundPath, 500, Easing.Out);

            title.Text = newBeatmap.Settings.Song;
            artist.Text = newBeatmap.Settings.Artist;
            author.Text = $"Author: {newBeatmap.Settings.Author}";
            bpm.Text = $"BPM: {newBeatmap.Settings.Bpm}";
            difficulty.Text = $"Difficulty: {newBeatmap.Settings.Difficulty}";
            description.Text = $"Description: {newBeatmap.Settings.BeatmapDesc}";
        }
    }
}
