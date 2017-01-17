using System;
using GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kulki
{
    public class FPSCounter : DrawableGameComponent
    {
        private ScreenManager manager;
        private SpriteFont font;
        private SpriteBatch spriteBatch;
        private int totalFrames = 0;
        private float elapsedTime = 0.0f;
        private int fps = 0;
        private bool enable = false;

        public FPSCounter(Game game, ScreenManager manager) : base(game)
        {
            this.manager = manager;
            
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = manager.Content.Load<SpriteFont>("MenuItemsFont");

        }

        public void Enabled()
        {
            enable = true;
        }

        public void Disable()
        {
            enable = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (enable)
            {
                elapsedTime += (float) gameTime.ElapsedGameTime.TotalMilliseconds;

                if (elapsedTime >= 1000.0f)
                {
                    fps = totalFrames;
                    totalFrames = 0;
                    elapsedTime = 0;
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (enable)
            {

                totalFrames++;
                
                spriteBatch.Begin();
                spriteBatch.DrawString(font, String.Format("FPS: {0}", fps), Vector2.Zero, Color.Black);
                spriteBatch.DrawString(font, String.Format("FPS: {0}", fps), Vector2.One, Color.White);

                //spriteBatch.DrawString(font, "FPS: "  + Math.Floor(1000f / gameTime.ElapsedGameTime.Milliseconds), Vector2.Zero, Color.Black);
                //spriteBatch.DrawString(font, "FPS: " + Math.Floor(1000f / gameTime.ElapsedGameTime.Milliseconds), Vector2.One, Color.White);
                spriteBatch.End();
            }
        }
    }
}
