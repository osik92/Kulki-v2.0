using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Utilities
{
    public static class RectangleExtension
    {
        public static Rectangle CreateRectangle(Vector2 position, Vector2 size)
        {
            return new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int) size.Y);
        }

        public static Point ToPoint(this Vector2 vector)
        {
            return new Point((int)vector.X, (int)vector.Y);
        }

        private static Texture2D solidTexture;

        public static Texture2D SolidTexture(this SpriteBatch spriteBatch)
        {
            if (solidTexture == null)
            {
                solidTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                solidTexture.SetData(new Color[] {Color.White});
            }
            return solidTexture;
        }

        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 startPosition, Vector2 endPosition, Color color, int lineWidth = 1)
        {
            Vector2 edge = endPosition - startPosition;
            float angle = (float)System.Math.Atan2(edge.Y, edge.X);
            
            spriteBatch.Draw(spriteBatch.SolidTexture(), RectangleExtension.CreateRectangle(startPosition, new Vector2(edge.Length(), lineWidth)), null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
        }

        public static Vector2 Center(this Texture2D texture)
        {
            return new Vector2(texture.Width / 2f, texture.Height / 2f);
        }

        public static Vector2 ToVectorAndScale(this Rectangle rect, Vector2 oryginalSize, out Vector2 scale)
        {
            scale = rect.Size.ToVector2()/oryginalSize;
            return rect.Location.ToVector2();
        }

        public static Rectangle ToRectangle(this Vector2 position, Vector2 oryginalSize, Vector2 scale)
        {
            return new Rectangle(position.ToPoint(), new Point((int)(oryginalSize.X * scale.X), (int)(oryginalSize.Y * scale.Y)));
        }

        public static Vector2 Scale(this Vector2 currentSize, Vector2 oryginalSize)
        {
            return currentSize / oryginalSize;
        }
    }
}
