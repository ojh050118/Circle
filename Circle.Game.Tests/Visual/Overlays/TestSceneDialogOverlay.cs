using Circle.Game.Graphics.UserInterface;
using Circle.Game.Overlays;
using osu.Framework.Graphics.Containers;

namespace Circle.Game.Tests.Visual.Overlays
{
    public class TestSceneDialogOverlay : CircleTestScene
    {
        public TestSceneDialogOverlay()
        {
            DialogOverlay dialog;
            Add(dialog = new DialogOverlay(new BufferedContainer())
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
