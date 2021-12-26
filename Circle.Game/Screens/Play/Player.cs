using Circle.Game.Beatmap;
using Circle.Game.Overlays;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osuTK.Input;
using System.Linq;
using osu.Framework.Audio;
using Circle.Game.Rulesets.Objects;
using osuTK.Graphics;
using Circle.Game.Rulesets.Extensions;

namespace Circle.Game.Screens.Play
{
    public class Player : CircleScreen
    {
        public override bool FadeBackground => false;

        private GamePlayState playState = GamePlayState.NotPlaying;

        private readonly Key[] blockedKeys =
        {
            Key.AltLeft,
            Key.AltRight,
            Key.BackSpace,
            Key.CapsLock,
            Key.ControlLeft,
            Key.ControlRight,
            Key.Delete,
            Key.Enter,
            Key.Home,
            Key.Insert,
            Key.End,
            Key.PageDown,
            Key.PageUp,
            Key.PrintScreen,
            Key.ScrollLock,
            Key.Pause,
            Key.LWin,
            Key.RWin
        };

        [Resolved]
        private MusicController musicController { get; set; }

        [Resolved]
        private Bindable<BeatmapInfo> beatmap { get; set; }

        private readonly ObjectContainer tiles;
        private readonly Planet redPlanet;
        private readonly Planet bluePlanet;

        /// <summary>
        /// 현재 회전하는 행성을 가리킵니다.
        /// </summary>
        private readonly Bindable<PlanetState> planetState;

        public Player()
        {
            InternalChildren = new Drawable[]
            {
                tiles = new ObjectContainer(),
                redPlanet = new Planet(Color4.Red),
                bluePlanet = new Planet(Color4.DeepSkyBlue)
            };

            redPlanet.Expansion = bluePlanet.Expansion = 0;
            bluePlanet.Rotation = -180;
            planetState = new Bindable<PlanetState>(PlanetState.Ice);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            musicController.CurrentTrack.VolumeTo(0, 500, Easing.Out)
                           .Then()
                           .Schedule(() =>
                           {
                               musicController.Stop();
                               musicController.SeekTo(beatmap.Value.Settings.Offset);
                           });

            playState = GamePlayState.Ready;
            planetState.ValueChanged += _ => movePlanet();
        }

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            if (blockedKeys.Contains(e.Key))
                return false;

            switch (playState)
            {
                case GamePlayState.Ready:
                    startPlay();
                    break;

                case GamePlayState.Playing:
                    tiles.MoveCamera();
                    break;

                case GamePlayState.Complete:
                    OnExit();
                    break;
            }

            return base.OnKeyDown(e);
        }

        /// <summary>
        /// 게임 플레이 상태가 준비되었을때 클릭하면 이 메서드가 호출됩니다.
        /// 회전은 먼저 파란 행성이 사작하며, -180도에서 회전과 행성간 거리를 확장하며 사작합니다.
        /// </summary>
        private void startPlay()
        {
            playState = GamePlayState.Playing;
            musicController.CurrentTrack.VolumeTo(1);

            bluePlanet.ExpandTo(1, 60000 / beatmap.Value.Settings.BPM, Easing.Out);
            bluePlanet.RotateTo(bluePlanet.Rotation + tiles.Children[tiles.Current].Angle + 180, 60000 / beatmap.Value.Settings.BPM)
                      .Then()
                      .Schedule(() =>
                      {
                          bluePlanet.Expansion = 0;
                          planetState.Value = PlanetState.Fire;
                      });

            Scheduler.AddDelayed(() => musicController.Play(), 60000 / beatmap.Value.Settings.BPM);
        }

        /// <summary>
        /// 행성이 회전을 마쳤을 때 메서드가 호출됩니다.
        /// </summary>
        private void movePlanet()
        {
            tiles.Current++;
            if (tiles.Current >= tiles.Children.Count)
                return;

            var currentAngle = tiles.Children[tiles.Current].Angle;
            //var nextAngle = tiles.Children[tiles.Current + 1].Angle;

            if (planetState.Value == PlanetState.Fire)
            {
                redPlanet.Expansion = 1;
                redPlanet.Rotation -= 180;
                redPlanet.RotateTo(redPlanet.Rotation + currentAngle, 60000 / beatmap.Value.Settings.BPM)
                         .Then()
                         .Schedule(() =>
                         {
                             redPlanet.Expansion = 0;
                             redPlanet.Position = tiles.Children[tiles.Current].Position;
                             planetState.Value = PlanetState.Ice;
                         });
            }
            else
            {
                bluePlanet.Expansion = 1;
                bluePlanet.RotateTo(bluePlanet.Rotation + currentAngle, 60000 / beatmap.Value.Settings.BPM)
                          .Then()
                          .Schedule(() =>
                          {
                              bluePlanet.Expansion = 0;
                              bluePlanet.Position = tiles.Children[tiles.Current].Position;
                              planetState.Value = PlanetState.Fire;
                          });
            }

            this.MoveTo(-tiles.PlanetPositions[tiles.Current], 250, Easing.OutSine);
        }
    }

    public enum PlanetState
    {
        Fire,
        Ice
    }
}
