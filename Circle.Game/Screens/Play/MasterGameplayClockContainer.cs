#nullable disable

using System;
using Circle.Game.Beatmaps;
using Circle.Game.Rulesets.UI;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Video;
using osu.Framework.Logging;

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

        private readonly double countdownDuration;
        private readonly double gameplayStartTime;

        private FrameStabilityContainer container;

        private bool isStarted;

        private Track track;

        [Resolved]
        private Bindable<WorkingBeatmap> workingBeatmap { get; set; }

        public MasterGameplayClockContainer(BeatmapInfo info, double gameplayStartTime, double countdownDuration, Track track)
            : base(new TrackVirtual(track.Length), applyOffsets: true, requireDecoupling: true)
        {
            this.gameplayStartTime = gameplayStartTime;
            this.countdownDuration = countdownDuration;

            GameplayStartTime = gameplayStartTime;
            StartTime = gameplayStartTime;

            this.track = new TrackVirtual(track.Length);
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Children = new Drawable[]
            {
                container = new FrameStabilityContainer(workingBeatmap.Value.Metadata.VidOffset + gameplayStartTime - countdownDuration),
                new FrameStabilityContainer(gameplayStartTime)
                {
                    Child = new InputManager
                    {
                        RelativeSizeAxes = Axes.Both,
                        Beatmap = workingBeatmap.Value.Beatmap,
                        Child = Playfield = new Playfield(workingBeatmap.Value.Beatmap)
                    }
                }
            };

            var stream = workingBeatmap.Value.GetVideo();

            if (stream != null)
            {
                container.Add(new Video(stream, false)
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    FillMode = FillMode.Fill,
                    RelativeSizeAxes = Axes.Both,
                    Loop = false,
                    PlaybackPosition = workingBeatmap.Value.Metadata.VidOffset + gameplayStartTime - countdownDuration,
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

        public void Skip()
        {
            if (GameplayClock.CurrentTime > GameplayStartTime - MINIMUM_SKIP_TIME)
                return;

            double skipTarget = GameplayStartTime - MINIMUM_SKIP_TIME;

            if (StartTime < -10000 && GameplayClock.CurrentTime < 0 && skipTarget > 6000)
                skipTarget = 0;

            Seek(skipTarget);
        }

        private bool speedAdjustmentsApplied;

        public void StopUsingBeatmapClock()
        {
            removeAdjustmentsFromTrack();

            track = new TrackVirtual(track.Length);
            track.Seek(CurrentTime);
            if (IsRunning)
                track.Start();
            ChangeSource(track);

            addAdjustmentsToTrack();
        }

        private void addAdjustmentsToTrack()
        {
            if (speedAdjustmentsApplied)
                return;

            track.AddAdjustment(AdjustableProperty.Frequency, UserPlaybackRate);

            speedAdjustmentsApplied = true;
        }

        private void removeAdjustmentsFromTrack()
        {
            if (!speedAdjustmentsApplied)
                return;

            track.RemoveAdjustment(AdjustableProperty.Frequency, UserPlaybackRate);

            speedAdjustmentsApplied = false;
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            removeAdjustmentsFromTrack();
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
