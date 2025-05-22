#nullable disable

using System;
using Circle.Game.Beatmaps;
using Circle.Game.Graphics.UserInterface;
using Circle.Game.Rulesets.Extensions;
using Circle.Game.Rulesets.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

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
        private void load()
        {
            RelativeSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Children = new Drawable[]
            {
                new Background(TextureSource.External, beatmapInfo: currentBeatmap.BeatmapInfo),
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
        }

        protected override void LoadComplete()
        {
            tileContainer.AddTileTransforms();
            planetContainer.AddPlanetTransform();
            cameraContainer.AddCameraTransforms();

            this.FilterToggleTo(FilterType.Arcade, true);
            //this.FilterTo(FilterType.Led, 0.5f, 100f, Easing.OutQuint);

            base.LoadComplete();
        }

        private void addFilterTransform()
        {
            foreach (var tile in currentBeatmap.Tiles)
            {
            }
        }
    }
}
