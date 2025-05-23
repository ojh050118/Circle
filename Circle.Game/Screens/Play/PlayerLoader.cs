#nullable disable

using Circle.Game.Beatmaps;
using Circle.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Screens;
using osu.Framework.Threading;

namespace Circle.Game.Screens.Play
{
    public partial class PlayerLoader : CircleScreen
    {
        private ScreenHeader header;

        private CircleScreen player;

        private LoadingSpinner spinner;
        private ScheduledDelegate spinnerShow;

        private readonly BeatmapInfo beatmapInfo;
        private WorkingBeatmap workingBeatmap;

        public PlayerLoader(BeatmapInfo beatmapInfo)
        {
            this.beatmapInfo = beatmapInfo;
            InternalChildren = new Drawable[]
            {
                header = new ScreenHeader(this)
            };
        }

        public override bool FadeBackground => false;

        public override bool PlaySample => false;

        [BackgroundDependencyLoader]
        private void load(BeatmapManager beatmapManager)
        {
            workingBeatmap = beatmapManager.GetWorkingBeatmap(beatmapInfo);

            InternalChild = header = new ScreenHeader(this)
            {
                Text = workingBeatmap.Beatmap.ToString(),
                Alpha = 0
            };
        }

        public override void OnEntering(ScreenTransitionEvent e)
        {
            base.OnEntering(e);

            if (workingBeatmap.Beatmap.AngleData == null || workingBeatmap.Beatmap.AngleData.Length == 0)
            {
                OnExit();
                return;
            }

            LoadComponentAsync(player = new Player(workingBeatmap));
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

        public override bool OnExiting(ScreenExitEvent e)
        {
            Scheduler.CancelDelayedTasks();

            return base.OnExiting(e);
        }

        public override void OnResuming(ScreenTransitionEvent e)
        {
            base.OnResuming(e);
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
                Scheduler.Add(() => this.Push(player));
        }
    }
}
