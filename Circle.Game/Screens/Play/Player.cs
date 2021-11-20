using Circle.Game.Beatmap;
using Circle.Game.Overlays;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osuTK.Input;
using System.Linq;
using osu.Framework.Audio;

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

        public Player()
        {
            InternalChildren = new Drawable[]
            {
                new ObjectContainer(),
            };
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
            musicController.Play();
        }
    }
}
