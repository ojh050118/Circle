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
    public class DialogOverlay : CircleFocusedOverlayContainer
    {
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

        /// <summary>
        /// 버튼들. 버튼이 없으면 버튼 없이 대화 상자가 생성됨.
        /// </summary>
        public IReadOnlyList<DialogButton> Buttons;

        public Action OnHide;

        private readonly SpriteText title;
        private readonly SpriteText description;
        private FillFlowContainer buttonContainer;
        private FillFlowContainer divisor;

        public DialogOverlay(BufferedContainer screenContainer)
            : base(screenContainer)
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
                    new FillFlowContainer
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
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
                        }
                    }
                }
            };
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            if (Buttons == null || Buttons.Count == 0)
                return;

            Content.Add(createButtonContainer());
        }

        public void Push()
        {
            if (buttonContainer == null)
                Content.Add(createButtonContainer());
            else
            {
                divisor.Children = createButtonDivisor(Buttons);
                buttonContainer.Children = computeWidth(Buttons);
            }

            Show();
        }

        private Container createButtonContainer()
        {
            return new Container
            {
                RelativeSizeAxes = Axes.X,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                AutoSizeAxes = Axes.Y,
                Padding = new MarginPadding { Top = 80 },
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
                    divisor = new FillFlowContainer
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Direction = FillDirection.Horizontal,
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Children = createButtonDivisor(Buttons)
                    },
                    buttonContainer = new FillFlowContainer
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Direction = FillDirection.Horizontal,
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Children = computeWidth(Buttons)
                    }
                }
            };
        }

        private Container[] createButtonDivisor(IReadOnlyList<DialogButton> buttons)
        {
            List<Container> boxContainer = new List<Container>();

            for (int i = 1; i < buttons.Count; i++)
            {
                boxContainer.Add(new Container
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 50,
                    Width = (float)buttons.Count / (buttons.Count * buttons.Count),
                    Child = new Box
                    {
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomCentre,
                        Alpha = 0.3f,
                        Colour = Color4.Black,
                        RelativeSizeAxes = Axes.Y,
                        Width = 2,
                        Height = 0.96f
                    }
                });
            }

            return boxContainer.ToArray();
        }

        private DialogButton[] computeWidth(IReadOnlyList<DialogButton> buttons)
        {
            var result = new List<DialogButton>();

            foreach (var button in buttons)
            {
                button.Width = (float)buttons.Count / (buttons.Count * buttons.Count);
                result.Add(button);
            }

            return result.ToArray();
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
