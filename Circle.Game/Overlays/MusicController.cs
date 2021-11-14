using System.Diagnostics.CodeAnalysis;
using Circle.Game.Beatmap;
using Circle.Game.IO;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Audio;
using osu.Framework.Graphics.Containers;
using osu.Framework.Threading;

namespace Circle.Game.Overlays
{
    public class MusicController : CompositeDrawable
    {
        [NotNull]
        public DrawableTrack CurrentTrack { get; private set; } = new DrawableTrack(new TrackVirtual(1000));

        [Resolved]
        private ExternalAudioManager audio { get; set; }

        [Resolved]
        private Bindable<BeatmapInfo> workingBeatmap { get; set; }

        [Resolved]
        private FrameworkConfigManager config { get; set; }

        public bool IsPlaying => CurrentTrack.IsRunning;

        public bool TrackLoaded => CurrentTrack.TrackLoaded;

        public bool HasCompleted => CurrentTrack.HasCompleted;

        private ScheduledDelegate seekDelegate;

        [BackgroundDependencyLoader]
        private void load(FrameworkConfigManager config)
        {
            workingBeatmap.ValueChanged += info => changeTrack(info.NewValue);
        }

        protected override void Update()
        {
            base.Update();

            // 성능에 큰 영향을 미칩니다.
            // 좋은 방법을 찾기 전까지 이걸 사용합니다.
            audio.Volume.Value = config.Get<double>(FrameworkSetting.VolumeMusic) * config.Get<double>(FrameworkSetting.VolumeUniversal);
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

            var queuedTrack = new DrawableTrack(audio.Tracks.Get(info.Settings.Track));

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
                {
                    queuedTrack.Dispose();
                }
            });
        }

        public void RestartTrack() => Schedule(() => CurrentTrack.Restart());
    }
}
