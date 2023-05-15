#nullable disable

using Circle.Game.Graphics.UserInterface;
using osu.Framework.Graphics;

namespace Circle.Game.Tests.Visual.UserInterface
{
    public partial class TestSceneCircleDropdown : CircleTestScene
    {
        public TestSceneCircleDropdown()
        {
            Add(new CircleEnumDropdown<TestEnum>
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Width = 400
            });
        }

        private enum TestEnum
        {
            Enum1,
            Enum2,
            Enum3,
            Enum4,
            Enum5
        }
    }
}
