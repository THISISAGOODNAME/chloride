﻿using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AnotherWheel.Viewer.Components {
    internal sealed class KeyboardStateHandler : GameComponent {

        public KeyboardStateHandler([NotNull] Game game)
            : base(game) {
            _previousState = Keyboard.GetState();
        }

        public event EventHandler<KeyEventArgs> KeyDown;

        public event EventHandler<KeyEventArgs> KeyUp;

        public event EventHandler<KeyEventArgs> KeyHold;

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);

            if (!Enabled) {
                return;
            }

            var state = Keyboard.GetState();

            var oldPressed = _previousState.GetPressedKeys();
            var newPressed = state.GetPressedKeys();

            for (var i = 0; i < oldPressed.Length; ++i) {
                if (Array.IndexOf(newPressed, oldPressed[i]) < 0) {
                    var e = new KeyEventArgs(oldPressed[i], KeyState.Down, KeyState.Up);

                    KeyUp?.Invoke(this, e);
                }
            }

            for (var i = 0; i < newPressed.Length; ++i) {
                if (Array.IndexOf(oldPressed, newPressed[i]) < 0) {
                    var e = new KeyEventArgs(newPressed[i], KeyState.Up, KeyState.Down);

                    KeyDown?.Invoke(this, e);
                } else {
                    var e = new KeyEventArgs(newPressed[i], KeyState.Down, KeyState.Down);

                    KeyHold?.Invoke(this, e);
                }
            }

            _previousState = state;
        }

        private KeyboardState _previousState;

    }
}
