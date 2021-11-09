using System;
using Circle.Game.Screens.Setting.Sections;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;
using osu.Framework.Extensions.Color4Extensions;
using Circle.Game.Graphics.UserInterface;

namespace Circle.Game.Screens.Setting
{
    public class SettingsScreen : CircleScreen
    {
        public override string Header => "settings";

        public SettingsScreen()
        {
            InternalChildren = new Drawable[]
            {
                new SpriteText
                {
                    Margin = new MarginPadding(60),
                    Text = Header,
                    Font = FontUsage.Default.With(size: 40)
                }.WithEffect(new GlowEffect
                {
                    BlurSigma = new Vector2(10),
                    PadExtent = true,
                }),
                new Container
                {
                    Margin = new MarginPadding { Top = 130, Left = 80 },
                    Size = new Vector2(500, Y),
                    RelativeSizeAxes = Axes.Y,
                    Masking = true,
                    CornerRadius = 7,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            Colour = Color4.White.Opacity(0.2f),
                            RelativeSizeAxes = Axes.Both
                        },
                        new CircleScrollContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            Child = new FillFlowContainer
                            {
                                Direction = FillDirection.Vertical,
                                Spacing = new Vector2(10),
                                AutoSizeAxes = Axes.Y,
                                RelativeSizeAxes = Axes.X,
                                Children = new Drawable[]
                                {
                                    new DebugSection()
                                }
                            }
                        }
                    }
                },
            };
        }
    }
}
