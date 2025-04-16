using System;
using Circle.Game.Beatmaps;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;
using osu.Framework.Timing;

namespace Circle.Game.Screens.Play
{
    [Cached(typeof(IGameplayClock))]
    [Cached(typeof(GameplayClockContainer))]
    public partial class GameplayClockContainer : Container, IAdjustableClock, IGameplayClock
    {
        public IBindable<bool> IsPaused => isPaused;

        public double Rate => GameplayClock.Rate;
        public double CurrentTime => GameplayClock.CurrentTime;
        public bool IsRunning => GameplayClock.IsRunning;

        public bool IsRewinding => GameplayClock.IsRewinding;

        public double ElapsedFrameTime => GameplayClock.ElapsedFrameTime;

        public double FramesPerSecond => GameplayClock.FramesPerSecond;

        public double StartTime { get; protected set; }
        public double GameplayStartTime { get; protected set; }

        public FramedBeatmapClock GameplayClock { get; private set; }

        protected override Container<Drawable> Content { get; } = new Container { RelativeSizeAxes = Axes.Both };

        private readonly BindableBool isPaused = new BindableBool(true);

        public GameplayClockContainer(IClock sourceClock, bool applyOffsets, bool requireDecoupling)
        {
            RelativeSizeAxes = Axes.Both;

            InternalChildren = new Drawable[]
            {
                GameplayClock = new FramedBeatmapClock(applyOffsets, requireDecoupling, sourceClock),
                Content
            };
        }

        public virtual void Start()
        {
            if (!isPaused.Value)
                return;

            isPaused.Value = false;

            SchedulerAfterChildren.Add(() =>
            {
                if (isPaused.Value)
                    return;

                StartGameplayClock();
            });
        }

        public virtual void Seek(double time)
        {
            Logger.Log($"{nameof(GameplayClockContainer)} seeking to {time}");

            GameplayClock.Seek(time);

            OnSeek?.Invoke();
        }

        public virtual void Stop()
        {
            if (isPaused.Value)
                return;

            isPaused.Value = true;
            StopGameplayClock();
        }

        public virtual void Reset(double? time = null, bool startClock = false)
        {
            bool wasPaused = isPaused.Value;

            GameplayClock.Stop();

            if (time != null)
                StartTime = time.Value;

            Seek(StartTime);

            if (!wasPaused || startClock)
                Start();
        }

        public void ProcessFrame()
        {
            // Handled via update. Don't process here to safeguard from external usages potentially processing frames additional times.
        }

        protected virtual void StartGameplayClock()
        {
            Logger.Log($"{nameof(GameplayClockContainer)} started via call to {nameof(StartGameplayClock)}");
            GameplayClock.Start();
        }

        protected virtual void StopGameplayClock()
        {
            Logger.Log($"{nameof(GameplayClockContainer)} stopped via call to {nameof(StopGameplayClock)}");
            GameplayClock.Stop();
        }

        protected void ChangeSource(IClock sourceClock) => GameplayClock.ChangeSource(sourceClock);

        public event Action? OnSeek;

        #region IAdjustableClock

        bool IAdjustableClock.Seek(double position)
        {
            Seek(position);
            return true;
        }

        void IAdjustableClock.Reset() => Reset();

        public virtual void ResetSpeedAdjustments()
        {
        }

        double IAdjustableClock.Rate
        {
            get => GameplayClock.Rate;
            set => throw new NotSupportedException();
        }

        #endregion
    }
}
