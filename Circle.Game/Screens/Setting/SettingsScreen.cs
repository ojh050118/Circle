using Circle.Game.Graphics.Containers;
using Circle.Game.Graphics.UserInterface;
using Circle.Game.Screens.Setting.Sections;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Screens;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Screens.Setting
{
    public class SettingsScreen : CircleScreen
    {
        public override string Header => "Settings";

        [Resolved]
        private Background background { get; set; }

        private string oldTexture;
        private TextureSource oldTextureSource;

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
                    Width = 0.6f,
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
                                        new MaintenanceSection(),
                                        new DebugSection()
                                    }
                                }
                            }
                        }
                    }
                },
            });
        }

        public override void OnEntering(IScreen last)
        {
            base.OnEntering(last);

            oldTexture = background.TextureName;
            oldTextureSource = background.TextureSource;
            background.ChangeTexture(TextureSource.Internal, "bg2", 1000, Easing.OutPow10);
        }

        public override bool OnExiting(IScreen next)
        {
            background.ChangeTexture(oldTextureSource, oldTexture, 1000, Easing.OutPow10);

            return base.OnExiting(next);
        }
    }
}
