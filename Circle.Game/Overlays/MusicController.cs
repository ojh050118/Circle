using System.Diagnostics.CodeAnalysis;
using Circle.Game.Beatmap;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Audio;
using osu.Framework.Graphics.Containers;
using osu.Framework.Threading;

namespace Circle.Game.Overlays
{
    public class MusicController : CompositeDrawable
    {
        [NotNull]
        public DrawableTrack CurrentTrack { get; set; } = new DrawableTrack(new TrackVirtual(1000));

        [Resolved]
        private Bindable<BeatmapInfo> workingBeatmap { get; set; }

        [Resolved]
        private BeatmapResourcesManager beatmapResources { get; set; }

        public bool IsPlaying => CurrentTrack.IsRunning;

        public bool TrackLoaded => CurrentTrack.TrackLoaded;

        public bool HasCompleted => CurrentTrack.HasCompleted;

        private ScheduledDelegate seekDelegate;

        [BackgroundDependencyLoader]
        private void load()
        {
            workingBeatmap.ValueChanged += info => changeTrack(info.NewValue);
        }

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

        private void changeTrack(BeatmapInfo info)
        {
            if (info.Settings.Track == string.Empty)
                return;

            var lastTrack = CurrentTrack;

            var queuedTrack = new DrawableTrack(beatmapResources.GetBeatmapTrack(info));

            CurrentTrack = queuedTrack;

            Schedule(() =>
            {
                lastTrack.VolumeTo(0, 500, Easing.Out).Expire();

                if (queuedTrack == CurrentTrack)
                {
                    AddInternal(queuedTrack);
                    queuedTrack.Seek(info.Settings.PreviewTrackStart * 1000);
                    queuedTrack.VolumeTo(0).Then().VolumeTo(1, 300, Easing.Out);
                }
                else
                    queuedTrack.Dispose();
            });
        }

        public void RestartTrack() => Schedule(() => CurrentTrack.Restart());
    }
}
