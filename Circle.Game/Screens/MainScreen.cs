using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Screens;
using osuTK.Graphics;

namespace Circle.Game.Screens
{
    public class MainScreen : CircleScreen
    {
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
                    Text = "Circle",
                    Margin = new MarginPadding(60),
                    Font = FontUsage.Default.With(size: 40)
                }
            };
        }
    }
}
