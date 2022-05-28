using System;
using Circle.Game.Beatmaps;
using Circle.Game.Configuration;
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

        [Resolved]
        private Background background { get; set; }

        private IconWithTextButton play;
        private IconWithTextButton edit;
        private IconWithTextButton settings;
        private IconWithTextButton exit;

        [BackgroundDependencyLoader]
        private void load(CircleConfigManager localConfig, BeatmapManager beatmapManager)
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
                        play = new IconWithTextButton("play", "button-play-select", Color4.LightGreen)
                        {
                            Icon = FontAwesome.Solid.Play,
                            Action = () => this.Push(new SongSelectScreen())
                        },
                        edit = new IconWithTextButton("edit", "button-edit-select", Color4.Yellow)
                        {
                            Icon = FontAwesome.Solid.Edit,
                        },
                        settings = new IconWithTextButton("settings", "button-settings-select", Color4.Gray)
                        {
                            Icon = FontAwesome.Solid.Cog,
                            Action = () => this.Push(new SettingsScreen())
                        },
                        exit = new IconWithTextButton("exit", colour: Color4.IndianRed)
                        {
                            Icon = FontAwesome.Solid.DoorOpen,
                            Action = OnExit
                        }
                    }
                }
            };

            if (localConfig.Get<bool>(CircleSetting.LoadBeatmapsOnStartup))
                beatmapManager.ReloadBeatmaps();
        }

        public override void OnEntering(ScreenTransitionEvent e)
        {
            base.OnEntering(e);

            background.ChangeTexture(TextureSource.Internal, "bg1", 1000, Easing.OutPow10);
        }

        public override void OnResuming(ScreenTransitionEvent e)
        {
            base.OnResuming(e);

            background.ChangeTexture(TextureSource.Internal, "bg1", 1000, Easing.OutPow10);
        }

        public override void OnExit()
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
            switch (e.Action)
            {
                case InputAction.Select:
                case InputAction.Play:
                    play.Action?.Invoke();
                    return true;

                case InputAction.Edit:
                    edit.Action?.Invoke();
                    return true;

                case InputAction.Settings:
                    settings.Action?.Invoke();
                    return true;

                case InputAction.Back:
                case InputAction.Exit:
                    exit.Action?.Invoke();
                    return true;
            }

            return base.OnPressed(e);
        }

        private class IconWithTextButton : Container
        {
            private readonly SpriteIcon icon;

            private Sample sampleSelect;
            private readonly string sampleName;
            private readonly Color4 colour;

            public IconUsage Icon
            {
                get => icon.Icon;
                set => icon.Icon = value;
            }

            public Action Action { get; set; }

            public IconWithTextButton(string text = @"", string sample = @"", Color4 colour = default)
            {
                sampleName = sample;
                this.colour = colour;
                AutoSizeAxes = Axes.X;
                Height = 40;
                Child = new FillFlowContainer
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Alpha = 0.7f,
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
                            Size = new Vector2(20),
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
                            Font = FontUsage.Default.With(size: 28)
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
                Action += () =>
                {
                    sampleSelect?.Play();
                    Child.FlashColour(colour, 750, Easing.Out);
                };
            }

            protected override bool OnHover(HoverEvent e)
            {
                Child.FadeTo(1).Then().FadeTo(0.85f, 250, Easing.Out);

                return base.OnHover(e);
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                base.OnHoverLost(e);

                Child.FadeTo(0.7f, 500, Easing.Out);
            }

            protected override bool OnClick(ClickEvent e)
            {
                Action?.Invoke();

                return base.OnClick(e);
            }
        }
    }
}
