#nullable disable

using Circle.Game.Graphics.UserInterface;
using Circle.Game.Overlays;

namespace Circle.Game.Tests.Visual.Overlays
{
    public partial class TestSceneDialogOverlay : CircleTestScene
    {
        public TestSceneDialogOverlay()
        {
            DialogOverlay dialog;
            Add(dialog = new DialogOverlay
            {
                Title = "Title",
                Description = "Description",
                Buttons = new[]
                {
                    new DialogButton
                    {
                        Text = "Hide",
                        Action = Hide
                    },
                    new DialogButton
                    {
                        Text = "Dialog Button",
                    },
                    new DialogButton
                    {
                        Text = "Show",
                        Action = Show
                    },
                }
            });
            AddStep("show", dialog.Show);
            AddStep("hide", dialog.Hide);
        }
    }
}
