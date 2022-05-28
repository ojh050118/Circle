using System;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Overlays.OSD
{
    public class DrawableToast : CircularContainer
    {
        private readonly ToastInfo toastInfo;

        private const int toast_minimum_width = 300;
        private const int toast_default_padding = 15;
        private const int toast_height = 70;

        public event Action<bool> CloseRequested;

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
                                new SpriteText
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Colour = Color4.Gray,
                                    Text = toastInfo.Description,
                                    Font = FontUsage.Default.With(family: "OpenSans-Bold", size: (toast_height - toast_default_padding * 2) / 2)
                                }
                            },
                            new Drawable[]
                            {
                                new SpriteText
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Colour = Color4.DarkGray,
                                    Text = toastInfo.SubDescription,
                                    Font = FontUsage.Default.With(size: (toast_height - toast_default_padding * 2) / 2)
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

        protected override bool OnDragStart(DragStartEvent e)
        {
            return true;
        }

        protected override void OnDrag(DragEvent e)
        {
            base.OnDrag(e);

            this.MoveToOffset(new Vector2(0, e.Delta.Y));
        }

        protected override void OnDragEnd(DragEndEvent e)
        {
            base.OnDragEnd(e);

            if (Y > 0)
                this.MoveTo(Vector2.Zero, Y * 2, Easing.OutElastic);
            else if (Y < 0)
            {
                CloseRequested?.Invoke(true);
                this.MoveToY(-100, 250, Easing.OutCubic).Expire();
            }

        }
    }
}
