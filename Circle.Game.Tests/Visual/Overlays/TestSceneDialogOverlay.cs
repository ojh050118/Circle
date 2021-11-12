using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Circle.Game.Graphics.UserInterface;
using Circle.Game.Overlays;
using osu.Framework.Bindables;

namespace Circle.Game.Tests.Visual.Overlays
{
    public class TestSceneDialogOverlay : CircleTestScene
    {
        private DialogOverlay dialog;

        public TestSceneDialogOverlay()
        {
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
