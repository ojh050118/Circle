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

        [Resolved]
        private CircleConfigManager localConfig { get; set; }

        public bool IsPlaying => CurrentTrack.IsRunning;

        public bool TrackLoaded => CurrentTrack.TrackLoaded;

        public static List<string> TrackList { get; private set; }

        private int currentIdx = -1;

        public Bindable<string> CurrentTrackName { get; private set; } = new Bindable<string>(string.Empty);

        public bool HasCompleted => CurrentTrack.HasCompleted;

        public Bindable<bool> IsPlayingBindable { get; private set; } = new Bindable<bool>(false);

        private ScheduledDelegate seekDelegate;

        public MusicController(Storage storage)
        {
            this.storage = storage?.GetStorageForDirectory("tracks");
            TrackList = this.storage?.GetFiles(string.Empty).ToList();
        }

        [BackgroundDependencyLoader]
        private void load(ExternalAudioManager audio, Storage storage)
        {
            this.audio = audio;
        }

        public void Play(bool restart = false)
        {
            if (currentIdx == -1)
                return;

            if (restart)
                CurrentTrack.Restart();
            else if (!IsPlaying)
                CurrentTrack.Start();

            IsPlayingBindable.Value = true;
            Logger.Log($"Now playing: {CurrentTrackName}");
        }

        public void Stop()
        {
            if (IsPlaying)
                CurrentTrack.Stop();

            IsPlayingBindable.Value = false;
        }

        public void SeekTo(double position)
        {
            seekDelegate?.Cancel();
            seekDelegate = Schedule(() =>
            {
                CurrentTrack.Seek(position);
            });
        }

        /// <summary>
        /// 특정 트랙으로 바꿈.
        /// </summary>
        /// <param name="name">트랙 이름.</param>
        public void ChangeTrack(string name)
        {
            if (TrackList.Count == 0)
                return;

            // 메모리 사용이 증가하는것을 막습니다. 근본적인 원인을 찾고 고치기 전까지 이 방법을 사용합니다.
            if (name.Equals(CurrentTrackName))
            {
                Play(true);
                return;
            }

            currentIdx = TrackList.IndexOf(name);
            Stop();
            CurrentTrack = new DrawableTrack(audio.Tracks.Get(TrackList[currentIdx]));
            CurrentTrackName.Value = TrackList[currentIdx];
            Play();
        }

        /// <summary>
        /// 방향으로 트랙을 바꿈.
        /// </summary>
        /// <param name="direction">방향.</param>
        public void ChangeTrack(TrackChangeDirection direction)
        {
            if (TrackList.Count == 0)
                return;

            if (direction == TrackChangeDirection.Forward)
            {
                if (currentIdx + 1 >= TrackList.Count)
                    currentIdx = 0;
                else
                    currentIdx++;
            }
            else
            {
                if (currentIdx - 1 < 0)
                    currentIdx = TrackList.Count - 1;
                else
                    currentIdx--;
            }

            Stop();
            CurrentTrack = new DrawableTrack(audio.Tracks.Get(TrackList[currentIdx]));
            CurrentTrackName.Value = TrackList[currentIdx];
            Play();
        }

        public void RestartTrack() => Schedule(() => CurrentTrack.Restart());

        public void ReloadTracks(bool userRequested = false)
        {
            TrackList = storage.GetFiles(string.Empty).ToList();

            Logger.Log($"Reloaded tracks. Count: {TrackList.Count}");
        }

        public void ReloadTracks(List<string> newTrackList)
        {
            TrackList = newTrackList;
            Logger.Log($"Reloaded trackList. Count: {TrackList.Count}");
        }

        /// <summary>
        /// 트랙리스트 마지막에 트랙을 추가함.
        /// </summary>
        /// <param name="name">트랙 이름.</param>
        public void AddTrack(string name) => TrackList.Add(name);
    }

    public enum TrackChangeDirection
    {
        Forward,
        Backward
    }
}
