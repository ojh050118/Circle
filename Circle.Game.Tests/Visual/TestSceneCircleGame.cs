using osu.Framework.Allocation;
using osu.Framework.Platform;

namespace Circle.Game.Tests.Visual
{
    public class TestSceneCircleGame : CircleTestScene
    {
        private CircleGame game;

        [BackgroundDependencyLoader]
        private void load(GameHost host)
        {
            game = new CircleGame();
            game.SetHost(host);

            AddGame(game);
        }
    }
}
