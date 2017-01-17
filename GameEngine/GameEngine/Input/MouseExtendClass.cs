using Microsoft.Xna.Framework.Input;

namespace GameEngine.Input
{
    public static class MouseExtendClass
    {
        public static ButtonState MouseState(this MouseState mouse, eMouseButton button)
        {
            switch (button)
            {
                case eMouseButton.Left:
                    return mouse.LeftButton;
                case eMouseButton.Middle:
                    return mouse.MiddleButton;
                case eMouseButton.Right:
                    return mouse.RightButton;
                case eMouseButton.X1:
                    return mouse.XButton1;
                case eMouseButton.X2:
                    return mouse.XButton2;
                default:
                    return mouse.LeftButton;
            }
        }
    }
}