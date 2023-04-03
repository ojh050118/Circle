using System;
using System.Collections.Generic;
using System.Linq;
using Circle.Game.Beatmaps;
using Circle.Game.Configuration;
using Circle.Game.Rulesets;
using Circle.Game.Rulesets.Extensions;
using Circle.Game.Rulesets.Objects;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;
using osuTK;

namespace Circle.Game.Screens.Play
{
    public class Playfield : Container
    {
        /// <summary>
        /// 카운트다운 지속시간.
        /// </summary>
        private readonly double countdownDuration;

        private readonly Beatmap currentBeatmap;

        /// <summary>
        /// 준비 시간을 포함한 게임이 시작하는 시간.
        /// </summary>
        private readonly double gameplayStartTime;

        private Container cameraContainer;
        private PlanetContainer planetContainer;
        private ObjectContainer tileContainer;

        private TileInfo[] tilesInfo;

        public Playfield(Beatmap beatmap, double gameplayStartTime, double countdownDuration)
        {
            currentBeatmap = beatmap;
            this.gameplayStartTime = gameplayStartTime;
            this.countdownDuration = countdownDuration;
        }

        public Action OnComplete { get; set; }

        private IReadOnlyList<double> startTimes => CalculationExtensions.GetTileStartTime(currentBeatmap, gameplayStartTime, countdownDuration);

        [BackgroundDependencyLoader]
        private void load(CircleConfigManager config)
        {
            RelativeSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Child = cameraContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Child = new Container
                {
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Children = new Drawable[]
                    {
                        tileContainer = new ObjectContainer(currentBeatmap),
                        planetContainer = new PlanetContainer()
                    }
                }
            };

            tilesInfo = tileContainer.GetTilesInfo().ToArray();

            planetContainer.RedPlanet.Expansion = planetContainer.BluePlanet.Expansion = 0;
            planetContainer.BluePlanet.Rotation = tilesInfo[0].Angle - CalculationExtensions.GetTimebaseRotation(gameplayStartTime, startTimes[0], currentBeatmap.Settings.Bpm);

            cameraContainer.Rotation = currentBeatmap.Settings.Rotation;
            var zoom = Precision.AlmostEquals(currentBeatmap.Settings.Zoom, 0) ? 100 : currentBeatmap.Settings.Zoom;
            cameraContainer.Scale = new Vector2(1 / (zoom / 100));
        }

        protected override void LoadComplete()
        {
            tileContainer.AddTileTransforms(gameplayStartTime, countdownDuration);
            planetContainer.AddPlanetTransform(currentBeatmap, gameplayStartTime);
            addCameraTransforms();

            base.LoadComplete();
        }

        private void addCameraTransforms()
        {
            float bpm = currentBeatmap.Settings.Bpm;
            var offset = startTimes;
            var cameraTransforms = new List<CameraTransform>();

            for (int floor = 0; floor < tilesInfo.Length; floor++)
            {
                bpm = getNewBpm(bpm, floor);
                var prevAngle = tilesInfo[floor].Angle;
                var fixedRotation = computeRotation(floor, prevAngle);

                // Camera move (relative to player)
                using (cameraContainer.Child.BeginAbsoluteSequence(offset[floor], false))
                    cameraContainer.Child.MoveTo(-tilesInfo[floor].Position, 400 + 60 / bpm * 500, Easing.OutSine);

                // 한 타일의 액션에 접근
                for (int actionIndex = 0; actionIndex < tilesInfo[floor].Action.Length; actionIndex++)
                {
                    var action = tilesInfo[floor].Action[actionIndex];

                    switch (action.EventType)
                    {
                        case EventType.MoveCamera:
                            var angleOffset = CalculationExtensions.GetRelativeDuration(fixedRotation, fixedRotation + action.AngleOffset, bpm);
                            cameraTransforms.Add(new CameraTransform
                            {
                                Action = action,
                                StartTime = offset[floor] + angleOffset,
                                Duration = getRelativeDuration(fixedRotation, tilesInfo[floor].TileType == TileType.Midspin ? floor + 1 : floor, bpm) * action.Duration
                            });
                            break;

                        // 이벤트 반복은 원래 이벤트를 포함해 반복하지 않습니다. (ex: 반복횟수가 1이면 이벤트는 총 2번 실행됨)
                        case EventType.RepeatEvents:
                            var cameraEvents = Array.FindAll(tilesInfo[action.Floor].Action, a => a.EventType == EventType.MoveCamera);
                            var intervalBeat = 60000 / bpm * action.Interval;

                            for (int i = 1; i <= action.Repetitions; i++)
                            {
                                var startTime = offset[floor] + intervalBeat * i;

                                foreach (var cameraEvent in cameraEvents)
                                {
                                    var angleTimeOffset = CalculationExtensions.GetRelativeDuration(fixedRotation, fixedRotation + cameraEvent.AngleOffset, bpm);

                                    if (cameraEvent.EventTag == action.Tag)
                                    {
                                        cameraTransforms.Add(new CameraTransform
                                        {
                                            Action = cameraEvent,
                                            StartTime = startTime + angleTimeOffset,
                                            Duration = getRelativeDuration(fixedRotation, tilesInfo[floor].TileType == TileType.Midspin ? floor + 1 : floor, bpm) * cameraEvent.Duration
                                        });
                                    }
                                }
                            }

                            break;
                    }
                }
            }

            // 트랜스폼을 시작 시간순으로 추가해야 올바르게 추가됩니다.
            foreach (var cameraTransform in cameraTransforms.OrderBy(a => a.StartTime))
            {
                var action = cameraTransform.Action;

                using (cameraContainer.BeginAbsoluteSequence(cameraTransform.StartTime, false))
                {
                    if (action.Zoom.HasValue)
                    {
                        float cameraZoom = 1 / ((float)action.Zoom.Value / 100);

                        if (float.IsInfinity(cameraZoom))
                            cameraZoom = 0;

                        cameraContainer.ScaleTo(cameraZoom, cameraTransform.Duration, action.Ease);
                    }

                    if (action.Rotation.HasValue)
                        cameraContainer.RotateTo(action.Rotation.Value, cameraTransform.Duration, action.Ease);
                }
            }

            //cameraContainer.AddCameraTransforms(currentBeatmap.Settings, tilesInfo, ElementTransformExtensions.GenerateCameraTransforms(currentBeatmap.Settings, startTimes, tilesInfo).ToList());
        }

        private float computeRotation(int floor, float prevAngle) => CalculationExtensions.ComputeRotation(tilesInfo, floor, prevAngle);

        private float getRelativeDuration(float oldRotation, int floor, float bpm) => CalculationExtensions.GetRelativeDuration(oldRotation, tilesInfo[floor].Angle, bpm);

        private float getNewBpm(float current, int floor) => CalculationExtensions.GetNewBpm(tilesInfo, current, floor);
    }
}
