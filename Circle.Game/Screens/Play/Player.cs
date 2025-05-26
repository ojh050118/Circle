#nullable disable

using System.Collections.Generic;
using System.Linq;
using Circle.Game.Beatmaps;
using Circle.Game.Graphics.UserInterface;
using Circle.Game.Overlays;
using Circle.Game.Screens.Setting;
using osu.Framework.Allocation;
using osu.Framework.Audio;
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

        private float beat => 60000 / currentBeatmap.Metadata.Bpm;
        private int tick => currentBeatmap.Metadata.CountdownTicks;
        private double gameplayTime => masterGameplayClockContainer.CurrentTime;

        private readonly IEnumerable<Key> allowedKeys;

        private GameplayMusicController gameMusic;

        private MasterGameplayClockContainer masterGameplayClockContainer;

        private ScheduledDelegate scheduledDelegate;

        private string textureName;

        private TextureSource textureSource;

        [Resolved]
        private MusicController musicController { get; set; }

        [Resolved]
        private DialogOverlay dialog { get; set; }

        [Resolved]
        private Background background { get; set; }

        public Player(WorkingBeatmap workingBeatmap)
        {
            currentBeatmap = workingBeatmap;

            List<Key> keys = new List<Key> { Key.Up, Key.Down, Key.Left, Key.Right, Key.Space };

            // 특수키를 제외한 모든 키를 추가합니다.
            for (int i = 67; i <= 130; i++)
                keys.Add((Key)i);

            allowedKeys = keys;
        }

        [BackgroundDependencyLoader]
        private void load(GameHost host, AudioManager audio)
        {
            textureSource = background.TextureSource;
            textureName = background.TextureName;

            InternalChildren = new Drawable[]
            {
                masterGameplayClockContainer = new MasterGameplayClockContainer(currentBeatmap, currentBeatmap.Metadata.Offset, currentBeatmap.Track),
                gameMusic = new GameplayMusicController(currentBeatmap.BeatmapInfo)
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
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            gameMusic.SetOffset(currentBeatmap.Metadata.Offset, beat * tick);
            musicController.CurrentTrack.VolumeTo(0, 500, Easing.Out)
                           .Then()
                           .Schedule(() =>
                           {
                               musicController.Stop();
                           });
        }

        protected override void Update()
        {
            base.Update();

            if (masterGameplayClockContainer.State == GameplayState.Complete)
            {
                dialog.BlockInputAction = false;
            }
        }

        public override void OnExit()
        {
            if (masterGameplayClockContainer.State == GameplayState.Playing || masterGameplayClockContainer.State == GameplayState.Ready)
            {
                onPaused();
                return;
            }

            musicController.CurrentTrack.DelayUntilTransformsFinished().Schedule(() =>
            {
                musicController.SeekTo(gameplayTime + tick * beat - 1000);
                musicController.CurrentTrack.VolumeTo(1, 200, Easing.Out);
                musicController.Play();
            });

            base.OnExit();
        }

        public override bool OnExiting(ScreenExitEvent e)
        {
            this.FadeOut(1000, Easing.OutPow10);

            return base.OnExiting(e);
        }

        public override void OnResuming(ScreenTransitionEvent e)
        {
            background.ChangeTexture(textureSource, textureName, currentBeatmap.BeatmapInfo, 1000, Easing.OutPow10);
            onPaused();

            base.OnResuming(e);
        }

        protected override bool OnClick(ClickEvent e)
        {
            updateState();

            return base.OnClick(e);
        }

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            if (!allowedKeys.Contains(e.Key))
                return false;

            updateState();

            return base.OnKeyDown(e);
        }

        private void updateState()
        {
            switch (masterGameplayClockContainer.State)
            {
                case GameplayState.NotPlaying:
                case GameplayState.Ready:
                    masterGameplayClockContainer.Start();
                    gameMusic.Start();
                    break;

                case GameplayState.Playing:
                    break;

                case GameplayState.Fail:
                    // TODO: 플레이어를 다시 로드하던지 해서 재도전을 할 수 있도록 해야함
                    break;

                case GameplayState.Complete:
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

                if (gameplayTime - countdown >= 0)
                {
                    gameMusic.SetOffset(gameplayTime - countdown, countdown);
                    gameMusic.Resume();
                }
                else
                {
                    gameMusic.SetOffset(gameplayTime - countdown, countdown);
                    gameMusic.Resume(countdown);
                }

                scheduledDelegate = masterGameplayClockContainer.Resume(countdown);
            });
        }
    }
}
