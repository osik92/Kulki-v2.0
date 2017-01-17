using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace GameEngine.Screens
{
    public abstract class GameScreen
    {
        private bool otherScreenHasFocus;

        public eScreenState State
        {
            get;
            protected set;
        }

        public virtual void Draw(GameTime gameTime) { }
        public bool CursorVisible { get; protected set; }

        public virtual void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            this.otherScreenHasFocus = otherScreenHasFocus;
            if (this.IsExiting)
            {
                this.State = eScreenState.TransitionOff;
                if (!this.UpdateTransition(gameTime, this.TransitionOffTime, 1))
                    this.ScreenManager.RemoveScreen(this);
            }
            else if (coveredByOtherScreen)
            {
                this.State = UpdateTransition(gameTime, this.TransitionOffTime, 1)
                    ? eScreenState.TransitionOff
                    : eScreenState.Hidden;
            }
            else
            {
                this.State = this.UpdateTransition(gameTime, this.TransitionOnTime, -1) 
                    ? eScreenState.TransitionOn 
                    : eScreenState.Active;
            }
            
        }

        private bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            float transitionDelta = (time == TimeSpan.Zero) ? 1 : (float)(gameTime.ElapsedGameTime.TotalMilliseconds / time.TotalMilliseconds);
            this.TransitionPosition += transitionDelta*direction;

            if (((direction < 0) && (this.TransitionPosition <= 0)) ||
                ((direction > 0) && (this.TransitionPosition >= 1)))
            {
                this.TransitionPosition = MathHelper.Clamp(this.TransitionPosition, 0, 1);
                return false;
            }
            return true;

        }

        public ScreenManager ScreenManager { protected get; set; }
        public bool IsPopup { get; protected set; }

        public bool IsExiting { get; private set; }

        public TimeSpan TransitionOnTime { private get; set; }
        public TimeSpan TransitionOffTime { private get; set; }

        public float TransitionPosition {   get; protected set; }

        public float TransitionAlpha { get { return 1f - this.TransitionPosition; } }
        protected GameScreen()
        {
            this.IsExiting = false;
            this.CursorVisible = true;
            this.TransitionOnTime = TimeSpan.Zero;
            this.TransitionOffTime = TimeSpan.Zero;
            this.TransitionPosition = 1.0f;
            this.State = eScreenState.TransitionOn;
        }

        public bool IsActive
        {
            get
            {
                return !this.otherScreenHasFocus &&
                       (this.State == eScreenState.TransitionOn ||
                        this.State == eScreenState.Active);
            }
        }
        

        public virtual void HandleInput(GameTime gameTime, InputHandler input)  {}

        public virtual void Unload() {}
        public virtual void Activate(bool instancePreserved) { }
        public virtual void Deactivate() { }

        public void ExitScreen()
        {
            if (this.TransitionOffTime == TimeSpan.Zero)
                ScreenManager.RemoveScreen(this);
            else
                IsExiting = true;
        }
    }
}