using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Final_Bomber.Controls
{
    class InputHandler : Microsoft.Xna.Framework.GameComponent
    {
        #region Mouse Field Region

        static MouseState _mouseState;
        static MouseState _lastMouseState;

        #endregion

        #region Keyboard Field Region

        static KeyboardState _keyboardState;
        static KeyboardState _lastKeyboardState;

        #endregion

        #region Game Pad Field Region

        static GamePadState[] _gamePadStates;
        static GamePadState[] _lastGamePadStates;

        #endregion

        #region Mouse Property Region

        public static MouseState MouseState
        {
            get { return _mouseState; }
        }

        public static MouseState LastMouseState
        {
            get { return _lastMouseState; }
        }

        #endregion

        #region Keyboard Property Region

        public static KeyboardState KeyboardState
        {
            get { return _keyboardState; }
        }

        public static KeyboardState LastKeyboardState
        {
            get { return _lastKeyboardState; }
        }

        #endregion

        #region Game Pad Property Region

        public static GamePadState[] GamePadStates
        {
            get { return _gamePadStates; }
        }

        public static GamePadState[] LastGamePadStates
        {
            get { return _lastGamePadStates; }
        }

        #endregion

        #region Constructor Region

        public InputHandler(Game game)
            : base(game)
        {
            _mouseState = Mouse.GetState();

            _keyboardState = Keyboard.GetState();

            _gamePadStates = new GamePadState[Enum.GetValues(typeof(PlayerIndex)).Length];

            foreach (PlayerIndex index in Enum.GetValues(typeof(PlayerIndex)))
                _gamePadStates[(int)index] = GamePad.GetState(index);
        }

        #endregion

        #region XNA methods

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            _lastMouseState = _mouseState;
            _mouseState = Mouse.GetState();

            _lastKeyboardState = _keyboardState;
            _keyboardState = Keyboard.GetState();

            _lastGamePadStates = (GamePadState[])_gamePadStates.Clone();
            foreach (PlayerIndex index in Enum.GetValues(typeof(PlayerIndex)))
                _gamePadStates[(int)index] = GamePad.GetState(index);

            base.Update(gameTime);
        }

        #endregion

        #region General Method Region

        public static void Flush()
        {
            _lastMouseState = _mouseState;
            _lastKeyboardState = _keyboardState;
        }

        public static bool PressedUp()
        {
            return
                (KeyPressed(Keys.Up) ||
                ButtonPressed(Buttons.DPadUp, PlayerIndex.One) ||
                ButtonPressed(Buttons.LeftThumbstickUp, PlayerIndex.One));
        }

        public static bool PressedDown()
        {
            return
                (KeyPressed(Keys.Down) ||
                ButtonPressed(Buttons.DPadDown, PlayerIndex.One) ||
                ButtonPressed(Buttons.LeftThumbstickDown, PlayerIndex.One));
        }

        public static bool PressedLeft()
        {
            return
                (KeyPressed(Keys.Left) ||
                ButtonPressed(Buttons.DPadLeft, PlayerIndex.One) ||
                ButtonPressed(Buttons.LeftThumbstickLeft, PlayerIndex.One));
        }

        public static bool PressedRight()
        {
            return
                (KeyPressed(Keys.Right) ||
                ButtonPressed(Buttons.DPadRight, PlayerIndex.One) ||
                ButtonPressed(Buttons.LeftThumbstickRight, PlayerIndex.One));
        }

        public static bool PressedAction()
        {
            return
                (KeyPressed(Keys.Enter) ||
                ButtonPressed(Buttons.A, PlayerIndex.One));
        }

        public static bool PressedCancel()
        {
            return
                (KeyPressed(Keys.Escape) ||
                ButtonPressed(Buttons.B, PlayerIndex.One));
        }

        #endregion

        #region Mouse Region

        public static bool Scroll()
        {
            return _mouseState.ScrollWheelValue == _lastMouseState.ScrollWheelValue;
        }

        public static bool ScrollUp()
        {
            return _mouseState.ScrollWheelValue > _lastMouseState.ScrollWheelValue;
        }

        public static bool ScrollDown()
        {
            return _mouseState.ScrollWheelValue < _lastMouseState.ScrollWheelValue;
        }

        #endregion

        #region Keyboard Region

        public static bool KeyReleased(Keys key)
        {
            return _keyboardState.IsKeyUp(key) &&
                _lastKeyboardState.IsKeyDown(key);
        }

        public static bool KeyPressed(Keys key)
        {
            return _keyboardState.IsKeyDown(key) &&
                _lastKeyboardState.IsKeyUp(key);
        }

        public static bool KeyDown(Keys key)
        {
            return _keyboardState.IsKeyDown(key);
        }

        public static bool HavePressedKey()
        {
            return _keyboardState != _lastKeyboardState;
        }

        public static Keys[] GetPressedKeys()
        {
            return _keyboardState.GetPressedKeys();
        }

        #endregion

        #region Game Pad Region

        public static bool ButtonReleased(Buttons button, PlayerIndex index)
        {
            return _gamePadStates[(int)index].IsButtonUp(button) &&
                _lastGamePadStates[(int)index].IsButtonDown(button);
        }

        public static bool ButtonPressed(Buttons button, PlayerIndex index)
        {
            return _gamePadStates[(int)index].IsButtonDown(button) &&
                _lastGamePadStates[(int)index].IsButtonUp(button);
        }

        public static bool ButtonDown(Buttons button, PlayerIndex index)
        {
            return _gamePadStates[(int)index].IsButtonDown(button);
        }

        public static bool HavePressedButton(PlayerIndex index)
        {
            return _gamePadStates[(int)index] != _lastGamePadStates[(int)index];
        }

        public static Buttons[] GetPressedButton(PlayerIndex index)
        {
            return Enum.GetValues(typeof(Buttons)).Cast<Buttons>().Where(button => ButtonPressed(button, index)).ToArray();
        }

        #endregion
    }
}
