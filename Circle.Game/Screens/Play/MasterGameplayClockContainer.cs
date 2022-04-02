using Circle.Game.Beatmaps;
using Circle.Game.Rulesets.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Video;
using osu.Framework.Timing;

namespace Circle.Game.Screens.Play
{
    public class MasterGameplayClockContainer : GameplayClockContainer
    {
        private readonly BeatmapInfo info;
        private readonly Beatmap beatmap;
        private readonly double gameplayStartTime;
        private readonly double startOffset;

        public Playfield Playfield;

        private FrameStabilityContainer container;

        private bool isStarted;

        public MasterGameplayClockContainer(BeatmapInfo info, double gameplayStartTime, double startOffset, IClock clock)
            : base(clock)
        {
            beatmap = info.Beatmap;
            this.info = info;
            this.gameplayStartTime = gameplayStartTime;
            this.startOffset = startOffset;
        }

        [BackgroundDependencyLoader]
        private void load(BeatmapStorage storage)
        {
            Children = new Drawable[]
            {
                container = new FrameStabilityContainer(beatmap.Settings.VidOffset + gameplayStartTime - startOffset),
                new FrameStabilityContainer(gameplayStartTime)
                {
                    Child = Playfield = new Playfield(beatmap, gameplayStartTime, startOffset),
                }
            };

            var stream = storage.GetVideo(info);

            if (stream != null)
            {
                container.Add(new Video(stream, false)
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    FillMode = FillMode.Fill,
                    RelativeSizeAxes = Axes.Both,
                    Loop = false,
                    PlaybackPosition = beatmap.Settings.VidOffset + gameplayStartTime - startOffset,
                });
            }

            // 음악 시작 시간보다 한 박자 먼저 시작됩니다.
            Seek(gameplayStartTime);
        }

        public override void Start()
        {
            base.Start();

            if (!isStarted)
                isStarted = true;
        }

        protected override GameplayClock CreateGameplayClock(IFrameBasedClock source)
        {
            return new GameplayClock(source);
        }
    }
}
