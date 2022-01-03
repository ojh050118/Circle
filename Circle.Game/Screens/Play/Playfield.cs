using System;
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

            bluePlanet.Rotation = tiles.FilteredAngles[0] - 180;
            planetState.ValueChanged += _ => movePlanet();
            currentBpm = beatmap.Value.Settings.BPM;
        }

        public void StartPlaying()
        {
            bluePlanet.ExpandTo(1, 60000 / beatmap.Value.Settings.BPM, Easing.Out);
            bluePlanet.RotateTo(tiles.FilteredAngles[currentFloor], calculateDuration(tiles.FilteredAngles[currentFloor]))
                      .Then()
                      .Schedule(() =>
                      {
                          bluePlanet.Expansion = 0;
                          currentFloor++;
                          bluePlanet.Rotation = getSafeAngle(bluePlanet.Rotation);
                          bluePlanet.Position = tiles.PlanetPositions[currentFloor];
                          prevAngle = bluePlanet.Rotation;
                          planetState.Value = PlanetState.Fire;
                      });
        }

        private void movePlanet()
        {
            if (currentFloor >= tiles.FilteredAngles.Count)
            {
                this.MoveTo(-tiles.PlanetPositions[currentFloor], 500, Easing.OutSine);
                return;
            }

            if (tiles.Children[currentFloor].Reverse.Value)
                isClockwise = !isClockwise;

            float fixedRotation = prevAngle - 180;
            float newRotation = tiles.FilteredAngles[currentFloor] > 180 ? tiles.FilteredAngles[currentFloor] - 360 : tiles.FilteredAngles[currentFloor];

            // 회전방향에 따라 새로운 각도 계산
            if (isClockwise)
                newRotation = fixedRotation > newRotation ? 360 + newRotation : newRotation;
            else
                newRotation = fixedRotation < newRotation ? newRotation - 360 : newRotation;

            if (planetState.Value == PlanetState.Fire)
            {
                redPlanet.Expansion = 1;
                redPlanet.Position = tiles.PlanetPositions[currentFloor];
                redPlanet.Rotation = fixedRotation;
                redPlanet.RotateTo(newRotation, calculateDuration(newRotation))
                         .Then()
                         .Schedule(() =>
                         {
                             redPlanet.Expansion = 0;
                             currentFloor++;
                             redPlanet.Rotation = getSafeAngle(redPlanet.Rotation);
                             prevAngle = redPlanet.Rotation;
                             if (currentFloor < tiles.PlanetPositions.Count)
                                 redPlanet.Position = tiles.PlanetPositions[currentFloor];
                             planetState.Value = PlanetState.Ice;
                         });
            }
            else
            {
                bluePlanet.Expansion = 1;
                bluePlanet.Position = tiles.PlanetPositions[currentFloor];
                bluePlanet.Rotation = fixedRotation;
                bluePlanet.RotateTo(newRotation, calculateDuration(newRotation))
                          .Then()
                          .Schedule(() =>
                          {
                              bluePlanet.Expansion = 0;
                              currentFloor++;
                              bluePlanet.Rotation = getSafeAngle(bluePlanet.Rotation);
                              prevAngle = bluePlanet.Rotation;
                              if (currentFloor < tiles.PlanetPositions.Count)
                                  bluePlanet.Position = tiles.PlanetPositions[currentFloor];
                              planetState.Value = PlanetState.Fire;
                          });
            }

            this.MoveTo(-tiles.PlanetPositions[currentFloor], 500, Easing.OutSine);
        }

        private float calculateDuration(float newRotation)
        {
            return 60000 / currentBpm * Math.Abs((planetState.Value == PlanetState.Fire ? redPlanet.Rotation : bluePlanet.Rotation) - newRotation) / 180;
        }

        private float getSafeAngle(float angle)
        {
            if (angle < 0)
            {
                while (angle < 0)
                    angle += 360;

                return angle;
            }

            if (angle <= 360)
                return angle;

            while (angle > 360)
                angle -= 360;

            return angle;
        }
    }
}
