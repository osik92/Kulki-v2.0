using System;
using GameEngine.Screens;
using GameEngine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Kulki.Screens
{
    public  class SplashScreen : GameScreen
    {

        Texture2D background;
        private TimeSpan showTime = TimeSpan.FromSeconds(0.0);
        private bool startingNewScreen = false;

        public SplashScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.0f);
            TransitionOffTime = TimeSpan.FromSeconds(0.05f);
            CursorVisible = false;
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                background = ScreenManager.Content.Load<Texture2D>("SplashScreen");
            }
        }



        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
            if (State == eScreenState.Active)
            {
                showTime -= gameTime.ElapsedGameTime;
                if (showTime <= TimeSpan.FromSeconds(0.0))
                {
                    if (!IsExiting)
                        ExitScreen();
                    return;
                }

            }
            else if (State == eScreenState.TransitionOff && !startingNewScreen)
            {
                showTime -= gameTime.ElapsedGameTime;
                if (showTime <= TimeSpan.FromSeconds(0))
                {
                    ScreenManager.AddScreen(new BackgroundScreen());
                    ScreenManager.AddScreen(new MainMenuScreen());
                    
                    startingNewScreen = true;
                    return;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            Vector2 screenSize = new Vector2 (spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height);
            Vector2 imageSize = new Vector2(background.Width, background.Height);
            

            float scale = (float)screenSize.Y/(float)imageSize.Y;

            imageSize = new Vector2(background.Width * scale, background.Height * scale);
            var rect = new Rectangle(
                (int) (screenSize.X/2f), (int) (screenSize.Y/2f),
                (int) (imageSize.X*0.75f), (int) (imageSize.Y*0.75f));

            spriteBatch.Begin();
            spriteBatch.Draw(background, rect, null, Color.White * TransitionAlpha, 0, background.Center(), SpriteEffects.None, 0 );
            spriteBatch.End();
        }

        public override void Unload()
        {
            background.Dispose();
        }
    }
}