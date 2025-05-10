#nullable disable

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Circle.Game.Beatmaps;
using Circle.Game.Graphics;
using Circle.Game.Graphics.Containers;
using Circle.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Logging;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Overlays
{
    public partial class ConvertOverlay : CircleFocusedOverlayContainer
    {
        private readonly Box background;
        private readonly Box fileBackground;
        private readonly TextFlowContainer text;
        private readonly CircularProgress circularProgress;

        private readonly List<DirectoryInfo> levels;

        [Resolved]
        private BeatmapManager manager { get; set; }

        public ConvertOverlay()
        {
            CircleDirectorySelector directorySelector;
            levels = new List<DirectoryInfo>();
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
                    directorySelector = new CircleDirectorySelector
                    {
                        RelativeSizeAxes = Axes.Both,
                        Width = 0.6f,
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
                                        circularProgress = new CircularProgress
                                        {
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Size = new Vector2(75),
                                            Colour = Color4.DeepSkyBlue,
                                            InnerRadius = 0.2f,
                                            RoundedCaps = true
                                        },
                                        text = new TextFlowContainer(t => t.Font = CircleFont.Default.With(size: 32))
                                        {
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            AutoSizeAxes = Axes.Y,
                                            RelativeSizeAxes = Axes.X,
                                            Text = "Select a ADOFAI level path",
                                            TextAnchor = Anchor.Centre,
                                        }
                                    }
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
                                        Text = "Start convert",
                                        Action = () => startConvert(directorySelector.CurrentPath.Value)
                                    }
                                }
                            },
                        }
                    }
                }
            };

            directorySelector.CurrentPath.BindValueChanged(pathChanged);
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

        [BackgroundDependencyLoader]
        private void load(CircleColour colours)
        {
            background.Colour = colours.TransparentGray;
            fileBackground.Colour = colours.TransparentBlack;
        }

        private void pathChanged(ValueChangedEvent<DirectoryInfo> path)
        {
            levels.Clear();
            var directories = path.NewValue?.GetDirectories("*", new EnumerationOptions()) ?? Array.Empty<DirectoryInfo>();

            foreach (var dir in directories)
            {
                try
                {
                    foreach (var file in dir.GetFiles("*.adofai"))
                    {
                        if (file.Name == "backup.adofai")
                            continue;

                        levels.Add(dir);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Error trying to finding adofai file");
                }
            }

            text.Text = levels.Count != 0 ? $"Founded {levels.Count}level!" : "Select a ADOFAI level path";
        }

        private void startConvert(DirectoryInfo di)
        {
            if (!di.Exists)
                return;

            var progress = new Bindable<int>();
            progress.ValueChanged += progressChanged;

            Task.Factory.StartNew(() => manager.ConvertWithImport(levels.ToArray(), progress), TaskCreationOptions.LongRunning);
        }

        private void progressChanged(ValueChangedEvent<int> value)
        {
            Logger.Log($"{value.NewValue}, {(double)value.NewValue / levels.Count}");
            Schedule(() => circularProgress.ProgressTo((double)value.NewValue / levels.Count, 750, Easing.OutPow10));
        }
    }
}
