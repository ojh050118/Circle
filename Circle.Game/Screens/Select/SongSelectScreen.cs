using System;
using Circle.Game.Beatmaps;
using Circle.Game.Graphics.UserInterface;
using Circle.Game.Input;
using Circle.Game.Overlays;
using Circle.Game.Screens.Play;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osu.Framework.Screens;

namespace Circle.Game.Screens.Select
{
    public class SongSelectScreen : CircleScreen
    {
        public override string Header => "Play";

        private BeatmapCarousel carousel;
        private BeatmapDetails details;

        [Resolved]
        private Background background { get; set; }

        [Resolved]
        private BeatmapStorage beatmaps { get; set; }

        [Resolved]
        private BeatmapManager beatmapManager { get; set; }

        [Resolved]
        private MusicController music { get; set; }

        [BackgroundDependencyLoader]
        private void load()
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
                    Origin = Anchor.CentreRight,
                }
            };

            if (beatmapManager.LoadedBeatmaps == null)
                beatmapManager.ReloadBeatmaps();

            foreach (var bi in beatmapManager.LoadedBeatmaps)
                carousel.AddItem(bi, () => this.Push(new PlayerLoader()));

            carousel.SelectedItem.ValueChanged += info => beatmapManager.CurrentBeatmap = info.NewValue.BeatmapInfo;
            beatmapManager.OnBeatmapChanged += beatmapChanged;
            checkIsLoadedCarousel();
        }

        private void checkIsLoadedCarousel()
        {
            if (carousel.LoadState != LoadState.Loaded)
            {
                Schedule(checkIsLoadedCarousel);
                return;
            }

            setBeatmap();
        }

        public override void OnEntering(IScreen last)
        {
            base.OnEntering(last);

            details.RotateTo(-45).Then().RotateTo(0, 1000, Easing.OutPow10);
            details.MoveToY(500).Then().MoveToY(0, 1000, Easing.OutPow10);
        }

        private void setBeatmap()
        {
            if (carousel.Count == 0)
                return;

            BeatmapInfo beatmapInfo;

            if (beatmapManager.CurrentBeatmap == null)
            {
                var idx = new Random().Next(0, beatmapManager.LoadedBeatmaps.Count);
                beatmapInfo = beatmapManager.LoadedBeatmaps[idx];
            }
            else
                beatmapInfo = beatmapManager.CurrentBeatmap;

            beatmapChanged((beatmapManager.CurrentBeatmap, beatmapInfo));
        }

        private void beatmapChanged((BeatmapInfo oldBeatmap, BeatmapInfo newBeatmap) beatmap)
        {
            details.ChangeBeatmap(beatmap.newBeatmap);
            if (!beatmaps.Storage.Exists(beatmap.newBeatmap.RelativeBackgroundPath))
                background.ChangeTexture(TextureSource.Internal, "bg1", 500, Easing.Out);
            else
                background.ChangeTexture(TextureSource.External, beatmap.newBeatmap.RelativeBackgroundPath, 500, Easing.Out);

            carousel.SelectBeatmap(beatmap.newBeatmap);

            if (!BeatmapUtils.Compare(beatmap.oldBeatmap, beatmap.newBeatmap))
            {
                music.ChangeTrack(beatmapManager.CurrentBeatmap);
                music.SeekTo(beatmapManager.CurrentBeatmap.Beatmap.Settings.PreviewSongStart * 1000);
                music.Play();
            }
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

                case InputAction.Select:
                    if (beatmapManager.CurrentBeatmap != null)
                        this.Push(new PlayerLoader());
                    break;
            }

            return base.OnPressed(e);
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
            details.MoveToX(0, 500, Easing.OutPow10);
        }

        protected override void Dispose(bool isDisposing)
        {
            beatmapManager.OnBeatmapChanged -= beatmapChanged;

            base.Dispose(isDisposing);
        }
    }
}
