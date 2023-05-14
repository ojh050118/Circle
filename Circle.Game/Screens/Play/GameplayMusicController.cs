using System;
using Circle.Game.Beatmaps;
using Circle.Game.Overlays;
using osu.Framework.Audio;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Audio;
using osu.Framework.Graphics.Transforms;
using osu.Framework.Threading;

namespace Circle.Game.Screens.Play
{
    public class GameplayMusicController : MusicController
    {
        public BeatmapInfo BeatmapInfo { get; }

        /// <summary>
        /// 재생 지연 시간.
        /// </summary>
        public double TimeUntilPlay { get; private set; }

        public GameplayMusicController(BeatmapInfo beatmapInfo)
        {
            BeatmapInfo = beatmapInfo;
        }

        protected override void LoadComplete() => ChangeTrack(BeatmapInfo);

        /// <summary>
        /// 오프셋을 설정합니다.
        /// 오프셋이 음수이거나 카운트다운이 오프셋보다 긴 경우 음악을 재생할 때 그 차이만큼 지연됩니다.
        /// </summary>
        /// <param name="offset">게임플레이 시작 오프셋.</param>
        /// <param name="countdown">게임이 시작하기 전 카운트다운 지속시간.</param>
        public void SetOffset(double offset, double countdown)
        {
            if (offset - countdown >= 0)
            {
                TimeUntilPlay = 0;
                SeekTo(offset - countdown);
            }
            else
            {
                TimeUntilPlay = countdown;
                SeekTo(offset);
            }

            SeekTo(Math.Clamp(offset - countdown, 0, double.MaxValue));
        }

        /// <summary>
        /// 음악 재생을 시작합니다.
        /// </summary>
        public void Start()
        {
            VolumeTo(1);
            Scheduler.AddDelayed(() => Play(), TimeUntilPlay);
        }

        /// <summary>
        /// 서서히 음악재생을 중지합니다.
        /// </summary>
        public void Pause() => VolumeTo(0, 750, Easing.OutPow10).Then().Schedule(Stop);

        /// <summary>
        /// 마지막으로 설정된 오프셋을 기준으로 재생을 재개합니다.
        /// 플레이 보장을 위해 DelayUntilTransformsFinished()를 사용해주세요.
        /// </summary>
        public ScheduledDelegate Resume(double timeUntilResume = 0)
        {
            return Scheduler.AddDelayed(() =>
            {
                VolumeTo(1, TimeUntilPlay == 0 ? TimeUntilPlay : 0, Easing.OutPow10);
                Play();
            }, TimeUntilPlay + timeUntilResume);
        }

        public TransformSequence<DrawableTrack> DelayUntilTransformsFinished() => CurrentTrack.DelayUntilTransformsFinished();

        public TransformSequence<DrawableTrack> VolumeTo(double newVolume, double duration = 0, Easing easing = Easing.None) => CurrentTrack.VolumeTo(newVolume, duration, easing);
    }
}
