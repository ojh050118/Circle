#nullable disable

using System;
using System.Collections.Generic;
using Circle.Game.Graphics.Containers;
using Circle.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Overlays
{
    public partial class DialogOverlay : CircleFocusedOverlayContainer
    {
        private readonly Container buttonContent;
        private readonly SpriteText description;

        private readonly SpriteText title;

        /// <summary>
        /// 버튼들. 버튼이 없으면 버튼 없이 대화 상자가 생성됨.
        /// </summary>
        public IReadOnlyList<DialogButton> Buttons;

        public Action OnHide;

        public DialogOverlay()
        {
            Content = new Container
            {
                Masking = true,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.X,
                Width = 0.3f,
                AutoSizeAxes = Axes.Y,
                CornerRadius = 10,
                Scale = new Vector2(1.2f),
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.White,
                        Alpha = 0.8f
                    },
                    new GridContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        RowDimensions = new[]
                        {
                            new Dimension(GridSizeMode.AutoSize),
                            new Dimension(GridSizeMode.AutoSize)
                        },
                        Content = new[]
                        {
                            new Drawable[]
                            {
                                new FillFlowContainer
                                {
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Direction = FillDirection.Vertical,
                                    Children = new Drawable[]
                                    {
                                        title = new SpriteText
                                        {
                                            Anchor = Anchor.TopCentre,
                                            Origin = Anchor.TopCentre,
                                            Font = FontUsage.Default.With(family: "OpenSans-Bold", size: 28),
                                            Colour = Color4.Black,
                                            Margin = new MarginPadding { Vertical = 10 },
                                        },
                                        description = new SpriteText
                                        {
                                            Anchor = Anchor.TopCentre,
                                            Origin = Anchor.TopCentre,
                                            Font = FontUsage.Default.With(size: 22),
                                            Colour = Color4.Black,
                                            Margin = new MarginPadding { Bottom = 10 },
                                        }
                                    },
                                }
                            },
                            new Drawable[]
                            {
                                buttonContent = new Container
                                {
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                }
                            }
                        }
                    }
                }
            };
        }

        public string Title
        {
            get => title.Text.ToString();
            set => title.Text = value;
        }

        public string Description
        {
            get => description.Text.ToString();
            set => description.Text = value;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            refreshButtons();
        }

        public void Push()
        {
            refreshButtons();

            Show();
        }

        private void refreshButtons()
        {
            buttonContent.Clear();

            if (Buttons != null && Buttons.Count != 0)
                buttonContent.Add(createButtonContainer());
        }

        private FillFlowContainer createButtonContainer()
        {
            return new FillFlowContainer
            {
                Direction = FillDirection.Vertical,
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Children = new Drawable[]
                {
                    new Box
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Alpha = 0.3f,
                        Colour = Color4.Black,
                        RelativeSizeAxes = Axes.X,
                        Height = 2,
                    },
                    new GridContainer
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        RowDimensions = new[]
                        {
                            new Dimension(GridSizeMode.AutoSize)
                        },
                        ColumnDimensions = getDimension(createButtonGrid(Buttons)),
                        Content = new[]
                        {
                            createButtonGrid(Buttons)
                        }
                    },
                }
            };
        }

        private Drawable[] createButtonGrid(IReadOnlyList<DialogButton> buttons)
        {
            List<Drawable> buttonGrid = new List<Drawable>();

            for (int i = 0; i < buttons.Count; i++)
            {
                buttonGrid.Add(buttons[i]);

                if (i < buttons.Count - 1)
                {
                    buttonGrid.Add(new Box
                    {
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        Alpha = 0.3f,
                        Colour = Color4.Black,
                        RelativeSizeAxes = Axes.Y,
                        Width = 2,
                    });
                }
            }

            return buttonGrid.ToArray();
        }

        private Dimension[] getDimension(Drawable[] content)
        {
            List<Dimension> dimensions = new List<Dimension>();

            foreach (var d in content)
            {
                switch (d)
                {
                    case Box _:
                        dimensions.Add(new Dimension(GridSizeMode.AutoSize));
                        break;

                    case DialogButton _:
                        dimensions.Add(new Dimension(GridSizeMode.Distributed));
                        break;
                }
            }

            return dimensions.ToArray();
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
            OnHide?.Invoke();
        }
    }
}
