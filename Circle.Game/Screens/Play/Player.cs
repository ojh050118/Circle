using System;
using System.Collections.Generic;
using System.Linq;
using Circle.Game.Beatmaps;
using Circle.Game.Configuration;
using Circle.Game.Graphics;
using Circle.Game.Graphics.UserInterface;
using Circle.Game.Overlays;
using Circle.Game.Rulesets.Extensions;
using Circle.Game.Screens.Setting;
using osu.Framework.Allocation;
using osu.Framework.Audio;
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
        private DialogOverlay dialog { get; set; }

        [Resolved]
        private Background background { get; set; }

        [Resolved]
        private CircleConfigManager localConfig { get; set; }

        private TextureSource textureSource;

        private string textureName;

        private MasterGameplayClockContainer masterGameplayClockContainer;
        private SpriteText complete;
        private SpriteText progress;
        private GameProgressBar bar;

        private ScheduledDelegate scheduledDelegate;

        private readonly BeatmapInfo beatmapInfo;
        private readonly Beatmap currentBeatmap;

        private bool parallaxEnabled;
        private double endTime;
        private List<double> hitTimes;

        private int floor = 1;
        private float percent;

        public Player(BeatmapInfo beatmapInfo)
        {
            this.beatmapInfo = beatmapInfo;
            currentBeatmap = beatmapInfo.Beatmap;
        }

        [BackgroundDependencyLoader]
        private void load(GameHost host, CircleColour colours)
        {
            parallaxEnabled = localConfig.Get<bool>(CircleSetting.Parallax);
            textureSource = background.TextureSource;
            textureName = background.TextureName;
            endTime = CalculationExtensions.GetTileHitTime(currentBeatmap, currentBeatmap.Settings.Offset - 60000 / currentBeatmap.Settings.Bpm).Last();
            hitTimes = CalculationExtensions.GetTileHitTime(currentBeatmap, currentBeatmap.Settings.Offset - 60000 / currentBeatmap.Settings.Bpm).ToList();
            InternalChildren = new Drawable[]
            {
                masterGameplayClockContainer = new MasterGameplayClockContainer(beatmapInfo, Clock),
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        new SpriteText
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Margin = new MarginPadding { Top = 34 },
                            Text = $"{currentBeatmap.Settings.Artist} - {currentBeatmap.Settings.Song}",
                            Font = FontUsage.Default.With(family: "OpenSans-Bold", size: 34),
                            Shadow = true,
                            ShadowColour = colours.TransparentBlack
                        },
                        complete = new SpriteText
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Alpha = 0,
                            Font = FontUsage.Default.With(family: "OpenSans-Bold", size: 64),
                            Shadow = true,
                            ShadowColour = colours.TransparentBlack
                        },
                        bar = new GameProgressBar(20, 10)
                        {
                            Anchor = Anchor.BottomLeft,
                            Origin = Anchor.BottomLeft,
                            StartFloor = 0,
                            EndFloor = hitTimes.Count,
                            Duration = 200
                        },
                        progress = new SpriteText
                        {
                            Anchor = Anchor.BottomCentre,
                            Origin = Anchor.BottomCentre,
                            Margin = new MarginPadding { Bottom = 20 },
                            Text = "0%",
                            Shadow = true,
                            Font = FontUsage.Default.With(family: "OpenSans-Bold", size: 32),
                            ShadowColour = colours.TransparentBlack
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

            localConfig.SetValue(CircleSetting.Parallax, false);
            musicController.CurrentTrack.VolumeTo(0, 500, Easing.Out)
                           .Then()
                           .Schedule(() =>
                           {
                               musicController.Stop();
                               musicController.ChangeTrack(beatmapInfo);
                               musicController.SeekTo(currentBeatmap.Settings.Offset);
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

            if (playState != GamePlayState.Complete)
            {
                if (masterGameplayClockContainer.CurrentTime >= endTime)
                {
                    playState = GamePlayState.Complete;
                    dialog.BlockInputAction = false;
                    complete.Text = "Congratulations!";
                    complete.Alpha = 1;
                    bar.CurrentFloor = ++floor;
                    percent = (float)floor / hitTimes.Count;
                    progress.ScaleTo(1.1f).ScaleTo(1, 500, Easing.OutQuint);
                    return;
                }

                if (masterGameplayClockContainer.CurrentTime >= hitTimes[floor])
                {
                    floor++;
                    bar.CurrentFloor = floor;
                    percent = (float)floor / hitTimes.Count;
                    progress.ScaleTo(1.1f).ScaleTo(1, 500, Easing.OutQuint);
                }
            }

            progress.Text = $"{percent * 100:0.#}%";
        }

        private void updateState()
        {
            switch (playState)
            {
                case GamePlayState.Ready:
                    masterGameplayClockContainer.Start();
                    musicController.CurrentTrack.VolumeTo(1);
                    Scheduler.AddDelayed(() => musicController.Play(), 60000 / currentBeatmap.Settings.Bpm);
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
                musicController.CurrentTrack.VolumeTo(1, 200, Easing.Out);
                musicController.Play();
            });

            base.OnExit();
        }

        public override bool OnExiting(IScreen next)
        {
            this.FadeOut(1000, Easing.OutPow10);
            localConfig.SetValue(CircleSetting.Parallax, parallaxEnabled);

            return base.OnExiting(next);
        }

        public override void OnResuming(IScreen last)
        {
            background.ChangeTexture(textureSource, textureName, 1000, Easing.OutPow10);
            onPaused();

            base.OnResuming(last);
        }

        private void onPaused()
        {
            masterGameplayClockContainer.Stop();
            musicController.CurrentTrack.VolumeTo(0, 750, Easing.OutPow10).Then().Schedule(musicController.Stop);
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
