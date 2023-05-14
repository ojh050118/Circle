#nullable disable

using Circle.Game.Overlays.OSD;
using osu.Framework.Graphics.Sprites;
using osuTK.Graphics;

namespace Circle.Game.Tests.Visual.Overlays
{
    public partial class TestSceneOnScreenDisplay : CircleTestScene
    {
        public TestSceneOnScreenDisplay()
        {
            Toast toast;
            Add(toast = new Toast());
            AddStep("Push empty osd toast", () => toast.Push(new ToastInfo()));
            AddStep("Push icon only empty toast", () => toast.Push(new ToastInfo { Icon = FontAwesome.Solid.Bell }));
            AddStep("Push long text only osd toast", () => toast.Push(new ToastInfo
            {
                Description = "A very very very very very very long text",
                SubDescription = "A very very very very very very long text"
            }));
            AddStep("Push long text with icon osd toast", () => toast.Push(new ToastInfo
            {
                Description = "A very very very very very very long text",
                SubDescription = "A very very very very very very long text",
                Icon = FontAwesome.Solid.Check
            }));
            AddStep("Push colored icon osd toast", () => toast.Push(new ToastInfo
            {
                Description = "Importing...",
                SubDescription = "[Developer] Circle - TestBeatmap.circlez",
                Icon = FontAwesome.Solid.FileDownload,
                IconColour = Color4.DeepSkyBlue
            }));
        }
    }
}
