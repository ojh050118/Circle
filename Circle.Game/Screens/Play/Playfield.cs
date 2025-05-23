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
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Video;

namespace Circle.Game.Screens.Play
{
    public partial class Playfield : PostProcessingContainer
    {
        private readonly Beatmap currentBeatmap;

        private CameraContainer cameraContainer;
        private PlanetContainer planetContainer;
        private ObjectContainer tileContainer;

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
        }

        protected override void LoadComplete()
        {
            tileContainer.AddTileTransforms();
            planetContainer.AddPlanetTransform();
            cameraContainer.AddCameraTransforms();
            addFilterTransforms();

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
    }
}
