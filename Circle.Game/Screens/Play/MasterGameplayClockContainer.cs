#nullable disable

using System;
using System.Linq;
using Circle.Game.Beatmaps;
using Circle.Game.Rulesets.Objects;
using Circle.Game.Rulesets.UI;
using Circle.Game.Screens.Play.HUD;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using osu.Framework.Threading;

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

        public GameplayState State { get; private set; }

        protected Playfield Playfield;
        private HUDOverlay hud;

        internal bool ShouldValidatePlaybackRate { get; init; }

        private readonly Bindable<bool> playbackRateValid = new Bindable<bool>(true);

        private bool isStarted;

        private readonly WorkingBeatmap beatmap;
        private Track track;
        private Sample hitSound;

        private Tile[] tiles;
        private int floor;
        private bool autoplay = true;

        public MasterGameplayClockContainer(WorkingBeatmap working, double gameplayStartTime, Track track)
            : base(new TrackVirtual(double.MaxValue), applyOffsets: true, requireDecoupling: true)
        {
            beatmap = working;

            GameplayStartTime = gameplayStartTime;
            StartTime = working.Beatmap.Metadata.CountdownDuration;

            // TODO: 이거 정말 이래야해요?
            this.track = new TrackVirtual(track.Length);
        }

        [BackgroundDependencyLoader]
        private void load(AudioManager audioManager)
        {
            tiles = beatmap.Beatmap.Tiles.ToArray();
            hitSound = audioManager.Samples.Get(@"normal-hitnormal.wav");

            Children = new Drawable[]
            {
                new FrameStabilityContainer(GameplayStartTime)
                {
                    Children = new Drawable[]
                    {
                        Playfield = new Playfield(beatmap.Beatmap),
                        new InputManager(beatmap.Beatmap)
                        {
                            RelativeSizeAxes = Axes.Both,
                            OnInputReceived = Playfield.HandleInput
                        }
                    }
                },
                hud = new HUDOverlay(beatmap.Beatmap),
            };

            // 음악 시작 시간보다 한 박자 먼저 시작됩니다.
            Seek(GameplayStartTime);
        }

        protected override void LoadComplete()
        {
        }

        protected override void Update()
        {
            if (!autoplay || floor >= tiles.Length)
                return;

            if (CurrentTime >= tiles[floor].EndTime)
            {
                hitSound?.Play();

                if (autoplay)
                {
                    floor++;
                    hud.UpdateProgress(floor);
                }
            }

            if (floor >= tiles.Length)
            {
                hud.Complete();
                State = GameplayState.Complete;
            }
        }

        public override void Start()
        {
            base.Start();

            if (!isStarted)
            {
                isStarted = true;
                hud.Start();
            }

            State = GameplayState.Playing;
        }

        public ScheduledDelegate Resume(double countdown)
        {
            if (IsRunning)
                return null;

            State = GameplayState.Ready;
            hud.Start();
            return Scheduler.AddDelayed(Start, countdown);
        }

        public override void Stop()
        {
            base.Stop();

            State = GameplayState.NotPlaying;
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
