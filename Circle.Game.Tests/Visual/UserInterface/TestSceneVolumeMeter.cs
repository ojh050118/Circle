using Circle.Game.Overlays.Volume;
using osu.Framework.Graphics;
using osuTK.Graphics;

namespace Circle.Game.Tests.Visual.UserInterface
{
    public class TestSceneVolumeMeter : CircleTestScene
    {
        public TestSceneVolumeMeter()
        {
            VolumeMeter meter;
            Add(meter = new VolumeMeter("very very very long text", 200, Color4.DeepSkyBlue)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            });
            AddSliderStep("Current", 0, 1, 0.3, volume => meter.Current.Value = volume);
        }
    }
}
