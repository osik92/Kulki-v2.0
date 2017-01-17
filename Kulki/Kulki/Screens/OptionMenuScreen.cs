using System;
using GameEngine.GUI;
using GameEngine.Screens;
using GameEngine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Kulki.Screens
{
    internal class OptionMenuScreen : GameScreen
    {
        private GUI gui;

        public OptionMenuScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5f);
            TransitionOffTime = TimeSpan.FromSeconds(0.5f);
        }

        public override void Activate(bool instancePreserved)
        {
            gui = new GUI(ScreenManager.Input);

            var btn = new Button("Powrot");
            btn.BackColor = Color.Transparent;

            btn.FrontColor = Color.White * 0.75f;
            btn.HoverColor = Color.White;
            btn.TextFont = ScreenManager.DefaultFont;
            btn.Position = new Vector2(ScreenManager.Viewport.Width - 300, ScreenManager.Viewport.Height - 100);
            btn.ClickAction += ExitScreen;
            gui.AddControl(btn);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            if (State == eScreenState.Active)
                gui.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.DrawString(ScreenManager.DefaultFont, "Jakis przykladowy tekst", Vector2.Zero, Color.White * TransitionAlpha);
            gui.Draw(spriteBatch, gameTime, TransitionAlpha);
            spriteBatch.End();
        }
    }
}