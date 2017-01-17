using System;
using System.Collections.Generic;
using GameEngine;
using GameEngine.GUI;
using GameEngine.Screens;
using Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Kulki.Screens
{


    internal class PlayMenu : GameScreen
    {
        private GUI gui;
        private int boardSize = 10;
        private int colors = 5;
        
        private TimeSpan colorChangeTimer = TimeSpan.FromSeconds(1.5f);
        private int actColor = 0;
        private int selectedBall = 0;

        private List<ImageButton> ballsButton = new List<ImageButton>();
        

        private Texture2D balls;
        private Texture2D frame;
        public PlayMenu()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5f);
            TransitionOffTime = TimeSpan.FromSeconds(0.5f);
        }
        public override void Activate(bool instancePreserved)
        {
            frame = ScreenManager.Content.Load<Texture2D>("frame");
            balls = ScreenManager.Content.Load<Texture2D>("ball2");

            var storage = Config.Storage;
            boardSize = storage.Get<int>("boardSize");
            colors = storage.Get<int>("colors");
            selectedBall = MathHelper.Clamp(storage.Get<int>("ballStyle"), 0, 3);

            gui = new GUI(ScreenManager.Input);

            var btn = new Button("Uruchom grê");
            btn.BackColor = Color.Transparent;

            btn.FrontColor = Color.White * 0.75f;
            btn.HoverColor = Color.White;
            btn.TextFont = ScreenManager.DefaultFont;
            btn.Position = new Vector2(ScreenManager.Viewport.Width - 300, ScreenManager.Viewport.Height - 150);
            btn.ClickAction += () =>
            {
                SaveStorage(storage);
                LoadingScreen.Load(ScreenManager, true, new PlayScreen());
            };
            gui.AddControl(btn);

            btn = new Button("<<");
            btn.BackColor = Color.Transparent;

            btn.FrontColor = Color.White * 0.75f;
            btn.HoverColor = Color.White;
            btn.TextFont = ScreenManager.DefaultFont;
            btn.Position = new Vector2(400, 50);
            btn.ClickAction += () => { boardSize = MathHelper.Clamp(--boardSize, 10, 20); };
            gui.AddControl(btn);

            btn = new Button(">>");
            btn.BackColor = Color.Transparent;

            btn.FrontColor = Color.White * 0.75f;
            btn.HoverColor = Color.White;
            btn.TextFont = ScreenManager.DefaultFont;
            btn.Position = new Vector2(600, 50);
            btn.ClickAction += () => { boardSize = MathHelper.Clamp(++boardSize, 10, 20); };
            gui.AddControl(btn);

            btn = new Button("<<");
            btn.BackColor = Color.Transparent;

            btn.FrontColor = Color.White * 0.75f;
            btn.HoverColor = Color.White;
            btn.TextFont = ScreenManager.DefaultFont;
            btn.Position = new Vector2(400, 100);
            btn.ClickAction += () => { colors = MathHelper.Clamp(--colors, 5, 11); };
            gui.AddControl(btn);

            btn = new Button(">>");
            btn.BackColor = Color.Transparent;

            btn.FrontColor = Color.White * 0.75f;
            btn.HoverColor = Color.White;
            btn.TextFont = ScreenManager.DefaultFont;
            btn.Position = new Vector2(600, 100);
            btn.ClickAction += () => { colors = MathHelper.Clamp(++colors, 5, 11); };
            gui.AddControl(btn);

            btn = new Button("Powrot");
            btn.BackColor = Color.Transparent;

            btn.FrontColor = Color.White * 0.75f;
            btn.HoverColor = Color.White;
            btn.TextFont = ScreenManager.DefaultFont;
            btn.Position = new Vector2(ScreenManager.Viewport.Width - 300, ScreenManager.Viewport.Height - 100);
            btn.ClickAction += () =>
            {
                SaveStorage(storage);
                ExitScreen();
            };
            gui.AddControl(btn);


            var img = new Image(balls, new Rectangle(0, 0, balls.Width/4, balls.Height));
            img.Size = new Vector2(50,50);

            var imgBtn = new ImageButton(img);
            imgBtn.Position = new Vector2(400,175);
            imgBtn.BackColor = Color.Transparent;

            imgBtn.FrontColor = Color.White;
            imgBtn.BackColor = Color.Transparent;
            imgBtn.ClickAction += () => { selectedBall = 0; };

            ballsButton.Add(imgBtn);
            img = new Image(balls, new Rectangle(balls.Width / 4, 0, balls.Width / 4, balls.Height));
            img.Size = new Vector2(50, 50);

            imgBtn = new ImageButton(img);
            imgBtn.Position = new Vector2(460, 175);
            imgBtn.BackColor = Color.Transparent;

            imgBtn.FrontColor = Color.White;
            imgBtn.BackColor = Color.Transparent;
            imgBtn.ClickAction += () => { selectedBall = 1; };
            ballsButton.Add(imgBtn);

            img = new Image(balls, new Rectangle((balls.Width / 4) * 2, 0, balls.Width / 4, balls.Height));
            img.Size = new Vector2(50, 50);

            imgBtn = new ImageButton(img);
            imgBtn.Position = new Vector2(535, 175);
            imgBtn.BackColor = Color.Transparent;

            imgBtn.FrontColor = Color.White;
            imgBtn.BackColor = Color.Transparent;
            imgBtn.ClickAction += () => { selectedBall = 2; };
            ballsButton.Add(imgBtn);

            img = new Image(balls, new Rectangle((balls.Width / 4) * 3, 0, balls.Width / 4, balls.Height));
            img.Size = new Vector2(50, 50);

            imgBtn = new ImageButton(img);
            imgBtn.Position = new Vector2(600, 175);
            imgBtn.BackColor = Color.Transparent;

            imgBtn.FrontColor = Color.White;
            imgBtn.BackColor = Color.Transparent;
            imgBtn.ClickAction += () => { selectedBall = 3; };
            ballsButton.Add(imgBtn);

            foreach (ImageButton imageButton in ballsButton)
            {
                gui.AddControl(imageButton);
            }

        }

        private void SaveStorage(Storage storage)
        {
            storage.Set("boardSize", boardSize);
            storage.Set("colors", colors);
            storage.Set("ballStyle", MathHelper.Clamp(selectedBall, 0, 3));
            storage.Save("config.conf");
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            if (State == eScreenState.Active)
            {
                gui.Update(gameTime);
                colorChangeTimer -= gameTime.ElapsedGameTime;
                if (colorChangeTimer <= TimeSpan.Zero)
                {
                    colorChangeTimer = TimeSpan.FromSeconds(1.5f);
                    actColor++;
                    if (actColor >= colors)
                        actColor = 0;
                }
            }
            for (int i = 0; i < ballsButton.Count; ++i)
            {
                ballsButton[i].FrontColor = BallsColors.BallColor(actColor);
                if (i == selectedBall)
                    ballsButton[i].BackColor = Color.White * 0.75f;
                else
                    ballsButton[i].BackColor = Color.Transparent;
            }

        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            spriteBatch.DrawString(ScreenManager.DefaultFont, "Wielkoœæ planszy", new Vector2(100, 50), Color.White * TransitionAlpha);
            spriteBatch.DrawString(ScreenManager.DefaultFont, String.Format("{0} x {0}", boardSize), new Vector2(480, 50), Color.White * TransitionAlpha);
            spriteBatch.DrawString(ScreenManager.DefaultFont, "Iloœæ kolorów", new Vector2(100, 100), Color.White * TransitionAlpha);
            spriteBatch.DrawString(ScreenManager.DefaultFont, String.Format("{0}", colors), new Vector2(510, 100), Color.White * TransitionAlpha);

            spriteBatch.DrawString(ScreenManager.DefaultFont, "Wygl¹d kulek", new Vector2(100, 180), Color.White * TransitionAlpha);
            //spriteBatch.Draw(balls, new Rectangle(425,200,50,50), new Rectangle(0,0,balls.Width/4, balls.Height), ballsColors[actColor] * TransitionAlpha, 0, new Vector2(balls.Width / 8, balls.Height/2), SpriteEffects.None, 0 );
            //spriteBatch.Draw(balls, new Rectangle(490, 200, 50, 50), new Rectangle(balls.Width / 4, 0, balls.Width / 4, balls.Height), ballsColors[actColor] * TransitionAlpha, 0, new Vector2(balls.Width / 8, balls.Height / 2), SpriteEffects.None, 0);
            //spriteBatch.Draw(balls, new Rectangle(560, 200, 50, 50), new Rectangle((balls.Width / 4) * 2, 0, balls.Width / 4, balls.Height), ballsColors[actColor] * TransitionAlpha, 0, new Vector2(balls.Width / 8, balls.Height / 2), SpriteEffects.None, 0);
            //spriteBatch.Draw(balls, new Rectangle(625, 200, 50, 50), new Rectangle((balls.Width / 4) * 3, 0, balls.Width / 4, balls.Height), ballsColors[actColor] * TransitionAlpha, 0, new Vector2(balls.Width / 8, balls.Height / 2), SpriteEffects.None, 0);

            gui.Draw(spriteBatch, gameTime, TransitionAlpha);
            spriteBatch.End();
        }

        public override void Unload()
        {
            balls = null;
            frame = null;
        }
    }
}