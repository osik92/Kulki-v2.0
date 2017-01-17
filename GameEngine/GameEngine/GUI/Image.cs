using System;
using System.Diagnostics;
using GameEngine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.GUI
{
    public class AnimationImage
    {
        private Texture2D texture;
        private Point frameCount;
        private readonly int animFrame;
        private int currentFrame = -1;
        private bool loop;
        private TimeSpan frameTime;
        private Vector2 frameSize;
        public AnimationImage(Texture2D texture, Point frameCount, int animFrames, bool loop = true)
        {
            this.animFrame = animFrames;
            this.texture = texture;
            this.frameCount = frameCount;
            this.loop = loop;
            frameSize = new Vector2(texture.Width / frameCount.X, texture.Height / frameCount.Y);

        }

        public TimeSpan AnimationTime{ get; set; }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Rectangle target, Color color)
        {
            frameTime -= gameTime.ElapsedGameTime;
            if (frameTime <= TimeSpan.Zero)
            {
                frameTime = TimeSpan.FromSeconds(AnimationTime.TotalSeconds / animFrame);
                if (currentFrame++ >= animFrame)
                {
                    currentFrame = loop ? 0 : animFrame - 1;
                }
            }


            var x = currentFrame%11;
            var y = currentFrame/11;


            Rectangle sourceRectangle = new Rectangle((int)(x* frameSize.X), (int)(y* frameSize.Y), (int)frameSize.X, (int)frameSize.Y);
            Debug.WriteLine(sourceRectangle);
            spriteBatch.Draw(texture, target.Location.ToVector2(), sourceRectangle, color, 0, frameSize / 2f, target.Size.ToVector2().Scale(frameSize), SpriteEffects.None, 0);

        }
    }

    public class Image
    {
        private Texture2D texture;
        private Rectangle? sourceRectangle;
        private Vector2 imageSize;

        public Image(Texture2D texture, Rectangle? sourceRectangle = null)
        {
            this.texture = texture;
            this.sourceRectangle = sourceRectangle;
            imageSize = this.sourceRectangle != null ? sourceRectangle.Value.Size.ToVector2() : new Vector2(texture.Width, texture.Height);
        }

        public Rectangle SourceRectangle => sourceRectangle.Value;

        public Vector2 Size
        {
            get { return imageSize; }
            set { imageSize = value; }
        }

        public Vector2 Center => imageSize/2f;

        public Texture2D Texture => texture;
    }
}