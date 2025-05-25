#nullable disable

using Circle.Game.Beatmaps;
using Circle.Game.Configuration;
using Circle.Game.Rulesets.Extensions;
using Circle.Game.Rulesets.Objects;
using Circle.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Circle.Game.Screens.Play
{
    public partial class PlanetContainer : Container<Planet>
    {
        public Planet RedPlanet { get; private set; }
        public Planet BluePlanet { get; private set; }

        private readonly Beatmap beatmap;

        private PlanetState rotatingPlanet = PlanetState.Ice;
        private Easing easing;

        public PlanetContainer(Beatmap beatmap)
        {
            this.beatmap = beatmap;
        }

        [BackgroundDependencyLoader]
        private void load(CircleConfigManager config)
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            AlwaysPresent = true;
            Children = new[]
            {
                RedPlanet = new Planet(Color4Utils.GetColor4(config.Get<Color4Enum>(CircleSetting.PlanetRed)))
                {
                    PlanetColour = config.GetBindable<Color4Enum>(CircleSetting.PlanetRed)
                },
                BluePlanet = new Planet(Color4Utils.GetColor4(config.Get<Color4Enum>(CircleSetting.PlanetBlue)))
                {
                    PlanetColour = config.GetBindable<Color4Enum>(CircleSetting.PlanetBlue)
                },
            };

            RedPlanet.Expansion = BluePlanet.Expansion = 0;
        }

        public void AddPlanetTransform()
        {
            var rotating = PlanetState.Ice;
            Tile prevTile = null;

            foreach (var tile in beatmap.Tiles)
            {
                float resolvedRotation = tile.Angle - CalculationExtensions.GetTimeBasedRotation(tile.StartTime, tile.EndTime, tile.Bpm);

                if (prevTile != null)
                    resolvedRotation = CalculationExtensions.ComputeStartRotation(prevTile.TileType, prevTile.Angle, tile.TileType, tile.Angle, tile.Clockwise);

                switch (rotating)
                {
                    case PlanetState.Fire:
                        using (BluePlanet.BeginAbsoluteSequence(tile.StartTime, false))
                            BluePlanet.ExpandTo(0);

                        using (RedPlanet.BeginAbsoluteSequence(tile.StartTime, false))
                        {
                            RedPlanet.ExpandTo(1);
                            RedPlanet.RotateTo(resolvedRotation);
                            RedPlanet.RotateTo(tile.Angle, tile.Duration, tile.Easing);
                        }

                        break;

                    case PlanetState.Ice:
                        using (RedPlanet.BeginAbsoluteSequence(tile.StartTime, false))
                            RedPlanet.ExpandTo(0);

                        using (BluePlanet.BeginAbsoluteSequence(tile.StartTime, false))
                        {
                            double expandDuration = tile.Floor == 0 ? 60000 / tile.Bpm : 0;

                            BluePlanet.ExpandTo(1, expandDuration);
                            BluePlanet.RotateTo(resolvedRotation);
                            BluePlanet.RotateTo(tile.Angle, tile.Duration, tile.Easing);
                        }

                        break;
                }

                using (BeginAbsoluteSequence(tile.StartTime, false))
                    this.MoveTo(tile.Position);

                rotating = rotating == PlanetState.Fire ? PlanetState.Ice : PlanetState.Fire;
                prevTile = tile;
            }

            if (prevTile == null)
                return;

            using (BeginAbsoluteSequence(prevTile.EndTime, false))
                this.MoveTo(prevTile.Position + CalculationExtensions.GetNextTilePosition(prevTile.Angle));

            switch (rotating)
            {
                case PlanetState.Fire:
                    using (BluePlanet.BeginAbsoluteSequence(prevTile.EndTime, false))
                        BluePlanet.ExpandTo(0);

                    using (RedPlanet.BeginAbsoluteSequence(prevTile.EndTime, false))
                    {
                        RedPlanet.ExpandTo(1);
                        RedPlanet.Spin(60000 / prevTile.Bpm * 2, prevTile.Clockwise ? RotationDirection.Clockwise : RotationDirection.Counterclockwise, prevTile.Angle + 180);
                    }

                    break;

                case PlanetState.Ice:
                    using (RedPlanet.BeginAbsoluteSequence(prevTile.EndTime, false))
                        RedPlanet.ExpandTo(0);

                    using (BluePlanet.BeginAbsoluteSequence(prevTile.EndTime, false))
                    {
                        BluePlanet.ExpandTo(1);
                        BluePlanet.Spin(60000 / prevTile.Bpm * 2, prevTile.Clockwise ? RotationDirection.Clockwise : RotationDirection.Counterclockwise, prevTile.Angle + 180);
                    }

                    break;
            }
        }
    }

    public enum PlanetState
    {
        Fire,
        Ice
    }
}
