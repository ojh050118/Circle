using Circle.Game.Graphics.Containers;
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
        public override string Header => "Settings";

        public SettingsScreen()
        {
            AddRangeInternal(new Drawable[]
            {
                new ScreenHeader(this),
                new Container
                {
                    Margin = new MarginPadding { Left = 80 },
                    Padding = new MarginPadding { Top = 130, Bottom = 65 },
                    RelativeSizeAxes = Axes.Both,
                    Width = 0.5f,
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
                                        new AudioSection(),
                                        new GraphicsSection(),
                                        new DebugSection()
                                    }
                                }
                            }
                        }
                    }
                },
            });
        }
    }
}
