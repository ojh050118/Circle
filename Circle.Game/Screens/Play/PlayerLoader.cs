using Circle.Game.Beatmaps;
using Circle.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Screens;
using osu.Framework.Threading;

namespace Circle.Game.Screens.Play
{
    public class PlayerLoader : CircleScreen
    {
        public override bool FadeBackground => false;

        public override bool PlaySample => false;

        private CircleScreen player;

        private LoadingSpinner spinner;
        private ScheduledDelegate spinnerShow;
        private ScreenHeader header;

        public PlayerLoader()
        {
            InternalChildren = new Drawable[]
            {
                header = new ScreenHeader(this)
            };
        }

        [BackgroundDependencyLoader]
        private void load(BeatmapManager beatmap)
        {
            InternalChild = header = new ScreenHeader(this)
            {
                Text = beatmap.CurrentBeatmap.ToString(),
                Alpha = 0
            };
        }

        public override void OnEntering(IScreen last)
        {
            base.OnEntering(last);

            LoadComponentAsync(player = new Player());

            LoadComponentAsync(spinner = new LoadingSpinner(true, true)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            }, _ =>
            {
                AddInternal(spinner);
                spinnerShow = Scheduler.AddDelayed(() =>
                {
                    spinner.Show();
                    header.FadeIn(500, Easing.Out);
                }, 100);
            });

            checkIsLoaded();
        }

        public override bool OnExiting(IScreen next)
        {
            Scheduler.CancelDelayedTasks();

            return base.OnExiting(next);
        }

        public override void OnResuming(IScreen last)
        {
            base.OnResuming(last);
            OnExit();
        }

        private void checkIsLoaded()
        {
            if (player.LoadState != LoadState.Ready)
            {
                Schedule(checkIsLoaded);
                return;
            }

            spinnerShow?.Cancel();

            if (spinner.State.Value == Visibility.Visible)
            {
                spinner.Hide();
                header.FadeOut(500, Easing.Out);
                Scheduler.AddDelayed(() => this.Push(player), LoadingSpinner.TRANSITION_DURATION);
            }
            else
                this.Push(player);
        }
    }
}
