#nullable disable

using System;
using System.Diagnostics;
using Circle.Game.Beatmaps;
using Circle.Game.Graphics.UserInterface;
using Circle.Game.Input;
using Circle.Game.Overlays;
using Circle.Game.Screens.Play;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Framework.Screens;

namespace Circle.Game.Screens.Select
{
    public partial class SongSelectScreen : CircleScreen
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

        #region Disposal

        protected override void Dispose(bool isDisposing)
        {
            beatmapManager.OnBeatmapChanged -= beatmapChanged;

            base.Dispose(isDisposing);
        }

        #endregion

        public override void OnEntering(ScreenTransitionEvent e)
        {
            base.OnEntering(e);

            details.RotateTo(-45).Then().RotateTo(0, 1000, Easing.OutPow10);
            details.MoveToY(500).Then().MoveToY(0, 1000, Easing.OutPow10);
        }

        public override bool OnPressed(KeyBindingPressEvent<InputAction> e)
        {
            switch (e.Action)
            {
                case InputAction.NextBeatmap:
                    carousel?.SelectNext();
                    break;

                case InputAction.PreviousBeatmap:
                    carousel?.SelectPrevious();
                    break;

                case InputAction.Select:
                    if (beatmapManager.CurrentBeatmap != null)
                        this.Push(new PlayerLoader(carousel.SelectedItem.Value.BeatmapInfo));
                    break;
            }

            return base.OnPressed(e);
        }

        public override void OnSuspending(ScreenTransitionEvent e)
        {
            base.OnSuspending(e);

            carousel.FadeOut(500, Easing.OutPow10);
            details.MoveToX(-500, 500, Easing.OutPow10);
        }

        public override void OnResuming(ScreenTransitionEvent e)
        {
            base.OnResuming(e);

            carousel.FadeIn(1000, Easing.OutPow10);
            details.MoveToX(0, 500, Easing.OutPow10);
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren = new Drawable[]
            {
                new ScreenHeader(this),
                new GridContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Content = new[]
                    {
                        new Drawable[]
                        {
                            details = new BeatmapDetails
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Padding = new MarginPadding { Left = 80, Top = 130, Bottom = 65 },
                            },
                            carousel = new BeatmapCarousel
                            {
                                Anchor = Anchor.CentreRight,
                                Origin = Anchor.CentreRight,
                                Padding = new MarginPadding { Left = 60 },
                                Depth = 1
                            }
                        }
                    }
                },
            };

            if (beatmapManager.LoadedBeatmaps == null)
                beatmapManager.ReloadBeatmaps();

            Debug.Assert(beatmapManager.LoadedBeatmaps != null);

            foreach (var bi in beatmapManager.LoadedBeatmaps)
                carousel.Add(bi, () => this.Push(new PlayerLoader(carousel.SelectedItem.Value.BeatmapInfo)));

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

        private void setBeatmap()
        {
            if (carousel.ItemCount == 0)
                return;

            BeatmapInfo beatmapInfo;

            if (beatmapManager.CurrentBeatmap == null)
            {
                int idx = new Random().Next(0, beatmapManager.LoadedBeatmaps.Count);
                beatmapInfo = beatmapManager.LoadedBeatmaps[idx];
            }
            else
                beatmapInfo = beatmapManager.CurrentBeatmap;

            beatmapChanged((beatmapManager.CurrentBeatmap, beatmapInfo));
        }

        private void beatmapChanged((BeatmapInfo oldBeatmap, BeatmapInfo newBeatmap) beatmap)
        {
            details.ChangeBeatmap(beatmap.newBeatmap);
            if (!beatmaps.Storage.Exists(beatmap.newBeatmap.RelativeBackgroundPath ?? string.Empty))
                background.ChangeTexture(TextureSource.Internal, "bg1", 500, Easing.Out);
            else
                background.ChangeTexture(TextureSource.External, beatmap.newBeatmap.RelativeBackgroundPath, 500, Easing.Out);

            carousel.Select(beatmap.newBeatmap);

            if (!BeatmapUtils.Compare(beatmap.oldBeatmap, beatmap.newBeatmap))
            {
                music.ChangeTrack(beatmapManager.CurrentBeatmap);
                music.SeekTo(beatmapManager.CurrentBeatmap.Beatmap.Settings.PreviewSongStart * 1000);
                music.Play();
            }
        }
    }
}
