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
            Key.Pause
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
            bluePlanet.Rotation = -90;
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

        private void startPlay()
        {
            playState = GamePlayState.Playing;
            musicController.CurrentTrack.VolumeTo(1);

            bluePlanet.ExpandTo(1, 60000 / beatmap.Value.Settings.BPM, Easing.Out);
            bluePlanet.RotateTo(bluePlanet.Rotation + tiles.Children[tiles.Current].Angle + 90, 60000 / beatmap.Value.Settings.BPM)
                      .Then()
                      .Schedule(() =>
                      {
                          bluePlanet.Expansion = 0;
                          planetState.Value = PlanetState.Fire;
                      });

            Scheduler.AddDelayed(() => musicController.Play(), 60000 / beatmap.Value.Settings.BPM);
        }

        private void movePlanet()
        {
            if (tiles.Current >= tiles.Children.Count)
                return;

            tiles.Current++;

            if (planetState.Value == PlanetState.Fire)
            {
                redPlanet.Expansion = 1;
                redPlanet.RotateTo(redPlanet.Rotation + tiles.Children[tiles.Current].Angle + 90, 60000 / beatmap.Value.Settings.BPM)
                         .Then()
                         .Schedule(() =>
                         {
                             redPlanet.Expansion = 0;
                             bluePlanet.Position = tiles.Children[tiles.Current].Position;
                             redPlanet.Position = tiles.Children[tiles.Current].Position;
                             planetState.Value = PlanetState.Ice;
                         });
            }
            else
            {
                bluePlanet.Expansion = 1;
                bluePlanet.RotateTo(bluePlanet.Rotation + tiles.Children[tiles.Current].Angle + 90, 60000 / beatmap.Value.Settings.BPM)
                          .Then()
                          .Schedule(() =>
                          {
                              bluePlanet.Expansion = 0;
                              bluePlanet.Position = tiles.Children[tiles.Current].Position;
                              redPlanet.Position = tiles.Children[tiles.Current].Position;
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
