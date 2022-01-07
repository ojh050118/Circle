﻿using System;
using Circle.Game.Beatmap;
using Circle.Game.Rulesets.Extensions;
using Circle.Game.Rulesets.Objects;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK.Graphics;

namespace Circle.Game.Screens.Play
{
    public class Playfield : Container
    {
        private readonly ObjectContainer tiles;
        private readonly Planet redPlanet;
        private readonly Planet bluePlanet;

        [Resolved]
        private Bindable<BeatmapInfo> beatmap { get; set; }

        /// <summary>
        /// 현재 회전하는 행성을 가리킵니다.
        /// </summary>
        private readonly Bindable<PlanetState> planetState;

        private int currentFloor;

        private float prevAngle;

        private bool isClockwise;

        private float currentBpm;

        public Playfield()
        {
            AutoSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Children = new Drawable[]
            {
                tiles = new ObjectContainer(),
                redPlanet = new Planet(Color4.Red),
                bluePlanet = new Planet(Color4.DeepSkyBlue),
            };

            redPlanet.Expansion = bluePlanet.Expansion = 0;
            planetState = new Bindable<PlanetState>(PlanetState.Ice);
            isClockwise = true;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            currentBpm = beatmap.Value.Settings.Bpm;
            bluePlanet.Rotation = tiles.Children[0].Angle - 180;
            planetState.ValueChanged += _ => movePlanet();
            planetState.ValueChanged += _ => this.MoveTo(-tiles.Children[currentFloor].Position, 500, Easing.OutSine);

            for (int i = 9; i < tiles.Children.Count; i++)
            {
                tiles.Children[i].Alpha = 0;
            }
        }

        public void StartPlaying()
        {
            bluePlanet.ExpandTo(1, 60000 / beatmap.Value.Settings.Bpm, Easing.Out);
            bluePlanet.RotateTo(tiles.Children[currentFloor].Angle, getRelativeDuration(tiles.Children[currentFloor].Angle))
                      .Then()
                      .Schedule(changePlanetState);
        }

        private void movePlanet()
        {
            // 음수인 각도가 나올 수 있기때문에 각도 수정.
            var currentAngle = CalculationExtensions.GetSafeAngle(tiles.Children[currentFloor].Angle);

            if (tiles.Children[currentFloor].Reverse.Value)
                isClockwise = !isClockwise;

            if (tiles.Children[currentFloor].SpeedType != null)
            {
                switch (tiles.Children[currentFloor].SpeedType)
                {
                    case SpeedType.Multiplier:
                        currentBpm *= tiles.Children[currentFloor].BpmMultiplier.Value;
                        break;

                    case SpeedType.Bpm:
                        currentBpm = tiles.Children[currentFloor].Bpm.Value;
                        break;
                }
            }

            float fixedRotation = prevAngle;

            // 현재 타일이 미드스핀 타일일 때 계산하면 안됩니다.
            if (tiles.Children[currentFloor].Angle != 999 && tiles.Children[currentFloor - 1].Angle != 999)
                fixedRotation -= 180;

            float newRotation = currentAngle >= 180 ? currentAngle - 360 : currentAngle;

            // 회전방향에 따라 새로운 각도 계산
            if (isClockwise)
            {
                if (fixedRotation >= newRotation)
                    newRotation += 360;
            }
            else
            {
                if (fixedRotation <= newRotation)
                    newRotation -= 360;
            }

            if (planetState.Value == PlanetState.Fire)
            {
                redPlanet.Expansion = 1;
                redPlanet.Position = tiles.Children[currentFloor].Position;
                redPlanet.Rotation = fixedRotation;
                redPlanet.RotateTo(newRotation, getRelativeDuration(newRotation))
                         .Then()
                         .Schedule(changePlanetState);
            }
            else
            {
                bluePlanet.Expansion = 1;
                bluePlanet.Position = tiles.Children[currentFloor].Position;
                bluePlanet.Rotation = fixedRotation;
                bluePlanet.RotateTo(newRotation, getRelativeDuration(newRotation))
                          .Then()
                          .Schedule(changePlanetState);
            }
        }

        private void changePlanetState()
        {
            if (currentFloor + 1 >= tiles.Children.Count)
            {
                var lastPosition = tiles.Children[currentFloor].Position + CalculationExtensions.GetComputedTilePosition(tiles.Children[currentFloor].Angle);
                this.MoveTo(-lastPosition, 500, Easing.OutSine);

                return;
            }

            currentFloor++;

            if (currentFloor + 8 < tiles.Children.Count)
                tiles.Children[currentFloor + 8].FadeTo(0.6f, 60000 / currentBpm, Easing.Out);
            else
                tiles.Children[currentFloor].FadeTo(0.6f, 60000 / currentBpm, Easing.Out);

            if (currentFloor > 3)
                tiles.Children[currentFloor - 4].FadeOut(60000 / currentBpm, Easing.Out);

            if (planetState.Value == PlanetState.Fire)
            {
                redPlanet.Expansion = 0;
                prevAngle = redPlanet.Rotation = CalculationExtensions.GetSafeAngle(redPlanet.Rotation);
                if (currentFloor < tiles.Children.Count)
                    redPlanet.Position = tiles.Children[currentFloor].Position;
            }
            else
            {
                bluePlanet.Expansion = 0;
                prevAngle = bluePlanet.Rotation = CalculationExtensions.GetSafeAngle(bluePlanet.Rotation);
                if (currentFloor < tiles.Children.Count)
                    bluePlanet.Position = tiles.Children[currentFloor].Position;
            }

            if (tiles.Children[currentFloor].TileType == TileType.Normal)
                planetState.Value = planetState.Value == PlanetState.Fire ? PlanetState.Ice : PlanetState.Fire;
            else
            {
                currentFloor++;

                if (currentFloor + 8 < tiles.Children.Count)
                    tiles.Children[currentFloor + 8].FadeTo(0.6f, 60000 / currentBpm, Easing.Out);
                else
                    tiles.Children[currentFloor].FadeTo(0.6f, 60000 / currentBpm, Easing.Out);

                if (currentFloor > 3)
                    tiles.Children[currentFloor - 4].FadeOut(60000 / currentBpm, Easing.Out);

                movePlanet();
            }
        }

        private float getRelativeDuration(float newRotation) => 60000 / currentBpm * Math.Abs((planetState.Value == PlanetState.Fire ? redPlanet.Rotation : bluePlanet.Rotation) - newRotation) / 180;
    }
}
