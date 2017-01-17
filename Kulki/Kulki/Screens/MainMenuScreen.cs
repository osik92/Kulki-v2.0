using System;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using GameEngine.GUI;
using GameEngine.Screens;
using GameEngine.Utilities;
using Math = System.Math;
using EngineMath = GameEngine.Utilities.Math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Kulki.Screens
{
    internal class MainMenuScreen : GameScreen
    {
        private SpriteFont MenuItemsFont;
        private Song bgSong;
        private Texture2D logo;

        private bool showSelectPlayerScreen = false;
      


        private GUI gui;
        public MainMenuScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5f);
            TransitionOffTime = TimeSpan.FromSeconds(0.5f);
        }
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                MenuItemsFont = ScreenManager.Content.Load<SpriteFont>("MenuItemsFont");
                bgSong = ScreenManager.Content.Load<Song>("MainMenuMusic");
                logo = ScreenManager.Content.Load<Texture2D>("Logo2_03");

            }

                MediaPlayer.Play(bgSong);

            if (Config.ProfileList.Profiles.Count != 1)
                showSelectPlayerScreen = true;
                    

                gui = new GUI(ScreenManager.Input);

                var btn = new Button("Nowa gra");
                btn.BackColor = Color.Transparent;
                
                btn.FrontColor = Color.White * 0.75f;
                btn.HoverColor = Color.White;
                btn.TextFont = MenuItemsFont;
                btn.Position = new Vector2(ScreenManager.Viewport.Width - 300, ScreenManager.Viewport.Height - 150);
                btn.ClickAction += () => { ScreenManager.AddScreen(new PlayMenu());};
                gui.AddControl(btn);

                //btn = new Button("Opcje");
                //btn.BackColor = Color.Transparent;

                //btn.FrontColor = Color.White * 0.75f;
                //btn.HoverColor = Color.White;
                //btn.TextFont = MenuItemsFont;
                //btn.Position = new Vector2(ScreenManager.Viewport.Width - 300, ScreenManager.Viewport.Height - 200);
                //btn.ClickAction += () => { ScreenManager.AddScreen(new OptionMenuScreen()); };
                //btn.IsActive = false;
                //gui.AddControl(btn);

                //btn = new Button("Tablica wynikow");
                //btn.BackColor = Color.Transparent;

                //btn.FrontColor = Color.White * 0.75f;
                //btn.HoverColor = Color.White;
                //btn.TextFont = MenuItemsFont;
                //btn.Position = new Vector2(ScreenManager.Viewport.Width - 300, ScreenManager.Viewport.Height - 150);
                //btn.IsActive = false;
                //gui.AddControl(btn);


                btn = new Button("Koniec");
                btn.BackColor = Color.Transparent;
                btn.FrontColor = Color.White * 0.75f;
                btn.HoverColor = Color.White;
                btn.TextFont = MenuItemsFont;
                btn.Position = new Vector2(ScreenManager.Viewport.Width - 300, ScreenManager.Viewport.Height - 100);
                btn.ClickAction += () => { ScreenManager.Game.Exit(); };
                gui.AddControl(btn);

            
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            if (State == eScreenState.Active)
            {
                gui.Update(gameTime);

                //if (showSelectPlayerScreen == true)
                //{
                //    showSelectPlayerScreen = false;
                //    ScreenManager.AddScreen(new SelectPlayerScreen());
                //}
            }
        }
        

        public override void Draw(GameTime gameTime)
        {

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            Vector2 screenSize = new Vector2(spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height);
            Vector2 imageSize = new Vector2(logo.Width, logo.Height);

            float scale = (float)screenSize.X / (float)imageSize.X;
            scale *= 0.70f;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(logo, new Vector2(ScreenManager.Viewport.Width / 2f + ((float)System.Math.Sin(gameTime.TotalGameTime.TotalSeconds) * 3.0f), ScreenManager.Viewport.Height / 2f + ( (float)Math.Cos(gameTime.TotalGameTime.TotalSeconds) * 10.0f)), null, Color.White * TransitionAlpha, MathHelper.ToRadians((float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 0.25f) * 5.0f), logo.Center(), new Vector2(scale), SpriteEffects.None, 0);
            gui.Draw(spriteBatch, gameTime, TransitionAlpha);

            spriteBatch.DrawString(ScreenManager.DefaultFont, "Przemysław 'Osik' Osipiuk - 2017", new Vector2(20, ScreenManager.Viewport.Height - 50), Color.White * TransitionAlpha);

            spriteBatch.End();
        }

        public override void Unload()
        {
            bgSong = null;
        }
    }
}