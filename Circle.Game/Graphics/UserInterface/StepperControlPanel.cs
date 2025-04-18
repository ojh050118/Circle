using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;

namespace Circle.Game.Graphics.UserInterface
{
    public abstract partial class StepperControlPanel : Container
    {
        public readonly Bindable<bool> NextEnabled = new BindableBool(true);
        public readonly Bindable<bool> PreviousEnabled = new BindableBool(true);

        protected abstract ClickableContainer PreviousButton { get; }
        protected abstract ClickableContainer NextButton { get; }

        [Resolved]
        private IStepperControl stepper { get; set; } = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            PreviousButton.Action = () => stepper.MovePrevious();
            NextButton.Action = () => stepper.MoveNext();

            PreviousEnabled.BindTo(PreviousButton.Enabled);
            NextEnabled.BindTo(NextButton.Enabled);
        }

        public virtual void OnValueChanged<T>(ValueChangedEvent<T> e)
        {
        }
    }
}
