using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Circle.Game.Configuration;
using Circle.Game.IO;
using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Audio;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Threading;

namespace Circle.Game.Overlays
{
    public class MusicController : CompositeDrawable
    {
        private readonly Storage storage;

        [NotNull]
        public DrawableTrack CurrentTrack { get; private set; } = new DrawableTrack(new TrackVirtual(1000));

        [Resolved]
        private ExternalAudioManager audio { get; set; }

        public bool IsPlaying => CurrentTrack.IsRunning;

        public bool TrackLoaded => CurrentTrack.TrackLoaded;

        private int currentIdx = -1;

        public Bindable<string> CurrentTrackName { get; private set; } = new Bindable<string>(string.Empty);

        public bool HasCompleted => CurrentTrack.HasCompleted;

        private ScheduledDelegate seekDelegate;

        public MusicController(Storage storage)
        {
            this.storage = storage?.GetStorageForDirectory("tracks");
        }

        public void Play(bool restart = false)
        {
            if (restart)
                CurrentTrack.Restart();
            else if (!IsPlaying)
                CurrentTrack.Start();

            Logger.Log($"Now playing: {CurrentTrackName}");
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

        public void RestartTrack() => Schedule(() => CurrentTrack.Restart());
    }

    public enum TrackChangeDirection
    {
        Forward,
        Backward
    }
}
