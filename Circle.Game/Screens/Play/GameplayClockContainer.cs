using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Timing;

namespace Circle.Game.Screens.Play
{
    [Cached]
    public abstract class GameplayClockContainer : Container, IAdjustableClock
    {
        public GameplayClock GameplayClock { get; private set; }

        public readonly BindableBool IsPaused = new BindableBool();

        protected readonly DecoupleableInterpolatingFramedClock AdjustableSource;

        protected IClock SourceClock { get; private set; }

        protected GameplayClockContainer(IClock sourceClock)
        {
            SourceClock = sourceClock;
            RelativeSizeAxes = Axes.Both;
            AdjustableSource = new DecoupleableInterpolatingFramedClock { IsCoupled = false };
            IsPaused.BindValueChanged(OnIspausedChanged);
        }

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            var dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

            dependencies.CacheAs(GameplayClock = CreateGameplayClock(AdjustableSource));
            GameplayClock.IsPaused.BindTo(IsPaused);

            return dependencies;
        }

        public virtual void Start()
        {
            if (!AdjustableSource.IsRunning)
            {
                AdjustableSource.Start();
            }

            IsPaused.Value = false;
        }

        public virtual void Seek(double time)
        {
            AdjustableSource.Seek(time);

            GameplayClock.UnderlyingClock.ProcessFrame();
        }

        public virtual void Stop() => IsPaused.Value = true;

        public virtual void Reset()
        {
            Seek(0);

            AdjustableSource.Stop();

            if (!IsPaused.Value)
                Start();
        }

        protected override void Update()
        {
            if (!IsPaused.Value)
                GameplayClock.UnderlyingClock.ProcessFrame();

            base.Update();
        }

        protected virtual void OnIspausedChanged(ValueChangedEvent<bool> isPaused)
        {
            if (isPaused.NewValue)
                AdjustableSource.Stop();
            else
                AdjustableSource.Start();
        }

        protected abstract GameplayClock CreateGameplayClock(IFrameBasedClock source);

        bool IAdjustableClock.Seek(double position)
        {
            Seek(position);
            return true;
        }

        public void ResetSpeedAdjustments()
        {
        }

        double IAdjustableClock.Rate
        {
            get => GameplayClock.Rate;
            set => throw new NotSupportedException();
        }

        double IClock.Rate => GameplayClock.Rate;

        public double CurrentTime => GameplayClock.CurrentTime;

        public bool IsRunning => GameplayClock.IsRunning;
    }
}
