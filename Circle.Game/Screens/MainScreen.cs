using System;
using Circle.Game.Screens.Setting;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
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

        [BackgroundDependencyLoader]
        private void load(LargeTextureStore textures)
        {
            InternalChildren = new Drawable[]
            {
                new Sprite
                {
                    RelativeSizeAxes = Axes.Both,
                    Texture = textures.Get("Duelyst"),
                    FillMode = FillMode.Fill
                },
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
                        new IconWithTextButton("play")
                        {
                            Icon = FontAwesome.Solid.Play
                        },
                        new IconWithTextButton("settings")
                        {
                            Icon = FontAwesome.Solid.Cog,
                            Action = () => this.Push(new SettingsScreen())
                        },
                        new IconWithTextButton("exit")
                        {
                            Icon = FontAwesome.Solid.DoorOpen
                        }
                    }
                }
            };
        }

        private class IconWithTextButton : CircularContainer
        {
            private readonly SpriteIcon icon;

            public IconUsage Icon
            {
                get => icon.Icon;
                set => icon.Icon = value;
            }

            public Action Action { get; set; }

            public IconWithTextButton(string text = @"")
            {
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
                            Font = FontUsage.Default.With(size: 24)
                        }.WithEffect(new GlowEffect
                        {
                            BlurSigma = new Vector2(5),
                            Colour = Color4.White,
                            PadExtent = true
                        })
                    }
                };
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

                return base.OnClick(e);
            }
        }
    }
}
