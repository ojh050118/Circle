using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Circle.Game.Overlays.OSD
{
    public class Toast : Container
    {
        public int ToastQueueCount;

        [Resolved]
        private AudioManager audio { get; set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.X;
            Height = 100;
        }

        public void Push(ToastInfo info)
        {
            int delay = 2000 * ToastQueueCount;
            ToastQueueCount++;

            DrawableToast toast;
            Sample sample = audio.Samples.Get(info.Sample);
            Schedule(() =>
            {
                Add(toast = new DrawableToast(info) { Y = -100 });

                toast.Delay(delay).MoveToY(0, 250, Easing.OutCubic).Then().Delay(1500).MoveToY(-100, 250, Easing.OutCubic).Then().Expire();
                Scheduler.AddDelayed(() => sample?.Play(), delay);
            });
            Scheduler.AddDelayed(() => ToastQueueCount--, 1750);
        }
    }
}
