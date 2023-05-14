#nullable disable

using Circle.Game.Overlays;
using Circle.Game.Overlays.Volume;
using osu.Framework.Graphics;

namespace Circle.Game.Tests.Visual.Overlays
{
    public class TestSceneVolumeOverlay : CircleTestScene
    {
        public TestSceneVolumeOverlay()
        {
            VolumeOverlay volumeOverlay;
            Add(volumeOverlay = new VolumeOverlay());
            Add(new VolumeControlReceptor
            {
                RelativeSizeAxes = Axes.Both,
                ActionRequested = action => volumeOverlay.Adjust(action),
                ScrollActionRequested = (action, amount, _) => volumeOverlay.Adjust(action, amount),
            });
            AddStep("Show", volumeOverlay.Show);
            AddStep("Hide", volumeOverlay.Hide);
        }
    }
}
