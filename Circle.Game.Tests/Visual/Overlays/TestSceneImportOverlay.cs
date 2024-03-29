#nullable disable

using Circle.Game.Overlays;

namespace Circle.Game.Tests.Visual.Overlays
{
    public partial class TestSceneImportOverlay : CircleTestScene
    {
        public TestSceneImportOverlay()
        {
            ImportOverlay import;
            Add(import = new ImportOverlay());
            AddStep("show", import.Show);
            AddStep("hide", import.Hide);
        }
    }
}
