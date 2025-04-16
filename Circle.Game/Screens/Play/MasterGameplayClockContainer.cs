#nullable disable

using System;
using Circle.Game.Beatmaps;
using Circle.Game.Rulesets.UI;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Video;
using osu.Framework.Logging;
using osu.Framework.Timing;

namespace Circle.Game.Screens.Play
{
    public partial class MasterGameplayClockContainer : GameplayClockContainer
    {
        public const double MINIMUM_SKIP_TIME = 1000;

        public IBindable<bool> PlaybackRateValid => playbackRateValid;

        public readonly BindableNumber<double> UserPlaybackRate = new BindableDouble(1)
        {
            MinValue = 0.05,
            MaxValue = 2,
            Precision = 0.01,
        };

        public Playfield Playfield;

        internal bool ShouldValidatePlaybackRate { get; init; }

        private readonly Bindable<bool> playbackRateValid = new Bindable<bool>(true);

        private readonly Beatmap beatmap;
        private readonly double countdownDuration;
        private readonly double gameplayStartTime;
        private readonly BeatmapInfo info;

        private FrameStabilityContainer container;

        private bool isStarted;

        public MasterGameplayClockContainer(BeatmapInfo info, double gameplayStartTime, double countdownDuration, IClock clock)
            : base(clock, applyOffsets: true, requireDecoupling: true)
        {
            beatmap = info.Beatmap;
            this.info = info;
            this.gameplayStartTime = gameplayStartTime;
            this.countdownDuration = countdownDuration;

            GameplayStartTime = gameplayStartTime;
            StartTime = gameplayStartTime;
        }

        public override void Start()
        {
            base.Start();

            if (!isStarted)
                isStarted = true;
        }

        public void Skip()
        {
            if (GameplayClock.CurrentTime > GameplayStartTime - MINIMUM_SKIP_TIME)
                return;

            double skipTarget = GameplayStartTime - MINIMUM_SKIP_TIME;

            if (StartTime < -10000 && GameplayClock.CurrentTime < 0 && skipTarget > 6000)
                skipTarget = 0;

            Seek(skipTarget);
        }

        [BackgroundDependencyLoader]
        private void load(BeatmapStorage storage)
        {
            Children = new Drawable[]
            {
                container = new FrameStabilityContainer(beatmap.Settings.VidOffset + gameplayStartTime - countdownDuration),
                new FrameStabilityContainer(gameplayStartTime)
                {
                    Child = new InputManager
                    {
                        RelativeSizeAxes = Axes.Both,
                        Beatmap = beatmap,
                        Child = Playfield = new Playfield(beatmap, gameplayStartTime, countdownDuration)
                    }
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
                    PlaybackPosition = beatmap.Settings.VidOffset + gameplayStartTime - countdownDuration,
                });
            }

            // 음악 시작 시간보다 한 박자 먼저 시작됩니다.
            Seek(gameplayStartTime);
        }

        #region Clock validation (ensure things are running correctly for local gameplay)

        private double elapsedGameplayClockTime;
        private double? elapsedValidationTime;
        private int playbackDiscrepancyCount;

        private const int allowed_playback_discrepancies = 5;

        private void checkPlaybackValidity()
        {
            if (!ShouldValidatePlaybackRate)
                return;

            if (GameplayClock.IsRunning)
            {
                elapsedGameplayClockTime += GameplayClock.ElapsedFrameTime;

                if (elapsedValidationTime == null)
                    elapsedValidationTime = elapsedGameplayClockTime;
                else
                    elapsedValidationTime += GameplayClock.Rate * Time.Elapsed;

                if (Math.Abs(elapsedGameplayClockTime - elapsedValidationTime!.Value) > 300)
                {
                    if (playbackDiscrepancyCount++ > allowed_playback_discrepancies)
                    {
                        if (playbackRateValid.Value)
                        {
                            playbackRateValid.Value = false;
                            Logger.Log("System audio playback is not working as expected. Some online functionality will not work.\n\nPlease check your audio drivers.", level: LogLevel.Important);
                        }
                    }
                    else
                    {
                        Logger.Log(
                            $"Playback discrepancy detected ({playbackDiscrepancyCount} of allowed {allowed_playback_discrepancies}): {elapsedGameplayClockTime:N1} vs {elapsedValidationTime:N1}");
                    }

                    elapsedValidationTime = null;
                }
            }
        }

        #endregion
    }
}
