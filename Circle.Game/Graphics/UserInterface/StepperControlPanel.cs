using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;

namespace Circle.Game.Graphics.UserInterface
{
    public abstract partial class StepperControlPanel : Container
    {
        public readonly Bindable<bool> NextEnabled = new BindableBool(true);
        public readonly Bindable<bool> PreviousEnabled = new BindableBool(true);

        protected abstract Button PreviousButton { get; }
        protected abstract Button NextButton { get; }

        [Resolved]
        private IStepperControl stepper { get; set; } = null!;

        protected StepperControlPanel()
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            PreviousButton.Action = () => stepper.MovePrevious();
            NextButton.Action = () => stepper.MoveNext();

            PreviousEnabled.BindTo(PreviousButton.Enabled);
            NextEnabled.BindTo(NextButton.Enabled);
        }
    }
}
