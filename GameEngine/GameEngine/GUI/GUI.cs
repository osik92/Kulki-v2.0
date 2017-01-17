using System.Collections.Generic;
using GameEngine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.GUI
{
    public class GUI
    {
        private List<Control> controls;
        InputHandler inputHandler;
        public SpriteFont TextFont;
        
        public GUI(InputHandler input)
        {
            controls = new List<Control>();
            this.inputHandler = input;
        }

        public void Update(GameTime gameTime)
        {
            foreach (Control control in controls)
            {
                if (control.IsActive)
                {
                    if (control.Bounds.Contains(inputHandler.GetMousePosition()))
                    {
                        control.OnHover();

                        if(control is Button && inputHandler.IsMouseButtonDown(eMouseButton.Left))
                            ((Button)control).OnClick();
                    }

                    control.Update(gameTime);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, float alpha)
        {
            foreach (Control control in controls)
            {
                control.Draw(spriteBatch, gameTime, alpha);
            }
        }

        public void AddControl(Control control)
        {
            controls.Add(control);
        }
    }
}
