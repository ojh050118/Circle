using System.IO;
using System.Threading.Tasks;
using Circle.Game.Beatmaps;
using Circle.Game.Graphics;
using Circle.Game.Graphics.Containers;
using Circle.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Overlays
{
    public class ImportOverlay : CircleFocusedOverlayContainer
    {
        private readonly CircleFileSelector fileSelector;

        private readonly SpriteIcon icon;
        private readonly TextFlowContainer text;

        private readonly Box background;
        private readonly Box fileBackground;

        [Resolved]
        private BeatmapManager manager { get; set; }

        public ImportOverlay()
        {
            Content = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(0.8f),
                Masking = true,
                CornerRadius = 10,
                Scale = new Vector2(1.2f),
                Children = new Drawable[]
                {
                    background = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                    },
                    fileSelector = new CircleFileSelector(validFileExtensions: new[] { ".circle", ".circlez" })
                    {
                        RelativeSizeAxes = Axes.Both,
                        Width = 0.6f
                    },
                    new Container
                    {
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight,
                        RelativeSizeAxes = Axes.Both,
                        Width = 0.4f,
                        Masking = true,
                        CornerRadius = 10,
                        Children = new Drawable[]
                        {
                            fileBackground = new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                            },
                            new CircleScrollContainer
                            {
                                RelativeSizeAxes = Axes.Both,
                                Child = new FillFlowContainer
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Spacing = new Vector2(10),
                                    Direction = FillDirection.Vertical,
                                    Padding = new MarginPadding(10),
                                    AutoSizeAxes = Axes.Y,
                                    RelativeSizeAxes = Axes.X,
                                    Children = new Drawable[]
                                    {
                                        icon = new SpriteIcon
                                        {
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Size = new Vector2(50),
                                            Icon = FontAwesome.Solid.File,
                                            Alpha = 0
                                        },
                                        text = new TextFlowContainer(t => t.Font = FontUsage.Default.With(size: 32))
                                        {
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            AutoSizeAxes = Axes.Y,
                                            RelativeSizeAxes = Axes.X,
                                            Text = "Select a beatmap file",
                                            TextAnchor = Anchor.Centre,
                                        }
                                    },
                                },
                                ScrollContent =
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre
                                }
                            },
                            new FillFlowContainer<BoxButton>
                            {
                                Anchor = Anchor.BottomCentre,
                                Origin = Anchor.BottomCentre,
                                Spacing = new Vector2(10),
                                Direction = FillDirection.Vertical,
                                Padding = new MarginPadding(10),
                                AutoSizeAxes = Axes.Y,
                                RelativeSizeAxes = Axes.X,
                                Children = new[]
                                {
                                    new BoxButton
                                    {
                                        Text = "Exit",
                                        Action = Hide
                                    },
                                    new BoxButton
                                    {
                                        Text = "Import all",
                                        Action = () =>
                                        {
                                            foreach (var circlez in fileSelector.CurrentPath.Value?.GetFiles(@"*.circlez")!)
                                                startImport(circlez.FullName);
                                        }
                                    },
                                    new BoxButton
                                    {
                                        Text = "Import",
                                        Action = () => startImport(fileSelector.CurrentFile.Value?.FullName)
                                    }
                                }
                            },
                        }
                    }
                }
            };

            fileSelector.CurrentPath.BindValueChanged(pathChanged);
            fileSelector.CurrentFile.BindValueChanged(fileChanged);
        }

        [BackgroundDependencyLoader]
        private void load(CircleColour colours)
        {
            background.Colour = colours.TransparentGray;
            fileBackground.Colour = colours.TransparentBlack;
        }

        private void pathChanged(ValueChangedEvent<DirectoryInfo> path)
        {
            fileSelector.CurrentFile.Value = null;
        }

        private void fileChanged(ValueChangedEvent<FileInfo> selectedFile)
        {
            icon.Alpha = selectedFile.NewValue == null ? 0 : 1;
            text.Text = selectedFile.NewValue?.Name ?? "Select a beatmap file";
        }

        private void startImport(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            Task.Factory.StartNew(() => manager.Import(path), TaskCreationOptions.LongRunning);
        }

        protected override void PopIn()
        {
            base.PopIn();

            Content.ScaleTo(1, 1000, Easing.OutPow10);
            Content.FadeTo(0.8f, 1000, Easing.OutPow10);
        }

        protected override void PopOut()
        {
            base.PopOut();

            Content.ScaleTo(1.2f, 1000, Easing.OutPow10);
            Content.FadeOut(1000, Easing.OutPow10);
        }
    }
}
