using System;
using System.Collections.Generic;
using System.Linq;
using Circle.Game.Beatmaps;
using Circle.Game.Configuration;
using Circle.Game.Graphics;
using Circle.Game.Graphics.UserInterface;
using Circle.Game.Overlays;
using Circle.Game.Rulesets.Extensions;
using Circle.Game.Screens.Play.HUD;
using Circle.Game.Screens.Setting;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
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
        private GameplayProgress progress;

        private ScheduledDelegate scheduledDelegate;

        private readonly BeatmapInfo beatmapInfo;
        private readonly Beatmap currentBeatmap;

        private bool parallaxEnabled;
        private double endTime;
        private List<double> hitTimes;
        private float beat => 60000 / currentBeatmap.Settings.Bpm;

        /// <summary>
        /// 타일 시작 시간과 게임플레이 시간을 비교하기 위해 존재합니다.
        /// 게임 시작 전에는 0%이어야 하므로 1번 타일과 비교합니다.
        /// </summary>
        private int floor = 1;

        private Sample sampleHit;

        public Player(BeatmapInfo beatmapInfo)
        {
            this.beatmapInfo = beatmapInfo;
            currentBeatmap = beatmapInfo.Beatmap;
        }

        [BackgroundDependencyLoader]
        private void load(GameHost host, AudioManager audio, CircleColour colours)
        {
            parallaxEnabled = localConfig.Get<bool>(CircleSetting.Parallax);
            textureSource = background.TextureSource;
            textureName = background.TextureName;
            hitTimes = CalculationExtensions.GetTileStartTime(currentBeatmap, currentBeatmap.Settings.Offset, beat * 4).ToList();
            endTime = hitTimes.Last();
            sampleHit = audio.Samples.Get("normal-hitnormal.wav");
            InternalChildren = new Drawable[]
            {
                masterGameplayClockContainer = new MasterGameplayClockContainer(beatmapInfo, currentBeatmap.Settings.Offset, beat * 4, Clock),
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        new SpriteText
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Margin = new MarginPadding { Top = 30 },
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
                        progress = new GameplayProgress(hitTimes.Count - 1)
                        {
                            Anchor = Anchor.BottomCentre,
                            Origin = Anchor.BottomCentre,
                        },
                    }
                },
            };

            if (!host.CanExit)
            {
                AddInternal(new IconButton
                {
                    Anchor = Anchor.TopRight,
                    Origin = Anchor.TopRight,
                    Margin = new MarginPadding(30),
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
                               if (currentBeatmap.Settings.Offset - beat * 4 >= 0)
                                   musicController.SeekTo(currentBeatmap.Settings.Offset - beat * 4);
                               else
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
                    progress.Increase();
                    sampleHit?.Play();
                    return;
                }

                if (masterGameplayClockContainer.CurrentTime >= hitTimes[floor])
                {
                    progress.ProgressTo(floor);
                    floor++;
                    sampleHit?.Play();
                }
            }
        }

        private void updateState()
        {
            switch (playState)
            {
                case GamePlayState.Ready:
                    masterGameplayClockContainer.Start();
                    musicController.CurrentTrack.VolumeTo(1);
                    var timeUntilRun = currentBeatmap.Settings.Offset - beat * 4 >= 0 ? 0 : beat * 4;
                    Scheduler.AddDelayed(() => musicController.Play(), timeUntilRun);
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
                musicController.SeekTo(musicController.CurrentTrack.CurrentTime - 500);
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
                        resume();
                        dialog.Hide();
                    }
                }
            };
            dialog.Push();
        }

        private void resume()
        {
            musicController.CurrentTrack.DelayUntilTransformsFinished().Schedule(() =>
            {
                if (masterGameplayClockContainer.CurrentTime - 1000 - beat * 4 >= 0)
                {
                    musicController.SeekTo(masterGameplayClockContainer.CurrentTime - 1000 - beat * 4);
                    musicController.Play();
                    musicController.CurrentTrack.VolumeTo(1, 1000, Easing.OutPow10);
                }

                scheduledDelegate = Scheduler.AddDelayed(() =>
                {
                    if (masterGameplayClockContainer.CurrentTime - 1000 - beat * 4 < 0)
                    {
                        Scheduler.AddDelayed(() =>
                        {
                            musicController.SeekTo(masterGameplayClockContainer.CurrentTime - beat * 4);
                            musicController.CurrentTrack.VolumeTo(1);
                            musicController.Play();
                        }, Math.Abs(masterGameplayClockContainer.CurrentTime - beat * 4));
                    }

                    playState = GamePlayState.Playing;
                    masterGameplayClockContainer.Start();
                }, 1000);
            });
        }
    }

    public enum PlanetState
    {
        Fire,
        Ice
    }
}
