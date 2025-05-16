#nullable disable

using System.Collections.Generic;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Circle.Game.Overlays.OSD
{
    public partial class Toast : Container
    {
        [Resolved]
        private AudioManager audio { get; set; }

        private readonly Queue<ToastInfo> toastQueue = new Queue<ToastInfo>();

        private int pendingToasts => toastQueue.Count;

        private DrawableToast currentToast;

        private const double transition_duration = 250;
        private const double show_duration = 1500;

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
        }

        public void Push(ToastInfo info)
        {
            toastQueue.Enqueue(info);

            if (toastQueue.Count <= 1)
                push();
        }

        private void push()
        {
            if (toastQueue.Count == 0)
                return;

            if (currentToast != null && currentToast.IsAlive)
            {
                Task.Run(push);
                return;
            }

            var info = toastQueue.Dequeue();
            var sample = audio.Samples.Get(info.Sample);

            Schedule(() =>
            {
                Add(currentToast = new DrawableToast(info) { Y = -100 });

                sample?.Play();
                currentToast.ShowAndHide();

                Task.Run(push);
            });
        }
    }
}
