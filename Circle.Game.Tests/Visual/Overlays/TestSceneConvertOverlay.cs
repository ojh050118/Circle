#nullable disable

using Circle.Game.Overlays;

namespace Circle.Game.Tests.Visual.Overlays
{
    public partial class TestSceneConvertOverlay : CircleTestScene
    {
        public TestSceneConvertOverlay()
        {
            ConvertOverlay convert;
            Add(convert = new ConvertOverlay());
            AddStep("show", convert.Show);
            AddStep("hide", convert.Hide);
        }
    }
}
