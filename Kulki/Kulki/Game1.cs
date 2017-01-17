using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using GameEngine;
using Kulki.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace Kulki
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private ScreenManager screenManager;
        private bool musicPlay = true;

        [DllImport("user32.dll")]
        static extern void ClipCursor(ref Rectangle rect);

        private ProfileSaver profileSaver;
        public Game1()
        {
            profileSaver = new ProfileSaver("Kulki 2.0", "profile.prof");
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            var storage = Config.Storage;

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //TargetElapsedTime = TimeSpan.FromSeconds(1f / 60f);
            screenManager = new ScreenManager(this) { TraceEnabled = true };
            Components.Add(screenManager);
            Components.Add(new Cursor(this, screenManager));
            if (storage.Get<bool>("showFps"))
            {
                FPSCounter counter = new FPSCounter(this, screenManager);
                counter.Enabled();
                Components.Add(counter);
            }
            graphics.IsFullScreen = storage.Get<bool>("fullscreen");
            graphics.PreferredBackBufferWidth = storage.Get<int>("width");
            graphics.PreferredBackBufferHeight = storage.Get<int>("height");
            
            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;

            graphics.ApplyChanges();
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = storage.Get<float>("volume");


            AddInitialScreens();

        }

        private void AddInitialScreens()
        {
            screenManager.AddScreen(new SplashScreen());

            var list = profileSaver.Load();

            //var list2 = new List<PlayerNickIdValue>();
            //list2.Add(new PlayerNickIdValue() { Id = 0, Nick = "Osik" });
            //list = new ProfileList() {Profiles = list2};
            //profileSaver.Save(list);
            //Debug.WriteLine("Saved completed [in AddInitialScreens]");
            Debug.WriteLine("Saved profiles: {0}", list.Profiles.Count);
            foreach (var element in list.Profiles)
            {
                Debug.WriteLine("{0} - {1}", element.Id, element.Nick);
            }

            Config.ProfileList = list;

        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!IsActive)
                MediaPlayer.Pause();
            else
                MediaPlayer.Resume();

            if (graphics.IsFullScreen)
            {
                if (screenManager != null)
                {
                    if (Window != null && GraphicsDevice != null)
                    {
                        var rect = Window.ClientBounds;
                        var rect2 = GraphicsDevice.Viewport.Bounds;
                        rect.X += rect2.X;
                        rect.Y += rect2.Y;
                        rect.Width = rect2.Width + rect.X;
                        rect.Height = rect2.Height + rect.Y;
                        ClipCursor(ref rect);
                    }
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            Content.Unload();
        }
    }
}
