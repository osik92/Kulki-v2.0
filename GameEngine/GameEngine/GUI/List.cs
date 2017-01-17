using System.Collections.Generic;
using GameEngine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.GUI
{
    public class List : Control
    {
        public Rectangle Bounds;
        private List<object> elementList = new List<object>();
        private SpriteBatch localSpriteBatch;
        private RenderTarget2D target;
        public override void Update(GameTime gameTime)
        {
            
        }
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime, float alpha)
        {
            if(localSpriteBatch == null)
                localSpriteBatch = new SpriteBatch(spriteBatch.GraphicsDevice);
            if(target == null)
                target = new RenderTarget2D(spriteBatch.GraphicsDevice, Bounds.Width, Bounds.Height);
            localSpriteBatch.GraphicsDevice.SetRenderTarget(target);
            localSpriteBatch.GraphicsDevice.Clear(Color.Transparent);
            localSpriteBatch.Begin();
            localSpriteBatch.Draw(spriteBatch.SolidTexture(), new Rectangle(0,0,Bounds.Width, Bounds.Height), null, Color.Black * 0.25f);
            localSpriteBatch.DrawString(TextFont, "test", Vector2.Zero, Color.White);
            localSpriteBatch.End();
            localSpriteBatch.GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Draw(target, Bounds, Color.White);
        }
        public override void OnHover()
        {
            
        }

        public void Add(object obj)
        {
            elementList.Add(obj);
        }

    }
}
