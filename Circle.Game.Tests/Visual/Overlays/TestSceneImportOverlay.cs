using Circle.Game.Overlays;
using osu.Framework.Graphics.Containers;

namespace Circle.Game.Tests.Visual.Overlays
{
    public class TestSceneImportOverlay : CircleTestScene
    {
        public TestSceneImportOverlay()
        {
            ImportOverlay import;
            Add(import = new ImportOverlay(new BufferedContainer()));
            AddStep("show", import.Show);
            AddStep("hide", import.Hide);
        }
    }
}
