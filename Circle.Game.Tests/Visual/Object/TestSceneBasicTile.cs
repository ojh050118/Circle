﻿using Circle.Game.Rulesets.Objects;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;

namespace Circle.Game.Tests.Visual.Object
{
    public class TestSceneBasicTile : CircleTestScene
    {
        public TestSceneBasicTile()
        {
            Add(new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Color4.Black
            });
            Add(new BasicTile(0)
            {
                Anchor = Anchor.Centre,
            });
        }
    }
}