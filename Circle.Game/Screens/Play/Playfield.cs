using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics;
using Circle.Game.Rulesets.Objects;
using osuTK.Graphics;
using osu.Framework.Bindables;
using Circle.Game.Beatmap;
using osu.Framework.Allocation;
using Circle.Game.Rulesets.Extensions;
using System;

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

        private RotationDirection rotationDirection;

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
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            bluePlanet.Rotation = tiles.FilteredAngles[0] - 180;
            planetState.ValueChanged += _ => movePlanet();
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
                this.MoveTo(-tiles.PlanetPositions[currentFloor], 250, Easing.OutSine);
                return;
            }

            float fixedRotation = prevAngle - 180;
            float newRotation = tiles.FilteredAngles[currentFloor] > 180 ? tiles.FilteredAngles[currentFloor] - 360 : tiles.FilteredAngles[currentFloor];

            // 반시계방향으로 회전할 여지가 있는지 확인하고 수정함.
            if (rotationDirection == RotationDirection.Clockwise)
                newRotation = fixedRotation > newRotation ? 360 + newRotation : newRotation;

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

            this.MoveTo(-tiles.PlanetPositions[currentFloor], 250, Easing.OutSine);
        }

        private float calculateDuration(float newRotation)
        {
            return 60000 / (float)beatmap.Value.Settings.BPM * Math.Abs((planetState.Value == PlanetState.Fire ? redPlanet.Rotation : bluePlanet.Rotation) - newRotation) / 180;
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
