using System;
using System.Collections.Generic;
using Circle.Game.Beatmaps;
using Circle.Game.Configuration;
using Circle.Game.Graphics.Containers;
using Circle.Game.Graphics.UserInterface;
using Circle.Game.Overlays;
using Circle.Game.Overlays.OSD;
using osu.Framework.Allocation;
using osu.Framework.Configuration.Tracking;
using osu.Framework.Graphics.Textures;
using osu.Framework.Platform;

namespace Circle.Game.Tests.Visual
{
    public class TestSceneCircleGame : CircleTestScene
    {
        private CircleGame game;

        private readonly IReadOnlyList<Type> requiredGameDependencies = new[]
        {
            typeof(CircleGame),
            typeof(LargeTextureStore),
            typeof(BeatmapStorage),
            typeof(BeatmapManager),
            typeof(MusicController),
            typeof(CircleGameBase),
            typeof(TrackedSettings),
            typeof(CircleConfigManager),
            typeof(GameScreenContainer),
            typeof(Toast),
            typeof(Background),
            typeof(ImportOverlay),
            typeof(DialogOverlay)
        };

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
