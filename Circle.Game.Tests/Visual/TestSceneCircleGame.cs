#nullable disable

using System;
using System.Collections.Generic;
using Circle.Game.Beatmaps;
using Circle.Game.Configuration;
using Circle.Game.Graphics;
using Circle.Game.Graphics.UserInterface;
using Circle.Game.Overlays;
using Circle.Game.Overlays.OSD;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Textures;
using osu.Framework.Platform;

namespace Circle.Game.Tests.Visual
{
    public partial class TestSceneCircleGame : CircleTestScene
    {
        private readonly IReadOnlyList<Type> requiredGameDependencies = new[]
        {
            typeof(CircleGame),
            typeof(LargeTextureStore),
            typeof(BeatmapStorage),
            typeof(BeatmapManager),
            typeof(MusicController),
            typeof(CircleGameBase),
            typeof(CircleConfigManager),
            typeof(Toast),
            typeof(Background),
            typeof(VolumeOverlay),
            typeof(ImportOverlay),
            typeof(DialogOverlay),
            typeof(CircleColour),
            typeof(ConvertOverlay)
        };

        private CircleGame game;

        [BackgroundDependencyLoader]
        private void load(GameHost host)
        {
            game = new CircleGame();
            game.SetHost(host);

            AddGame(game);
            AddAssert("Check DI members", () =>
            {
                foreach (var type in requiredGameDependencies)
                {
                    if (game.Dependencies.Get(type) == null)
                        throw new InvalidOperationException($"{type} has not been cached");
                }

                return true;
            });
        }
    }
}
