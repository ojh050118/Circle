using System.Linq;
using Circle.Game.Beatmap;
using Circle.Game.Graphics.UserInterface;
using Circle.Game.Overlays;
using Circle.Game.Screens.Setting;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK.Input;

namespace Circle.Game.Screens.Play
{
    public class Player : CircleScreen
    {
        public override bool BlockExit => !IsLoaded;
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

        [Resolved]
        private DialogOverlay dialog { get; set; }

        private MasterGameplayClockContainer masterGameplayClockContainer;

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren = new Drawable[]
            {
                masterGameplayClockContainer = new MasterGameplayClockContainer(beatmap, Clock),
            };

            masterGameplayClockContainer.Playfield.OnComplete = () => playState = GamePlayState.Complete;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            musicController.CurrentTrack.VolumeTo(0, 500, Easing.Out)
                           .Then()
                           .Schedule(() =>
                           {
                               musicController.Stop();
                               musicController.ChangeTrack(beatmap.Value);
                               musicController.SeekTo(beatmap.Value.Settings.Offset);
                               playState = GamePlayState.Ready;
                           });
        }

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            if (blockedKeys.Contains(e.Key))
                return false;

            switch (playState)
            {
                case GamePlayState.Ready:
                    masterGameplayClockContainer.Start();
                    musicController.CurrentTrack.VolumeTo(1);
                    Scheduler.AddDelayed(() => musicController.Play(), 60000 / beatmap.Value.Settings.Bpm);
                    playState = GamePlayState.Playing;
                    break;

                case GamePlayState.Playing:
                    break;

                case GamePlayState.Complete:
                    OnExit();
                    break;
            }

            return base.OnKeyDown(e);
        }

        protected override void Update()
        {
            base.Update();

            if (masterGameplayClockContainer.CurrentTime >= masterGameplayClockContainer.Playfield.EndTime)
                playState = GamePlayState.Complete;
        }

        public override void OnExit()
        {
            if (playState == GamePlayState.Playing)
            {
                onPaused();
                return;
            }

            Schedule(() =>
            {
                musicController.CurrentTrack.VolumeTo(1);
                musicController.Play();
            });

            base.OnExit();
        }

        protected override void OnResume()
        {
            base.OnResume();
            onPaused();
        }

        private void onPaused()
        {
            masterGameplayClockContainer.Stop();
            musicController.CurrentTrack.VolumeTo(0, 250, Easing.OutPow10).Then().Schedule(musicController.Stop);

            dialog.Title = "Paused";
            dialog.Description = "Game paused";

            dialog.Buttons = new[]
            {
                new DialogButton
                {
                    Text = "Exit",
                    Action = () =>
                    {
                        playState = GamePlayState.NotPlaying;
                        OnExit();
                        dialog.Hide();
                    }
                },
                new DialogButton
                {
                    Text = "Settings",
                    Action = () =>
                    {
                        this.Push(new SettingsScreen());
                        dialog.Hide();
                    }
                },
                new DialogButton
                {
                    Text = "Resume",
                    Font = FontUsage.Default.With(family: "OpenSans-Bold", size: 28),
                    Action = () =>
                    {
                        Scheduler.AddDelayed(() =>
                        {
                            playState = GamePlayState.Playing;
                            musicController.SeekTo(masterGameplayClockContainer.CurrentTime);
                            masterGameplayClockContainer.Start();
                            musicController.CurrentTrack.VolumeTo(1);
                            musicController.Play();
                        }, 1000);
                        dialog.Hide();
                    }
                }
            };
            dialog.Push();
        }
    }

    public enum PlanetState
    {
        Fire,
        Ice
    }
}
