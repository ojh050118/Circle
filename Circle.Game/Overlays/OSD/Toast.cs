using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Overlays.OSD
{
    public class Toast : Container
    {
        private const int toast_minimum_width = 300;
        private const int toast_default_padding = 15;

        public int ToastQueueCount;

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.X;
            Height = 100;
        }

        public void Push(ToastInfo info)
        {
            CircularContainer toast;
            int delay = 2000 * ToastQueueCount;
            ToastQueueCount++;

            Schedule(() =>
            {
                Add(toast = new CircularContainer
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Masking = true,
                    AutoSizeAxes = Axes.X,
                    Height = 80,
                    Y = -100,
                    Margin = new MarginPadding { Top = toast_default_padding },
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            RelativeSizeAxes = Axes.Y,
                            Width = toast_minimum_width,
                        },
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Color4.White
                        },
                        new Container
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Size = new Vector2(80),
                            Padding = new MarginPadding(toast_default_padding + 5),
                            Child = new SpriteIcon
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                RelativeSizeAxes = Axes.Both,
                                Colour = info.IconColour,
                                Icon = info.Icon
                            }
                        },
                        new Container
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            AutoSizeAxes = Axes.X,
                            RelativeSizeAxes = Axes.Y,
                            Padding = new MarginPadding { Horizontal = 80, Vertical = toast_default_padding },
                            Child = new GridContainer
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                AutoSizeAxes = Axes.X,
                                RelativeSizeAxes = Axes.Y,
                                ColumnDimensions = new[]
                                {
                                    new Dimension(GridSizeMode.AutoSize),
                                    new Dimension(GridSizeMode.AutoSize)
                                },
                                RowDimensions = new[]
                                {
                                    new Dimension(GridSizeMode.AutoSize),
                                    new Dimension(GridSizeMode.AutoSize)
                                },
                                Content = new[]
                                {
                                    new Drawable[]
                                    {
                                        new SpriteText
                                        {
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Colour = Color4.Gray,
                                            Text = info.Description,
                                            Font = FontUsage.Default.With(family: "OpenSans-Bold", size: 25)
                                        }
                                    },
                                    new Drawable[]
                                    {
                                        new SpriteText
                                        {
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Colour = Color4.DarkGray,
                                            Text = info.SubDescription,
                                            Font = FontUsage.Default.With(size: 25)
                                        }
                                    }
                                }
                            }
                        }
                    }
                });

                toast.Delay(delay).MoveToY(0, 250, Easing.OutCubic).Then().Delay(1500).MoveToY(-100, 250, Easing.OutCubic).Then().Expire();
            });
            Scheduler.AddDelayed(() => ToastQueueCount--, 2000);
        }
    }
}
