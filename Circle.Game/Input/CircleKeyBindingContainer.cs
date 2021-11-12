using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using osu.Framework.Graphics;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;

namespace Circle.Game.Input
{
    public class CircleKeyBindingContainer : KeyBindingContainer<InputAction>
    {
        private readonly Drawable handler;

        [CanBeNull]
        private InputManager parentInputManager;

        public override IEnumerable<IKeyBinding> DefaultKeyBindings => GlobalKeyBindings;

        public IEnumerable<KeyBinding> GlobalKeyBindings => new[]
        {
            new KeyBinding(InputKey.P, InputAction.Play),
            new KeyBinding(InputKey.S, InputAction.Settings),
            new KeyBinding(InputKey.E, InputAction.Exit),
            new KeyBinding(InputKey.Escape, InputAction.Back)
        };

        public CircleKeyBindingContainer(CircleGameBase game)
            : base(matchingMode: KeyCombinationMatchingMode.Modifiers)
        {
            if (game is IKeyBindingHandler<InputAction>)
                handler = game;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            parentInputManager = GetContainingInputManager();
        }

        protected override IEnumerable<Drawable> KeyBindingInputQueue
        {
            get
            {
                var inputQueue = parentInputManager?.NonPositionalInputQueue ?? base.KeyBindingInputQueue;

                return handler != null ? inputQueue.Prepend(handler) : inputQueue;
            }
        }
    }
}
