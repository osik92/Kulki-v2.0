using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameEngine.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine
{
    public class ScreenManager : DrawableGameComponent
    {
        private List<GameScreen> screens;
        private InputHandler input;
        private bool isInitialized;
        public SpriteFont DefaultFont;
        private bool instancePreserfed = true;

        public InputHandler Input {  get { return input; } }

        public SpriteBatch SpriteBatch { get; private set; }
        private Texture2D blankTexture;

        bool traceEnabled;
        public bool TraceEnabled
        {
            get { return traceEnabled; }
            set { traceEnabled = value; }
        }
        public override void Update(GameTime gameTime)
        {
            input.Update();
            List<GameScreen> tempScreens = new List<GameScreen>(screens);
            tempScreens.Reverse();

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;

            while (tempScreens.Count > 0)
            {
                GameScreen screen = tempScreens[0];
                tempScreens.RemoveAt(0);

                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.State == eScreenState.Active || screen.State == eScreenState.TransitionOn)
                {
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput(gameTime, input);
                        otherScreenHasFocus = true;
                    }
                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }
            }
            tempScreens.Clear();
            if (traceEnabled)
                TraceScreens();
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (GameScreen screen in screens)
            {
                if(screen.State == eScreenState.Hidden)
                    continue;
                screen.Draw(gameTime);
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            isInitialized = true;
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            blankTexture = Content.Load<Texture2D>("blank");
            DefaultFont = Content.Load<SpriteFont>("MenuItemsFont");
            foreach (GameScreen screen in screens)
            {
                screen.Activate(false);
            }
        }

        protected override void UnloadContent()
        {
            foreach (GameScreen screen in screens)
            {
                screen.Unload();
            }
            screens.Clear();
        }

        public void AddScreen(GameScreen screen)
        {
            screen.ScreenManager = this;
            screen.Activate(instancePreserfed);
            screens.Add(screen);
            instancePreserfed = false;
        }

        public Viewport Viewport => SpriteBatch.GraphicsDevice.Viewport;

        public void RemoveScreen(GameScreen screen)
        {
            if (screens.Contains(screen))
            {
                screen.Unload();
                screens.Remove(screen);
            }
        }
        public GameScreen[] GetScreens()
        {
            return screens.ToArray();
        }

        public ContentManager Content;

        public ScreenManager(Game game) : base(game)
        {
            screens = new List<GameScreen>();
            input = new InputHandler();
            Content = game.Content;
        }

        public void FadeBackBufferToBlack(float alpha)
        {
            SpriteBatch.Begin();
            SpriteBatch.Draw(blankTexture, GraphicsDevice.Viewport.Bounds, Color.Black * alpha);
            SpriteBatch.End();
        }

        void TraceScreens()
        {
            List<string> screenNames = new List<string>();

            foreach (GameScreen screen in screens)
                screenNames.Add(screen.GetType().Name + " " + Enum.GetName(typeof(eScreenState), screen.State));

            Debug.WriteLine(string.Join(", ", screenNames.ToArray()));
        }
    }
}