#nullable disable

using System;
using Circle.Game.Input;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace Circle.Game.Overlays.Volume
{
    public partial class VolumeControlReceptor : Container, IScrollBindingHandler<InputAction>, IHandleGlobalKeyboardInput
    {
        public Func<InputAction, bool> ActionRequested;
        public Func<InputAction, float, bool, bool> ScrollActionRequested;

        public bool OnPressed(KeyBindingPressEvent<InputAction> e)
        {
            switch (e.Action)
            {
                case InputAction.DecreaseVolume:
                case InputAction.IncreaseVolume:
                case InputAction.NextVolumeMeter:
                case InputAction.PreviousVolumeMeter:
                    ActionRequested?.Invoke(e.Action);
                    return true;
            }

            return false;
        }

        public void OnReleased(KeyBindingReleaseEvent<InputAction> e)
        {
        }

        public bool OnScroll(KeyBindingScrollEvent<InputAction> e) =>
            ScrollActionRequested?.Invoke(InputAction.IncreaseVolume, e.ScrollAmount, e.IsPrecise) ?? false;

        protected override bool OnScroll(ScrollEvent e)
        {
            ScrollActionRequested?.Invoke(InputAction.IncreaseVolume, e.ScrollDelta.Y, e.IsPrecise);

            return true;
        }
    }
}
