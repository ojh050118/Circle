using Circle.Game.Beatmap;
using Circle.Game.Rulesets.UI;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Timing;

namespace Circle.Game.Screens.Play
{
    public class MasterGameplayClockContainer : GameplayClockContainer
    {
        private readonly Bindable<BeatmapInfo> beatmap;

        public Playfield Playfield;

        private bool isStarted;

        public MasterGameplayClockContainer(Bindable<BeatmapInfo> beatmap, IClock clock)
            : base(clock)
        {
            this.beatmap = beatmap;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Children = new Drawable[]
            {
                new FrameStabilityContainer(beatmap.Value.Settings.Offset)
                {
                    Child = Playfield = new Playfield(),
                }
            };

            // 음악 시작 시간보다 한 박자 먼저 시작됩니다.
            Seek(beatmap.Value.Settings.Offset - (60000 / beatmap.Value.Settings.Bpm));
        }

        public override void Start()
        {
            base.Start();

            if (!isStarted)
            {
                //Playfield.StartPlaying();
                isStarted = true;
            }
        }

        protected override GameplayClock CreateGameplayClock(IFrameBasedClock source)
        {
            return new GameplayClock(source);
        }
    }
}
