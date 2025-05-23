#nullable disable

using System;
using Circle.Game.Graphics;
using Circle.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Threading;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Overlays.OSD
{
    public partial class DrawableToast : CircularContainer
    {
        private const int toast_minimum_width = 300;
        private const int toast_default_padding = 15;
        private const int toast_height = 70;

        private const double transition_duration = 250;
        private const double show_duration = 1500;

        private readonly ToastInfo toastInfo;

        private ScheduledDelegate hideSchedule;

        public DrawableToast(ToastInfo toastInfo)
        {
            this.toastInfo = toastInfo;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Anchor = Anchor.TopCentre;
            Origin = Anchor.TopCentre;
            Masking = true;
            AutoSizeAxes = Axes.X;
            Height = toast_height;
            Margin = new MarginPadding { Top = toast_default_padding };
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
                    Size = new Vector2(toast_height),
                    Padding = new MarginPadding(toast_default_padding + 5),
                    Child = new SpriteIcon
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                        Colour = toastInfo.IconColour,
                        Icon = toastInfo.Icon
                    }
                },
                new Container
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    AutoSizeAxes = Axes.X,
                    RelativeSizeAxes = Axes.Y,
                    Padding = new MarginPadding { Horizontal = toast_height, Vertical = toast_default_padding },
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
                                new CircleSpriteText
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Colour = Color4.Gray,
                                    Text = toastInfo.Description,
                                    Shadow = false,
                                    Font = CircleFont.Default.With(weight: FontWeight.Bold, size: (toast_height - toast_default_padding * 2) / 2)
                                }
                            },
                            new Drawable[]
                            {
                                new CircleSpriteText
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Colour = Color4.DarkGray,
                                    Text = toastInfo.SubDescription,
                                    Shadow = false,
                                    Font = CircleFont.Default.With(size: (toast_height - toast_default_padding * 2) / 2)
                                }
                            }
                        }
                    }
                }
            };
            EdgeEffect = new EdgeEffectParameters
            {
                Type = EdgeEffectType.Shadow,
                Colour = Color4.Black.Opacity(0.2f),
                Radius = 10
            };
        }

        public void ShowAndHide()
        {
            this.MoveToY(0, transition_duration, Easing.OutCubic)
                .Then()
                .Delay(show_duration)
                .Schedule(() => this.MoveToY(-100, transition_duration, Easing.OutCubic).Expire(), out hideSchedule);
        }

        protected override bool OnDragStart(DragStartEvent e)
        {
            ClearTransforms();
            hideSchedule?.Cancel();
            return true;
        }

        protected override void OnDrag(DragEvent e)
        {
            ClearTransforms();
            Vector2 change = e.MousePosition - e.MouseDownPosition;
            change *= change.Length <= 0 ? 0 : MathF.Pow(change.Length, 0.7f) / change.Length;

            this.MoveTo(change);
        }

        protected override void OnDragEnd(DragEndEvent e)
        {
            base.OnDragEnd(e);

            if (Y > 0)
            {
                this.MoveTo(Vector2.Zero, 800, Easing.OutElastic);
            }
            else if (Y < 0)
            {
                this.MoveToY(-100, 250, Easing.OutCubic).Expire();

                return;
            }

            this.Delay(1500).Schedule(() => this.MoveToY(-100, 250, Easing.OutCubic).Expire(), out hideSchedule);
        }
    }
}
