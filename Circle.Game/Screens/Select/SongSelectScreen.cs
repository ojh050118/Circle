﻿using Circle.Game.Input;
using Circle.Game.Screens.Play;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osu.Framework.Screens;

namespace Circle.Game.Screens.Select
{
    public class SongSelectScreen : CircleScreen
    {
        public override string Header => "Play";

        private readonly BeatmapCarousel carousel;
        private readonly BeatmapDetails details;

        public SongSelectScreen()
        {
            InternalChildren = new Drawable[]
            {
                new ScreenHeader(this),
                details = new BeatmapDetails
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Padding = new MarginPadding { Top = 130, Bottom = 65 },
                    Margin = new MarginPadding { Left = 80 },
                    Width = 0.5f
                },
                carousel = new BeatmapCarousel
                {
                    Width = 0.4f,
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            carousel.PlayRequested.ValueChanged += v =>
            {
                if (v.NewValue)
                    this.Push(new PlayerLoader());
            };
            carousel.SelectedBeatmap.ValueChanged += beatmap => details.ChangeBeatmap(beatmap.NewValue);
        }

        public override bool OnPressed(KeyBindingPressEvent<InputAction> e)
        {
            switch (e.Action)
            {
                case InputAction.NextBeatmap:
                    carousel?.SelectBeatmap(VerticalDirection.Down);
                    break;

                case InputAction.PreviousBeatmap:
                    carousel?.SelectBeatmap(VerticalDirection.Up);
                    break;
            }

            return base.OnPressed(e);
        }

        public override void OnEntering(IScreen last)
        {
            base.OnEntering(last);

            details.RotateTo(-45).Then().RotateTo(0, 1000, Easing.OutPow10);
            details.MoveToY(500).Then().MoveToY(0, 1000, Easing.OutPow10);
        }

        public override void OnSuspending(IScreen next)
        {
            base.OnSuspending(next);

            carousel.FadeOut(500, Easing.OutPow10);
            details.MoveToX(-500, 500, Easing.OutPow10);
        }

        public override void OnResuming(IScreen last)
        {
            base.OnResuming(last);

            carousel.FadeIn(1000, Easing.OutPow10);
            carousel.PlayRequested.Value = false;
            details.MoveToX(0, 500, Easing.OutPow10);
        }
    }
}
