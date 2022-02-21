﻿using System;
using System.Linq;
using Circle.Game.Beatmap;
using Circle.Game.Graphics.UserInterface;
using Circle.Game.Overlays;
using Circle.Game.Screens.Setting;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osu.Framework.Threading;
using osuTK;
using osuTK.Graphics;
using osuTK.Input;

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
        private BeatmapManager beatmap { get; set; }

        [Resolved]
        private DialogOverlay dialog { get; set; }

        private MasterGameplayClockContainer masterGameplayClockContainer;

        private ScheduledDelegate scheduledDelegate;

        [BackgroundDependencyLoader]
        private void load(GameHost host)
        {
            InternalChildren = new Drawable[]
            {
                masterGameplayClockContainer = new MasterGameplayClockContainer(beatmap.CurrentBeatmap, Clock),
                new Container
                {
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Y = 32,
                    Children = new Drawable[]
                    {
                        new SpriteText
                        {
                            Text = $"{beatmap.CurrentBeatmap.Settings.Artist} - {beatmap.CurrentBeatmap.Settings.Song}",
                            Font = FontUsage.Default.With(family: "OpenSans-Bold", size: 32),
                            Shadow = true,
                            ShadowColour = Color4.Black.Opacity(0.4f),
                        }
                    }
                },
            };

            if (!host.CanExit)
            {
                AddInternal(new IconButton
                {
                    Anchor = Anchor.TopRight,
                    Origin = Anchor.TopRight,
                    Margin = new MarginPadding(10),
                    Size = new Vector2(50),
                    Icon = FontAwesome.Solid.Pause,
                    Action = onPaused
                });
            }

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
                               musicController.ChangeTrack(beatmap.CurrentBeatmap);
                               musicController.SeekTo(beatmap.CurrentBeatmap.Settings.Offset);
                               playState = GamePlayState.Ready;
                           });
        }

        protected override bool OnClick(ClickEvent e)
        {
            updateState();

            return base.OnClick(e);
        }

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            if (blockedKeys.Contains(e.Key))
                return false;

            updateState();

            return base.OnKeyDown(e);
        }

        protected override void Update()
        {
            base.Update();

            if (masterGameplayClockContainer.CurrentTime >= masterGameplayClockContainer.Playfield.EndTime)
            {
                playState = GamePlayState.Complete;
                dialog.BlockInputAction = false;
            }
        }

        private void updateState()
        {
            switch (playState)
            {
                case GamePlayState.Ready:
                    masterGameplayClockContainer.Start();
                    musicController.CurrentTrack.VolumeTo(1);
                    Scheduler.AddDelayed(() => musicController.Play(), 60000 / beatmap.CurrentBeatmap.Settings.Bpm);
                    playState = GamePlayState.Playing;
                    break;

                case GamePlayState.Playing:
                    break;

                case GamePlayState.Complete:
                    OnExit();
                    break;
            }
        }

        public override void OnExit()
        {
            if (playState == GamePlayState.Playing)
            {
                onPaused();
                return;
            }

            musicController.CurrentTrack.DelayUntilTransformsFinished().Schedule(() =>
            {
                musicController.CurrentTrack.VolumeTo(1);
                musicController.Play();
            });

            base.OnExit();
        }

        public override bool OnExiting(IScreen next)
        {
            this.FadeOut(1000, Easing.OutPow10);

            return base.OnExiting(next);
        }

        public override void OnResuming(IScreen last)
        {
            base.OnResuming(last);

            onPaused();
        }

        private void onPaused()
        {
            masterGameplayClockContainer.Stop();
            musicController.CurrentTrack.VolumeTo(0, 1000, Easing.OutPow10).Then().Schedule(musicController.Stop);
            scheduledDelegate?.Cancel();

            dialog.Title = "Paused";
            dialog.Description = "Game paused";
            dialog.BlockInputAction = true;

            dialog.Buttons = new[]
            {
                new DialogButton
                {
                    Text = "Exit",
                    TextColour = Color4.Red,
                    Action = () =>
                    {
                        playState = GamePlayState.NotPlaying;
                        OnExit();
                        dialog.Hide();
                        dialog.BlockInputAction = false;
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
                        musicController.CurrentTrack.DelayUntilTransformsFinished().Schedule(() =>
                        {
                            if (masterGameplayClockContainer.CurrentTime - 1000 >= 0)
                            {
                                musicController.SeekTo(masterGameplayClockContainer.CurrentTime - 1000);
                                musicController.Play();
                                musicController.CurrentTrack.VolumeTo(1, 1000, Easing.OutPow10);
                            }

                            scheduledDelegate = Scheduler.AddDelayed(() =>
                            {
                                if (masterGameplayClockContainer.CurrentTime - 1000 < 0)
                                {
                                    Scheduler.AddDelayed(() =>
                                    {
                                        musicController.SeekTo(masterGameplayClockContainer.CurrentTime);
                                        musicController.CurrentTrack.VolumeTo(1);
                                        musicController.Play();
                                    }, Math.Abs(masterGameplayClockContainer.CurrentTime));
                                }

                                playState = GamePlayState.Playing;
                                masterGameplayClockContainer.Start();
                            }, 1000);
                        });
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
