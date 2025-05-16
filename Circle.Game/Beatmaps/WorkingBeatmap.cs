#nullable disable

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Extensions;
using osu.Framework.Graphics.Textures;
using osu.Framework.Logging;

namespace Circle.Game.Beatmaps
{
    public abstract class WorkingBeatmap : IWorkingBeatmap
    {
        public readonly BeatmapInfo BeatmapInfo;
        public readonly BeatmapSetInfo BeatmapSetInfo;

        public BeatmapMetadata Metadata => BeatmapInfo.Metadata;

        private readonly AudioManager audioManager;

        private readonly object beatmapFetchLock = new object();

        private CancellationTokenSource loadCancellationSource = new CancellationTokenSource();

        protected WorkingBeatmap(BeatmapInfo beatmapInfo, AudioManager audioManager)
        {
            this.audioManager = audioManager;
            BeatmapInfo = beatmapInfo;
            BeatmapSetInfo = BeatmapInfo.BeatmapSet ?? new BeatmapSetInfo();
        }

        protected abstract Beatmap GetBeatmap();
        protected abstract Track GetBeatmapTrack();
        public abstract Texture GetBackground();
        public abstract Stream GetVideo();
        public abstract byte[] Get(string name);

        #region Async load control

        public void BeginAsyncLoad() => loadBeatmapAsync();

        public void CancelAsyncLoad()
        {
            lock (beatmapFetchLock)
            {
                loadCancellationSource?.Cancel();
                loadCancellationSource = new CancellationTokenSource();

                if (beatmapLoadTask?.IsCompleted != true)
                    beatmapLoadTask = null;
            }
        }

        #endregion

        #region Track

        public virtual bool TrackLoaded => track != null;

        private Track track;

        public Track Track
        {
            get
            {
                if (!TrackLoaded)
                    throw new InvalidOperationException("Track is not loaded! LoadTrack() must be called before accessing Track.");

                return track;
            }
        }

        public Track LoadTrack()
        {
            track = GetBeatmapTrack() ?? GetVirtualTrack(1000);

            return track;
        }

        protected Track GetVirtualTrack(double emptyLength = 0)
        {
            const double excess_length = 1000;

            double length = BeatmapInfo?.Length + excess_length ?? emptyLength;

            return audioManager.Tracks.GetVirtual(length);
        }

        #endregion

        #region Beatmap

        public bool BeatmapLoaded
        {
            get
            {
                lock (beatmapFetchLock)
                    return beatmapLoadTask?.IsCompleted ?? false;
            }
        }

        public Beatmap Beatmap
        {
            get
            {
                try
                {
                    return loadBeatmapAsync().GetResultSafely();
                }
                catch (AggregateException ae)
                {
                    // This is the exception that is generally expected here, which occurs via natural cancellation of the asynchronous load
                    if (ae.InnerExceptions.FirstOrDefault() is TaskCanceledException)
                        return null;

                    Logger.Error(ae, $"Failed to load beatmap ({BeatmapInfo}).");
                    return null;
                }
                catch (Exception e)
                {
                    Logger.Error(e, $"Failed to load beatmap ({BeatmapInfo}).");
                    return null;
                }
            }
        }

        private Task<Beatmap> beatmapLoadTask;

        private Task<Beatmap> loadBeatmapAsync()
        {
            lock (beatmapFetchLock)
            {
                return beatmapLoadTask ??= Task.Factory.StartNew(() =>
                {
                    var b = GetBeatmap() ?? new Beatmap();

                    b.BeatmapInfo.ID = BeatmapInfo.ID;
                    b.BeatmapInfo.BeatmapSet = BeatmapSetInfo;
                    b.BeatmapInfo.Metadata = BeatmapInfo.Metadata;

                    return b;
                }, loadCancellationSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
        }

        #endregion

        BeatmapInfo IWorkingBeatmap.BeatmapInfo => BeatmapInfo;

        public abstract Stream GetStream(string storagePath);
    }
}
