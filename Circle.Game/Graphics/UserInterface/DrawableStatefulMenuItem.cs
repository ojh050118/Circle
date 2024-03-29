﻿#nullable disable

using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace Circle.Game.Graphics.UserInterface
{
    public partial class DrawableStatefulMenuItem : DrawableCircleMenuItem
    {
        public DrawableStatefulMenuItem(StatefulMenuItem item)
            : base(item)
        {
        }

        protected new StatefulMenuItem Item => (StatefulMenuItem)base.Item;

        protected override TextContainer CreateTextContainer() => new ToggleTextContainer(Item);

        private partial class ToggleTextContainer : TextContainer
        {
            private readonly StatefulMenuItem menuItem;
            private readonly Bindable<object> state;
            private readonly SpriteIcon stateIcon;

            public ToggleTextContainer(StatefulMenuItem menuItem)
            {
                this.menuItem = menuItem;

                state = menuItem.State.GetBoundCopy();

                Add(stateIcon = new SpriteIcon
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Size = new Vector2(10),
                    Margin = new MarginPadding { Horizontal = MARGIN_HORIZONTAL },
                    AlwaysPresent = true,
                });
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();
                state.BindValueChanged(updateState, true);
            }

            protected override void Update()
            {
                base.Update();

                stateIcon.X = BoldText.DrawWidth + 10;
            }

            private void updateState(ValueChangedEvent<object> state)
            {
                var icon = menuItem.GetIconForState(state.NewValue);

                if (icon == null)
                    stateIcon.Alpha = 0;
                else
                {
                    stateIcon.Alpha = 1;
                    stateIcon.Icon = icon.Value;
                }
            }
        }
    }
}
