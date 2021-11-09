using Circle.Game.Graphics.UserInterface;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Screens;
using osu.Framework.Threading;

namespace Circle.Game.Screens
{
    public class Loader : CircleScreen
    {
        private Screen mainScreen;

        private LoadingSpinner spinner;
        private ScheduledDelegate spinnerShow;
        public Bindable<bool> ComponentLoaded { get; } = new Bindable<bool>(false);

        public override void OnEntering(IScreen last)
        {
            base.OnEntering(last);

            LoadComponentAsync(mainScreen = new MainScreen());

            LoadComponentAsync(spinner = new LoadingSpinner(true, true)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            }, _ =>
            {
                AddInternal(spinner);
                spinnerShow = Scheduler.AddDelayed(spinner.Show, 200);
            });

            checkIsLoaded();
        }

        private void checkIsLoaded()
        {
            if (mainScreen.LoadState != LoadState.Ready)
            {
                Schedule(checkIsLoaded);
                return;
            }
            else
                ComponentLoaded.Value = true;

            spinnerShow?.Cancel();

            if (spinner.State.Value == Visibility.Visible)
            {
                spinner.Hide();
                Scheduler.AddDelayed(() => this.Push(mainScreen), LoadingSpinner.TRANSITION_DURATION);
            }
            else
                this.Push(mainScreen);
        }
    }
}
