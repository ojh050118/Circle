using System.Linq;
using Circle.Game.Beatmaps;
using Circle.Game.Rulesets.Objects;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;

namespace Circle.Game.Graphics.Containers
{
    public partial class BeatSyncedContainer : Container
    {
        private int lastBeat;

        private double lastTimingPoint { get; set; }

        protected double TimingPoint { get; private set; }

        /// <summary>
        /// The time in milliseconds until the next beat.
        /// </summary>
        public double TimeUntilNextBeat { get; private set; }

        /// <summary>
        /// The time in milliseconds since the last beat
        /// </summary>
        public double TimeSinceLastBeat { get; private set; }

        public int BeatIndex { get; private set; }

        protected readonly Tile[] Tiles;

        public BeatSyncedContainer(Beatmap beatmap)
        {
            Tiles = beatmap.Tiles.ToArray();
        }

        protected virtual void OnNewBeat(int beatIndex, double timingPoint)
        {
            Logger.Log($"Beat: {beatIndex} | Timing: {timingPoint}");
        }

        protected override void Update()
        {
            if (BeatIndex >= Tiles.Length)
                return;

            TimingPoint = Tiles[BeatIndex].HitTime;

            TimeUntilNextBeat = TimingPoint - Time.Current;
            TimeSinceLastBeat = Time.Current - lastTimingPoint;

            if (Time.Current >= TimingPoint)
            {
                lastBeat = BeatIndex;
                lastTimingPoint = TimingPoint;

                OnNewBeat(lastBeat, TimingPoint);

                // 다음 비트를 나타냅니다.
                BeatIndex++;
            }
        }
    }
}
