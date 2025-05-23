#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using Circle.Game.Beatmaps;
using Circle.Game.Graphics.UserInterface;
using Circle.Game.Rulesets;
using Circle.Game.Rulesets.Extensions;
using Circle.Game.Rulesets.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Video;

namespace Circle.Game.Screens.Play
{
    public partial class Playfield : PostProcessingContainer
    {
        private readonly Beatmap currentBeatmap;

        private CameraContainer cameraContainer;
        private PlanetContainer planetContainer;
        private ObjectContainer tileContainer;

        private Box flashBackground;
        private Box flashForeground;

        public Playfield(Beatmap beatmap)
        {
            currentBeatmap = beatmap;
        }

        public Action OnComplete { get; set; }

        [BackgroundDependencyLoader]
        private void load(BeatmapManager beatmapManager)
        {
            Container backgrounds;

            RelativeSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Children = new Drawable[]
            {
                backgrounds = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Children = new Drawable[]
                    {
                        new Background(TextureSource.External, beatmapInfo: currentBeatmap.BeatmapInfo),
                    }
                },
                cameraContainer = new CameraContainer(currentBeatmap)
                {
                    Content = new Container
                    {
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Children = new Drawable[]
                        {
                            tileContainer = new ObjectContainer(currentBeatmap),
                            planetContainer = new PlanetContainer(currentBeatmap)
                        }
                    }
                },
                flashForeground = new Box
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0
                }
            };

            var stream = beatmapManager.GetWorkingBeatmap(currentBeatmap.BeatmapInfo).GetVideo();

            if (stream != null)
            {
                backgrounds.Add(new Video(stream, false)
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    FillMode = FillMode.Fill,
                    RelativeSizeAxes = Axes.Both,
                    Loop = false,
                    PlaybackPosition = currentBeatmap.Metadata.VidOffset - currentBeatmap.Metadata.Offset - 60000 / currentBeatmap.Metadata.Bpm * currentBeatmap.Metadata.CountdownTicks,
                });
            }

            backgrounds.Add(flashBackground = new Box
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Alpha = 0
            });
        }

        protected override void LoadComplete()
        {
            tileContainer.AddTileTransforms();
            planetContainer.AddPlanetTransform();
            cameraContainer.AddCameraTransforms();
            addFilterTransforms();
            addFlashTransforms();

            base.LoadComplete();
        }

        private void addFilterTransforms()
        {
            var filterTransforms = new List<FilterTransform>();
            var tiles = currentBeatmap.Tiles.ToArray();

            #region Process filter transforms

            for (int floor = 0; floor < tiles.Length; floor++)
            {
                var tile = tiles[floor];
                float resolvedRotation = CalculationExtensions.InvertAngle(tile.Angle);

                foreach (var action in tile.Actions)
                {
                    float angleOffset = CalculationExtensions.GetRelativeDuration(resolvedRotation, resolvedRotation + action.AngleOffset, tile.Bpm);

                    switch (action.EventType)
                    {
                        case EventType.SetFilter:
                            filterTransforms.Add(new FilterTransform
                            {
                                StartTime = tile.HitTime + angleOffset,
                                Enabled = action.Enabled,
                                FilterType = action.Filter!.Value,
                                Intensity = action.Intensity,
                                Duration = tiles.GetRelativeDuration(resolvedRotation, floor, tile.Bpm) * action.Duration,
                                DisableOthers = action.DisableOthers,
                                Easing = action.Ease
                            });

                            break;

                        case EventType.RepeatEvents:
                            var filterEvents = Array.FindAll(tiles[action.Floor].Actions, a => a.EventType == EventType.SetFilter);
                            double intervalBeat = 60000 / tile.Bpm * action.Interval;

                            for (int i = 0; i < action.Repetitions; i++)
                            {
                                double startTime = tile.HitTime + intervalBeat * i;

                                foreach (var filter in filterEvents)
                                {
                                    if (action.Tag != filter.EventTag)
                                        continue;

                                    // 이벤트 반복 목표태그와 일치하는 모든 필터 설정 이벤트를 추가합니다.
                                    filterTransforms.Add(new FilterTransform
                                    {
                                        StartTime = startTime + angleOffset,
                                        Enabled = filter.Enabled,
                                        FilterType = filter.Filter!.Value,
                                        Intensity = filter.Intensity,
                                        Duration = tiles.GetRelativeDuration(resolvedRotation, floor, tile.Bpm) * filter.Duration,
                                        DisableOthers = filter.DisableOthers,
                                        Easing = filter.Ease
                                    });
                                }
                            }

                            break;
                    }
                }
            }

            #endregion

            #region Register filter transforms

            foreach (var filter in filterTransforms.OrderBy(a => a.StartTime))
            {
                using (BeginAbsoluteSequence(filter.StartTime, false))
                {
                    this.FilterToggleTo(filter.FilterType, filter.Enabled);

                    if (filter.DisableOthers)
                    {
                        foreach (var filterType in Enum.GetValues(typeof(FilterType)).Cast<FilterType>())
                        {
                            if (filterType == filter.FilterType) continue;

                            this.FilterToggleTo(filterType, false);
                        }
                    }

                    if (filter.Intensity.HasValue)
                        this.FilterTo(filter.FilterType, filter.Intensity.Value, filter.Duration, filter.Easing);
                }
            }

            #endregion
        }

        private void addFlashTransforms()
        {
            var flashTransforms = new List<FlashTransform>();
            var tiles = currentBeatmap.Tiles.ToArray();

            #region Process flash transforms

            for (int floor = 0; floor < tiles.Length; floor++)
            {
                var tile = tiles[floor];
                float resolvedRotation = CalculationExtensions.InvertAngle(tile.Angle);

                foreach (var action in tile.Actions)
                {
                    float angleOffset = CalculationExtensions.GetRelativeDuration(resolvedRotation, resolvedRotation + action.AngleOffset, tile.Bpm);

                    switch (action.EventType)
                    {
                        case EventType.Flash:
                            flashTransforms.Add(new FlashTransform
                            {
                                StartTime = tile.HitTime + angleOffset,
                                Plane = action.Plane!.Value,
                                StartColor = Color4Extensions.FromHex(action.StartColor!),
                                StartOpacity = action.StartOpacity!.Value,
                                EndColor = Color4Extensions.FromHex(action.EndColor!),
                                EndOpacity = action.EndOpacity!.Value,
                                Duration = tiles.GetRelativeDuration(resolvedRotation, floor, tile.Bpm) * action.Duration
                            });

                            break;

                        case EventType.RepeatEvents:
                            var flashEvents = Array.FindAll(tiles[action.Floor].Actions, a => a.EventType == EventType.Flash);
                            double intervalBeat = 60000 / tile.Bpm * action.Interval;

                            for (int i = 0; i < action.Repetitions; i++)
                            {
                                double startTime = tile.HitTime + intervalBeat * i;

                                foreach (var flash in flashEvents)
                                {
                                    if (action.Tag != flash.EventTag)
                                        continue;

                                    // 이벤트 반복 목표태그와 일치하는 모든 필터 설정 이벤트를 추가합니다.
                                    flashTransforms.Add(new FlashTransform
                                    {
                                        StartTime = startTime + angleOffset,
                                        Plane = flash.Plane!.Value,
                                        StartColor = Color4Extensions.FromHex(flash.StartColor!),
                                        StartOpacity = flash.StartOpacity!.Value,
                                        EndColor = Color4Extensions.FromHex(flash.EndColor!),
                                        EndOpacity = flash.EndOpacity!.Value,
                                        Duration = tiles.GetRelativeDuration(resolvedRotation, floor, tile.Bpm) * flash.Duration
                                    });
                                }
                            }

                            break;
                    }
                }
            }

            #endregion

            #region Register flash transforms

            foreach (var filter in flashTransforms.OrderBy(a => a.StartTime))
            {
                switch (filter.Plane)
                {
                    case FlashPlane.Background:
                        using (flashBackground.BeginAbsoluteSequence(filter.StartTime, false))
                        {
                            flashBackground.FadeColour(filter.StartColor).FadeColour(filter.EndColor, filter.Duration);
                            flashBackground.FadeTo(filter.StartOpacity / 100).FadeTo(filter.EndOpacity / 100, filter.Duration);
                        }

                        break;

                    case FlashPlane.Foreground:
                        using (flashForeground.BeginAbsoluteSequence(filter.StartTime, false))
                        {
                            flashForeground.FadeColour(filter.StartColor).FadeColour(filter.EndColor, filter.Duration);
                            flashForeground.FadeTo(filter.StartOpacity / 100).FadeTo(filter.EndOpacity / 100, filter.Duration);
                        }

                        break;
                }
            }

            #endregion
        }
    }
}
