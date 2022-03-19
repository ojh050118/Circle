using Circle.Game.Overlays;

namespace Circle.Game.Tests.Visual.Overlays
{
    public class TestSceneImportOverlay : CircleTestScene
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
