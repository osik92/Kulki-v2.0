using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Kulki
{
    public class Cursor : DrawableGameComponent
    {
        private ScreenManager manager;
        private Vector2 mousePosition = Vector2.Zero;
        private Texture2D cursor;
        public Cursor(Game game, ScreenManager manager) : base(game)
        {
            this.manager = manager;
        }

        protected override void LoadContent()
        {
            cursor = manager.Content.Load<Texture2D>("arrow");
        }

        public override void Update(GameTime gameTime)
        {
            mousePosition = manager.Input.GetMousePosition().ToVector2();
        }

        public override void Draw(GameTime gameTime)
        {
            int screenCount = manager.GetScreens().Length;
            if (manager.GetScreens()[screenCount - 1].CursorVisible)
            {
                SpriteBatch sb = manager.SpriteBatch;

                Vector2 screenSize = new Vector2(sb.GraphicsDevice.Viewport.Width, sb.GraphicsDevice.Viewport.Height);
                Vector2 imageSize = new Vector2(cursor.Width, cursor.Height);


                float scale = (float)screenSize.Y / (float)imageSize.Y;

                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                sb.Draw(cursor, mousePosition, null, Color.White, 0, Vector2.Zero, new Vector2(scale * 0.05f), SpriteEffects.None,
                    0);
                sb.End();
            }
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            cursor.Dispose();
        }
    }
}
