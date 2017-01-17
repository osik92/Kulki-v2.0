using System;
using GameEngine.Screens;
using GameEngine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Kulki.Screens
{
    public  class BackgroundScreen : GameScreen
    {
        Texture2D background;

        public BackgroundScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5f);
            TransitionOffTime = TimeSpan.FromSeconds(1.0f);
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                background = ScreenManager.Content.Load<Texture2D>("bg");
            }
        }



        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();
            spriteBatch.Draw(background, ScreenManager.GraphicsDevice.Viewport.Bounds, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
            
            spriteBatch.Draw(spriteBatch.SolidTexture(), ScreenManager.GraphicsDevice.Viewport.Bounds, Color.Black * 0.25f);
            spriteBatch.End();
        }
    }
}