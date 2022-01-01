﻿using Circle.Game.Beatmap;
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
using osu.Framework.Graphics.UserInterface;
using osuTK;
using Circle.Game.Graphics.UserInterface;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Shapes;

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

        private Playfield playfield;

        public Player()
        {
            InternalChildren = new Drawable[]
            {
                playfield = new Playfield(),
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
                    playfield.StartPlaying();
                    musicController.CurrentTrack.VolumeTo(1);
                    musicController.Play();
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
    }

    public enum PlanetState
    {
        Fire,
        Ice
    }
}
