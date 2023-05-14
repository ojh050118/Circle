#nullable disable

using System.Diagnostics.CodeAnalysis;
using Circle.Game.Beatmaps;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Audio;
using osu.Framework.Graphics.Containers;
using osu.Framework.Threading;

namespace Circle.Game.Overlays
{
    public partial class MusicController : CompositeDrawable
    {
        private ScheduledDelegate seekDelegate;

        [NotNull]
        public DrawableTrack CurrentTrack { get; protected set; } = new DrawableTrack(new TrackVirtual(1000));

        [Resolved]
        private BeatmapStorage beatmaps { get; set; }

        public bool IsPlaying => CurrentTrack.IsRunning;

        public bool TrackLoaded => CurrentTrack.TrackLoaded;

        public bool HasCompleted => CurrentTrack.HasCompleted;

        public void Play(bool restart = false)
        {
            if (restart)
                CurrentTrack.Restart();
            else if (!IsPlaying)
                CurrentTrack.Start();
        }

        public void Stop()
        {
            if (IsPlaying)
                CurrentTrack.Stop();
        }

        public void SeekTo(double position)
        {
            seekDelegate?.Cancel();
            seekDelegate = Schedule(() =>
            {
                CurrentTrack.Seek(position);
            });
        }

        public void ChangeTrack(BeatmapInfo info)
        {
            var queuedTrack = new DrawableTrack(beatmaps.GetTrack(info));
            var lastTrack = CurrentTrack;
            CurrentTrack = queuedTrack;

            Schedule(() =>
            {
                lastTrack.VolumeTo(0, 500, Easing.Out).Expire();

                if (queuedTrack == CurrentTrack)
                {
                    AddInternal(queuedTrack);
                    queuedTrack.VolumeTo(0).Then().VolumeTo(1, 300, Easing.Out);
                }
                else
                    queuedTrack.Dispose();
            });
        }

        public void RestartTrack() => Schedule(() => CurrentTrack.Restart());
    }
}
