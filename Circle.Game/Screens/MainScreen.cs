using System;
using Circle.Game.Graphics.UserInterface;
using Circle.Game.Input;
using Circle.Game.Overlays;
using Circle.Game.Screens.Select;
using Circle.Game.Screens.Setting;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Screens
{
    public class MainScreen : CircleScreen
    {
        public override string Header => "Circle";

        public override bool BlockExit => true;

        public override bool FadeBackground => false;

        [Resolved]
        private DialogOverlay dialog { get; set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren = new Drawable[]
            {
                new SpriteText
                {
                    Text = Header,
                    Margin = new MarginPadding { Top = 60, Left = 60 },
                    Font = FontUsage.Default.With(size: 40)
                }.WithEffect(new GlowEffect
                {
                    Colour = Color4.White,
                    BlurSigma = new Vector2(4f),
                    PadExtent = true
                }),
                new FillFlowContainer
                {
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(10),
                    AutoSizeAxes = Axes.Both,
                    Margin = new MarginPadding { Top = 120, Left = 100 },
                    Children = new Drawable[]
                    {
                        new IconWithTextButton("play", "button-play-select")
                        {
                            Icon = FontAwesome.Solid.Play,
                            Action = () => this.Push(new SongSelectScreen())
                        },
                        new IconWithTextButton("edit", "button-edit-select")
                        {
                            Icon = FontAwesome.Solid.Edit,
                        },
                        new IconWithTextButton("settings", "button-settings-select")
                        {
                            Icon = FontAwesome.Solid.Cog,
                            Action = () => this.Push(new SettingsScreen())
                        },
                        new IconWithTextButton("exit")
                        {
                            Icon = FontAwesome.Solid.DoorOpen,
                            Action = onExit
                        }
                    }
                }
            };
        }

        private void onExit()
        {
            dialog.Title = "Exit";
            dialog.Description = "Are you sure exit game?";

            dialog.Buttons = new[]
            {
                new DialogButton
                {
                    Text = "Cancel",
                    Action = dialog.Hide
                },
                new DialogButton
                {
                    Text = "OK",
                    Font = FontUsage.Default.With(family: "OpenSans-Bold", size: 28),
                    Action = Game.Exit
                }
            };
            dialog.Push();
        }

        public override bool OnPressed(KeyBindingPressEvent<InputAction> e)
        {
            if (e.Action == InputAction.Back)
            {
                onExit();
                return true;
            }

            return base.OnPressed(e);
        }

        private class IconWithTextButton : CircularContainer
        {
            private readonly SpriteIcon icon;

            private Sample sampleSelect;
            private readonly string sampleName;

            public IconUsage Icon
            {
                get => icon.Icon;
                set => icon.Icon = value;
            }

            public Action Action { get; set; }

            public IconWithTextButton(string text = @"", string sample = @"")
            {
                sampleName = sample;
                Masking = true;
                AutoSizeAxes = Axes.X;
                Height = 40;
                Child = new FillFlowContainer
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Alpha = 0.6f,
                    RelativeSizeAxes = Axes.Y,
                    AutoSizeAxes = Axes.X,
                    Direction = FillDirection.Horizontal,
                    Spacing = new Vector2(10),
                    Children = new Drawable[]
                    {
                        icon = new SpriteIcon
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Margin = new MarginPadding { Left = 10 },
                            Size = new Vector2(17),
                        },
                        new Box
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Colour = Color4.White,
                            Alpha = 0.5f,
                            Width = 3,
                            RelativeSizeAxes = Axes.Y
                        },
                        new SpriteText
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Text = text,
                            Font = FontUsage.Default.With(size: 26)
                        }.WithEffect(new GlowEffect
                        {
                            BlurSigma = new Vector2(5),
                            Colour = Color4.White,
                            PadExtent = true
                        })
                    }
                };
            }

            [BackgroundDependencyLoader]
            private void load(AudioManager audio)
            {
                sampleSelect = audio.Samples.Get(sampleName);
            }

            protected override bool OnHover(HoverEvent e)
            {
                Child.FadeTo(1).Then().FadeTo(0.8f, 250, Easing.In);

                return base.OnHover(e);
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                base.OnHoverLost(e);

                Child.FadeTo(0.6f, 1000, Easing.OutPow10);
            }

            protected override bool OnClick(ClickEvent e)
            {
                Child.FlashColour(Color4.DarkGray, 250, Easing.InQuad);
                Action?.Invoke();
                sampleSelect?.Play();

                return base.OnClick(e);
            }
        }
    }
}
