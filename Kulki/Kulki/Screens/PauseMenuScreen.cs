using System;
using GameEngine.GUI;
using GameEngine.Screens;
using GameEngine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Kulki.Screens
{
    class PauseMenuScreen : GameScreen
    {
        private GUI gui;
        private Texture2D pause;
        public PauseMenuScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5f);
            TransitionOffTime = TimeSpan.FromSeconds(0.5f);
            IsPopup = true;
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                pause = ScreenManager.Content.Load<Texture2D>("pause2");
            }
            gui = new GUI(ScreenManager.Input);

            var btn = new Button("Wznów grę");
            btn.BackColor = Color.Transparent;

            btn.FrontColor = Color.White * 0.75f;
            btn.HoverColor = Color.White;
            btn.TextFont = ScreenManager.DefaultFont;
            btn.Position = new Vector2(ScreenManager.Viewport.Width / 2 - 50, 200);
            btn.ClickAction += () => { ExitScreen(); };
            gui.AddControl(btn);

            //btn = new Button("Opcje");
            //btn.BackColor = Color.Transparent;

            //btn.FrontColor = Color.White * 0.75f;
            //btn.HoverColor = Color.White;
            //btn.TextFont = ScreenManager.DefaultFont;
            //btn.Position = new Vector2(ScreenManager.Viewport.Width / 2 - 50, 200);
            //btn.ClickAction += () => { ScreenManager.AddScreen(new OptionMenuScreen());};
            //btn.IsActive = false;
            //gui.AddControl(btn);

            btn = new Button("Powrót do menu");
            btn.BackColor = Color.Transparent;

            btn.FrontColor = Color.White * 0.75f;
            btn.HoverColor = Color.White;
            btn.TextFont = ScreenManager.DefaultFont;
            btn.Position = new Vector2(ScreenManager.Viewport.Width / 2 - 50, 250);
            btn.ClickAction += () => {
                LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                        new MainMenuScreen());
            };
            gui.AddControl(btn);

            btn = new Button("Zakończ grę");
            btn.BackColor = Color.Transparent;

            btn.FrontColor = Color.White * 0.75f;
            btn.HoverColor = Color.White;
            btn.TextFont = ScreenManager.DefaultFont;
            btn.Position = new Vector2(ScreenManager.Viewport.Width / 2 - 50, 300);
            btn.ClickAction += () => { ScreenManager.Game.Exit(); };
            gui.AddControl(btn);


        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            if (State == eScreenState.Active && ScreenManager.Game.IsActive)
            {
                gui.Update(gameTime);
            }
            if(ScreenManager.Input.IsKeyDown(Keys.Escape))
                ExitScreen();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            var viewport = spriteBatch.GraphicsDevice.Viewport.Bounds;

            float scale = (viewport.Width/2f)/pause.Width;




            var bgRect = new Rectangle((int) ((viewport.Width/10f)*2.5f), (int) ((viewport.Height/10f)*1.05f),
                (int) (viewport.Width/2), (int) (viewport.Height /2));

            spriteBatch.Draw(spriteBatch.SolidTexture(), bgRect, null, Color.LightGreen * 0.75f * TransitionAlpha);

            spriteBatch.Draw(pause, new Vector2(viewport.Width/2, (viewport.Height/10f)*1.2f), null, Color.White * TransitionAlpha, 0,
                pause.Center(), scale, SpriteEffects.None, 0);

            gui.Draw(spriteBatch, gameTime, TransitionAlpha);
            spriteBatch.End();

        }
    }
}
