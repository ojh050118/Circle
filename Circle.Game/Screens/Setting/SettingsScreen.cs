using Circle.Game.Graphics.UserInterface;
using Circle.Game.Screens.Setting.Sections;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Screens.Setting
{
    public class SettingsScreen : CircleScreen
    {
        public override string Header => "settings";

        public SettingsScreen()
        {
            InternalChildren = new Drawable[]
            {
                new ScreenHeader(this),
                new Container
                {
                    Margin = new MarginPadding { Left = 80 },
                    Padding = new MarginPadding { Vertical = 130 },
                    Width = 600,
                    RelativeSizeAxes = Axes.Y,
                    Child = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Masking = true,
                        CornerRadius = 5,
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
                                        new GraphicsSection(),
                                        new DebugSection()
                                    }
                                }
                            }
                        }
                    }
                },
            };
        }
    }
}
