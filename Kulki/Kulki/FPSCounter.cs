using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine
{
    class FPSCounter : DrawableGameComponent
    {
        private ScreenManager manager;
        private SpriteFont font;

        private int totalFrames = 0;
        private float elapsedTime = 0.0f;
        private int fps = 0;

        public FPSCounter(Game game, ScreenManager manager) : base(game)
        {
            this.manager = manager;
        }

        protected override void LoadContent()
        {
            font = manager.Content.Load<SpriteFont>("MenuItemsFont");

        }

        public override void Update(GameTime gameTime)
        {
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (elapsedTime >= 1000.0f)
            {
                fps = totalFrames;
                totalFrames = 0;
                elapsedTime = 0;
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            totalFrames++;
            SpriteBatch sb = new SpriteBatch(GraphicsDevice);
            sb.Begin();
            sb.DrawString(font, String.Format("FPS: {0}",fps), Vector2.Zero, Color.Black);
            sb.DrawString(font, String.Format("FPS: {0}", fps), Vector2.One, Color.White);
            sb.End();
        }
    }
}
