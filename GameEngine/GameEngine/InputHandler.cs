using System.Runtime.CompilerServices;
using GameEngine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameEngine
{
    public class InputHandler
    {
        private KeyboardState currenKeyboardState;
        private KeyboardState lastKeyboardState;
        private MouseState currentMouseState;
        private MouseState lastMouseState;

        public InputHandler()
        {
            currenKeyboardState = lastKeyboardState = Keyboard.GetState();
            currentMouseState = lastMouseState = Mouse.GetState();
        }
        public void Update()
        {
            lastKeyboardState = currenKeyboardState;
            lastMouseState = currentMouseState;
            currenKeyboardState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();
        }

        public bool IsKeyDown(Keys key)
        {
            return currenKeyboardState[key] == KeyState.Down && lastKeyboardState[key] == KeyState.Up;
        }

        public bool IsKeyUp(Keys key)
        {
            return currenKeyboardState[key] == KeyState.Up && lastKeyboardState[key] == KeyState.Down;
        }

        public bool IsKeyPress(Keys key)
        {
            return currenKeyboardState[key] == KeyState.Down;
        }
        public bool IsMouseButtonDown(eMouseButton button)
        {
            return currentMouseState.MouseState(button) == ButtonState.Pressed && lastMouseState.MouseState(button) == ButtonState.Released;
        }

        public bool IsMouseButtonUp(eMouseButton button)
        {
            return currentMouseState.MouseState(button) == ButtonState.Released && lastMouseState.MouseState(button) == ButtonState.Pressed;
        }
        public bool IsMouseButtonPress(eMouseButton button)
        {
            return currentMouseState.MouseState(button) == ButtonState.Pressed;
        }

        public Point GetMousePosition()
        {
            return currentMouseState.Position;
        }

        public Point GetDeltaMousePosition()
        {
            return currentMouseState.Position - lastMouseState.Position;
        }

        public float GetMouseWheelPosition()
        {
            return currentMouseState.ScrollWheelValue;
        }
        public float GetDeltaMouseWheelPosition()
        {
            return currentMouseState.ScrollWheelValue - lastMouseState.ScrollWheelValue;
        }
    }
}