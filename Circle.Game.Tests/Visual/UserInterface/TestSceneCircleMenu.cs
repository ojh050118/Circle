using Circle.Game.Graphics.UserInterface;
using osu.Framework.Graphics;
using osuTK;
using osuTK.Graphics;

namespace Circle.Game.Tests.Visual.UserInterface
{
    public class TestSceneCircleMenu : CircleTestScene
    {
        public TestSceneCircleMenu()
        {
            Add(new CircleMenu(Direction.Vertical, true)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Items = new[]
                {
                    new CircleMenuItem("standard", MenuItemType.Standard),
                    new CircleMenuItem("highlighted", MenuItemType.Highlighted),
                    new CircleMenuItem("destructive", MenuItemType.Destructive),
                }
            });
        }
    }
}
