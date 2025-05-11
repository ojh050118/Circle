#nullable disable

using System.Collections.Generic;
using System.Linq;
using Circle.Game.Beatmaps;
using Circle.Game.Configuration;
using Circle.Game.Graphics;
using Circle.Game.Graphics.UserInterface;
using Circle.Game.Overlays;
using Circle.Game.Rulesets.Objects;
using Circle.Game.Screens.Play.HUD;
using Circle.Game.Screens.Setting;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
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
    public partial class Player : CircleScreen
    {
        public override bool FadeBackground => false;

        private readonly WorkingBeatmap currentBeatmap;

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

        private float beat => 60000 / currentBeatmap.Metadata.Bpm;
        private int tick => currentBeatmap.Metadata.CountdownTicks;
        private double gameTime => masterGameplayClockContainer.CurrentTime;

        /// <summary>
        /// 타일 시작 시간과 게임플레이 시간을 비교하기 위해 존재합니다.
        /// 게임 시작 전에는 0%이어야 하므로 1번 타일과 비교합니다.
        /// </summary>
        private int floor = 1;

        private GameplayMusicController gameMusic;
        private IEnumerator<Tile> tiles;
        private HUDOverlay hud;

        private MasterGameplayClockContainer masterGameplayClockContainer;

        private bool parallaxEnabled;

        private GamePlayState playState = GamePlayState.NotPlaying;

        private Sample sampleHit;

        private ScheduledDelegate scheduledDelegate;

        private string textureName;

        private TextureSource textureSource;

        [Resolved]
        private MusicController musicController { get; set; }

        [Resolved]
        private DialogOverlay dialog { get; set; }

        [Resolved]
        private Background background { get; set; }

        [Resolved]
        private CircleConfigManager localConfig { get; set; }

        public Player(WorkingBeatmap workingBeatmap)
        {
            currentBeatmap = workingBeatmap;
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
                musicController.SeekTo(gameTime - 1000);
                musicController.CurrentTrack.VolumeTo(1, 200, Easing.Out);
                musicController.Play();
            });

            base.OnExit();
        }

        public override bool OnExiting(ScreenExitEvent e)
        {
            this.FadeOut(1000, Easing.OutPow10);
            localConfig.SetValue(CircleSetting.Parallax, parallaxEnabled);

            return base.OnExiting(e);
        }

        public override void OnResuming(ScreenTransitionEvent e)
        {
            background.ChangeTexture(textureSource, textureName, currentBeatmap.BeatmapInfo, 1000, Easing.OutPow10);
            onPaused();

            base.OnResuming(e);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            tiles.MoveNext();
            tiles.MoveNext();
            localConfig.SetValue(CircleSetting.Parallax, false);
            gameMusic.SetOffset(gameTime, beat * tick);
            musicController.CurrentTrack.VolumeTo(0, 500, Easing.Out)
                           .Then()
                           .Schedule(() =>
                           {
                               musicController.Stop();
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

            if (playState == GamePlayState.Complete)
                return;

            if (masterGameplayClockContainer.CurrentTime >= tiles.Current!.HitTime)
            {
                if (tiles.MoveNext())
                {
                    hud.UpdateProgress(floor);
                    floor++;
                }
                else
                {
                    playState = GamePlayState.Complete;
                    dialog.BlockInputAction = false;
                    hud.Complete();
                }

                sampleHit?.Play();
            }
        }

        [BackgroundDependencyLoader]
        private void load(GameHost host, AudioManager audio, CircleColour colours, BeatmapManager beatmapManager)
        {
            parallaxEnabled = localConfig.Get<bool>(CircleSetting.Parallax);
            textureSource = background.TextureSource;
            textureName = background.TextureName;
            tiles = currentBeatmap.Beatmap.Tiles.GetEnumerator();
            sampleHit = audio.Samples.Get("normal-hitnormal.wav");

            gameMusic = new GameplayMusicController(currentBeatmap.BeatmapInfo);

            InternalChildren = new Drawable[]
            {
                masterGameplayClockContainer = new MasterGameplayClockContainer(currentBeatmap.BeatmapInfo, currentBeatmap.Metadata.Offset, beat * tick, currentBeatmap.Track),
                hud = new HUDOverlay(currentBeatmap.Beatmap),
                gameMusic
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

        private void updateState()
        {
            switch (playState)
            {
                case GamePlayState.Ready:
                    masterGameplayClockContainer.Start();
                    gameMusic.Start();
                    hud.Start();
                    playState = GamePlayState.Playing;
                    break;

                case GamePlayState.Playing:
                    break;

                case GamePlayState.Complete:
                    OnExit();
                    break;
            }
        }

        private void onPaused()
        {
            masterGameplayClockContainer.Stop();
            gameMusic.Pause();
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
            scheduledDelegate?.Cancel();

            gameMusic.DelayUntilTransformsFinished().Schedule(() =>
            {
                float countdown = beat * tick;

                if (gameTime - countdown * 2 >= 0)
                {
                    gameMusic.SetOffset(gameTime, countdown * 2);
                    gameMusic.Resume();
                    hud.Countdown(countdown);
                }
                else
                {
                    gameMusic.SetOffset(gameTime, countdown);
                    gameMusic.Resume(countdown);
                    hud.Countdown(gameMusic.TimeUntilPlay + countdown);
                }

                scheduledDelegate = Scheduler.AddDelayed(() =>
                {
                    playState = GamePlayState.Playing;
                    masterGameplayClockContainer.Start();
                }, countdown);
            });
        }
    }
}
