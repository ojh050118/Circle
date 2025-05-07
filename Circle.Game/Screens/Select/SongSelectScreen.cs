#nullable disable

using System.Collections.Generic;
using System.Linq;
using Circle.Game.Beatmaps;
using Circle.Game.Graphics.UserInterface;
using Circle.Game.Input;
using Circle.Game.Overlays;
using Circle.Game.Screens.Play;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osu.Framework.Utils;

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
        private Bindable<WorkingBeatmap> workingBeatmap { get; set; }

        [Resolved]
        private BeatmapManager beatmapManager { get; set; }

        [Resolved]
        private MusicController music { get; set; }

        private IEnumerable<BeatmapInfo> availableBeatmaps => beatmapManager.GetAvailableBeatmaps();

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

            var beatmaps = beatmapManager.GetAvailableBeatmaps();

            foreach (var bi in beatmaps)
                carousel.Add(bi, () => this.Push(new PlayerLoader(carousel.SelectedItem.Value.BeatmapInfo)));

            workingBeatmap.ValueChanged += workingBeatmapChanged;

            carousel.SelectedItem.ValueChanged += info => workingBeatmap.Value = beatmapManager.GetWorkingBeatmap(info.NewValue.BeatmapInfo);

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

        private void workingBeatmapChanged(ValueChangedEvent<WorkingBeatmap> beatmap)
        {
            details.ChangeBeatmap(beatmap.NewValue.BeatmapInfo);

            if (workingBeatmap.Value.GetBackground() == null)
                background.ChangeTexture(TextureSource.Internal, "bg1", null, 500, Easing.Out);
            else
                background.ChangeTexture(TextureSource.External, string.Empty, beatmap.NewValue.BeatmapInfo, 500, Easing.Out);

            carousel.Select(beatmap.NewValue.BeatmapInfo);

            if (!BeatmapUtils.Compare(beatmap.OldValue.BeatmapInfo, beatmap.NewValue.BeatmapInfo))
            {
                music.ChangeTrack(workingBeatmap.Value.BeatmapInfo);
                music.SeekTo(workingBeatmap.Value.Metadata.PreviewSongStart * 1000);
                music.Play();
            }
        }

        private void setBeatmap()
        {
            if (carousel.ItemCount == 0)
                return;

            if (workingBeatmap.Value is DummyWorkingBeatmap)
            {
                int idx = RNG.Next(0, availableBeatmaps.Count());
                workingBeatmap.Value = beatmapManager.GetWorkingBeatmap(availableBeatmaps.ElementAt(idx));
            }
            else
            {
                workingBeatmap.TriggerChange();
            }
        }
    }
}
