#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using Circle.Game.Beatmaps;
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
    public partial class CameraContainer : Container
    {
        public new Container Content;
        private Container offsetContainer, positionContainer, scalingContainer;

        private readonly Beatmap beatmap;

        public CameraContainer(Beatmap beatmap)
        {
            this.beatmap = beatmap;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Child = scalingContainer = new Container
            {
                Name = "Rotate/Scaling Container",
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Child = offsetContainer = new Container
                {
                    Name = "Offset Container",
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    AutoSizeAxes = Axes.Both,
                    Child = positionContainer = new Container
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Name = "Positioning Container",
                        AutoSizeAxes = Axes.Both,
                        Child = Content
                    }
                }
            };

            var metadata = beatmap.Metadata;

            scalingContainer.Rotation = metadata.Rotation;
            float zoom = Precision.AlmostEquals(metadata.Zoom, 0) ? 100 : metadata.Zoom;
            scalingContainer.Scale = new Vector2(1 / (zoom / 100));
            offsetContainer.Position = metadata.Position.ToVector2() * (DrawableTile.WIDTH - Planet.PLANET_SIZE);
        }

        public void AddCameraTransforms()
        {
            #region Initialize Camera Setting

            // 카메라 이동을 위한 전역 설정입니다.
            Relativity cameraRelativity = beatmap.Metadata.RelativeTo;
            Vector2 cameraOffset = beatmap.Metadata.Position.ToVector2() * (DrawableTile.WIDTH - Planet.PLANET_SIZE);
            Vector2 cameraPosition = cameraOffset;
            float cameraRotation = beatmap.Metadata.Rotation;

            var tiles = beatmap.Tiles.ToArray();
            var cameraTransforms = new List<CameraTransform>();

            // 카메라 기준좌표에 마지막위치로 설정하면 마지막에 설정한 기준좌표를 알 수 없습니다.
            if (beatmap.Metadata.RelativeTo == Relativity.LastPosition)
                cameraRelativity = Relativity.Player;

            #endregion

            #region Process Camera Transform

            for (int floor = 0; floor < tiles.Length; floor++)
            {
                // 각도를 반전합니다.
                float resolvedRotation = CalculationExtensions.InvertAngle(tiles[floor].Angle);
                var position = -tiles[floor].Position;
                float bpm = tiles[floor].Bpm;

                foreach (var action in tiles[floor].Actions)
                {
                    void setCameraProperty(ActionEvents cameraActionEvent)
                    {
                        var offset = cameraActionEvent.Position.ToVector2() * (DrawableTile.WIDTH - Planet.PLANET_SIZE);

                        if (cameraActionEvent.RelativeTo != Relativity.LastPosition)
                        {
                            cameraOffset = offset;

                            if (cameraActionEvent.Rotation.HasValue)
                                cameraRotation = cameraActionEvent.Rotation.Value;
                        }

                        switch (cameraActionEvent.RelativeTo)
                        {
                            case Relativity.Global:
                                cameraPosition = Vector2.Zero;
                                break;

                            case Relativity.Player:
                            case Relativity.Tile:
                                cameraPosition = position;
                                break;

                            case Relativity.LastPosition:
                                if (cameraRelativity == Relativity.Player)
                                    cameraPosition = position;

                                if (cameraActionEvent.Rotation.HasValue)
                                    cameraRotation += cameraActionEvent.Rotation.Value;

                                cameraOffset += offset;
                                break;

                            case null:
                                break;
                        }

                        if (cameraActionEvent.RelativeTo != null && cameraActionEvent.RelativeTo != Relativity.LastPosition)
                            cameraRelativity = cameraActionEvent.RelativeTo.Value;
                    }

                    switch (action.EventType)
                    {
                        case EventType.MoveCamera:
                            setCameraProperty(action);
                            float angleOffset = CalculationExtensions.GetRelativeDuration(resolvedRotation, resolvedRotation + action.AngleOffset, bpm);
                            Vector2? specificPosition0 = cameraRelativity == Relativity.Player ? null : cameraPosition;

                            // 특정 시간에 카메라 변환을 해야함을 알리는데 사용됩니다.
                            cameraTransforms.Add(new CameraTransform
                            {
                                StartTime = tiles[floor].HitTime + angleOffset,
                                Duration = tiles.GetRelativeDuration(resolvedRotation, floor, bpm) * action.Duration,
                                Position = specificPosition0,
                                Offset = cameraOffset,
                                Rotation = cameraRotation,
                                Zoom = action.Zoom,
                                Easing = action.Ease
                            });
                            break;

                        // 이벤트 반복엔 타일에 종속되지 않는 이벤트(카메라 트랜스폼)가 실행됩니다.
                        case EventType.RepeatEvents:
                            var cameraEvents = Array.FindAll(tiles[action.Floor].Actions, a => a.EventType == EventType.MoveCamera);
                            double intervalBeat = 60000 / bpm * action.Interval;

                            // 이벤트를 일정한 주기로 반복 횟수만큼 추가합니다.
                            for (int i = 1; i <= action.Repetitions; i++)
                            {
                                double startTime = tiles[floor].HitTime + intervalBeat * i;

                                foreach (var cameraEvent in cameraEvents)
                                {
                                    setCameraProperty(cameraEvent);
                                    float angleTimeOffset = CalculationExtensions.GetRelativeDuration(resolvedRotation, resolvedRotation + cameraEvent.AngleOffset, bpm);
                                    Vector2? specificPosition = cameraRelativity == Relativity.Player ? null : cameraPosition;

                                    if (cameraEvent.EventTag == action.Tag)
                                    {
                                        // 특정 시간에 카메라 변환을 해야함을 알리는데 사용됩니다.
                                        cameraTransforms.Add(new CameraTransform
                                        {
                                            StartTime = startTime + angleTimeOffset,
                                            Duration = tiles.GetRelativeDuration(resolvedRotation, floor, bpm) * cameraEvent.Duration,
                                            Position = specificPosition,
                                            Offset = cameraOffset,
                                            Rotation = cameraRotation,
                                            Zoom = cameraEvent.Zoom,
                                            Easing = cameraEvent.Ease
                                        });
                                    }
                                }
                            }

                            break;
                    }
                }

                if (cameraRelativity == Relativity.Player)
                {
                    cameraTransforms.Add(new CameraTransform
                    {
                        StartTime = tiles[floor].HitTime,
                        Duration = 400 + 60 / bpm * 500,
                        Position = position,
                        Offset = null,
                        Rotation = null,
                        Zoom = null,
                        Easing = Easing.OutSine
                    });
                }
            }

            #endregion

            #region Register Camera Transform

            // 트랜스폼을 시작 시간순으로 추가해야 올바르게 추가됩니다.
            // 여기서 특정 시간에 발생하는 이벤트의 액션을 수행합니다.
            foreach (var cameraTransform in cameraTransforms.OrderBy(a => a.StartTime))
            {
                using (scalingContainer.BeginAbsoluteSequence(cameraTransform.StartTime, false))
                {
                    // 확대 값이 있으면 확대 트랜스폼을 수행합니다.
                    if (cameraTransform.Zoom.HasValue)
                    {
                        float cameraZoom = 1 / (cameraTransform.Zoom.Value / 100);

                        if (float.IsInfinity(cameraZoom))
                            cameraZoom = 0;

                        scalingContainer.ScaleTo(cameraZoom, cameraTransform.Duration, cameraTransform.Easing);
                    }

                    // 회전 값이 있으면 회전 트랜스폼을 실행합니다.
                    if (cameraTransform.Rotation.HasValue)
                        scalingContainer.RotateTo(cameraTransform.Rotation.Value, cameraTransform.Duration, cameraTransform.Easing);
                }

                if (cameraTransform.Offset.HasValue)
                {
                    using (offsetContainer.BeginAbsoluteSequence(cameraTransform.StartTime, false))
                        offsetContainer.MoveTo(cameraTransform.Offset.Value, cameraTransform.Duration, cameraTransform.Easing);
                }

                if (cameraTransform.Position.HasValue)
                {
                    using (positionContainer.BeginAbsoluteSequence(cameraTransform.StartTime, false))
                        positionContainer.MoveTo(cameraTransform.Position.Value, cameraTransform.Duration, cameraTransform.Easing);
                }
            }

            #endregion
        }
    }
}
