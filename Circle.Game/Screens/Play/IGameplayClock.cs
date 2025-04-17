using osu.Framework.Bindables;
using osu.Framework.Timing;

namespace Circle.Game.Screens.Play
{
    public interface IGameplayClock : IFrameBasedClock
    {
        /// <summary>
        /// The time from which the clock should start. Will be seeked to on calling <see cref="GameplayClockContainer.Reset"/>.
        /// </summary>
        /// <remarks>
        /// By default, a value of zero will be used.
        /// Importantly, the value will be inferred from the current beatmap in <see cref="MasterGameplayClockContainer"/> by default.
        /// </remarks>
        double StartTime { get; }

        /// <summary>
        /// The time from which actual gameplay should start. When intro time is skipped, this will be the seeked location.
        /// </summary>
        double GameplayStartTime { get; }

        /// <summary>
        /// Whether gameplay is paused.
        /// </summary>
        IBindable<bool> IsPaused { get; }

        /// <summary>
        /// Whether the clock is currently rewinding.
        /// </summary>
        bool IsRewinding { get; }
    }
}
