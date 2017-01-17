using System;
using GameEngine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Math = System.Math;
using EngineMath = GameEngine.Utilities.Math;

namespace GameEngine.GUI
{
    public abstract class Control
    {
        public Color HoverColor;
        public SpriteFont TextFont;
        public bool IsActive { get; set; }

        public Rectangle Bounds { get; protected set; }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime, float alpha);

        public abstract void OnHover();

    }

    public class Button : Control
    {
        public Color BackColor { get { return bc; } set { bc = backColor = value; } }
        public Color FrontColor { get { return fc; } set { fc = frontColor = value; } }

        protected Color bc;
        protected Color fc;

        protected Color backColor;
        protected Color frontColor;
        protected Vector2 position;
        public string Text;
        protected bool isHover = false;
        protected float scale = 1.0f;

        public delegate void Click();

        public Click ClickAction;
        public Button(string text)
        {
            IsActive = true;
            this.Text = text;
            IsActive = true;
        }
        public Vector2 Position
        {
            get { return position; }
            set
            {
                this.position = value;

                var textSize = TextFont.MeasureString(Text);
                this.Bounds = new Rectangle(position.ToPoint(), new Point((int)textSize.X + 20, (int)textSize.Y + 10));
            }
        }

        public override void Update(GameTime gameTime)
        {
            frontColor = isHover ? HoverColor : FrontColor;
            scale = isHover ? (float)(1.0f + 0.15 * Math.Abs(Math.Sin((double)gameTime.TotalGameTime.TotalSeconds * 2))) : 1.0f;

            backColor = BackColor;
            isHover = false;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime, float alpha)
        {
            var rescaleRect = new Rectangle(Bounds.Location, new Point((int)(Bounds.Width * scale), (int)(Bounds.Height * scale)));

            spriteBatch.Draw(spriteBatch.SolidTexture(), rescaleRect, backColor * alpha);
            
            spriteBatch.DrawString(TextFont, Text, Position + new Vector2(10, 5) + new Vector2( 1, 1), Color.Black * alpha, 0, Vector2.Zero, new Vector2(scale), SpriteEffects.None, 0);
            spriteBatch.DrawString(TextFont, Text, Position + new Vector2(10, 5) + new Vector2( 1,-1), Color.Black * alpha, 0, Vector2.Zero, new Vector2(scale), SpriteEffects.None, 0);
            spriteBatch.DrawString(TextFont, Text, Position + new Vector2(10, 5) + new Vector2(-1,-1), Color.Black * alpha, 0, Vector2.Zero, new Vector2(scale), SpriteEffects.None, 0);
            spriteBatch.DrawString(TextFont, Text, Position + new Vector2(10, 5) + new Vector2(-1, 1), Color.Black * alpha, 0, Vector2.Zero, new Vector2(scale), SpriteEffects.None, 0);
            spriteBatch.DrawString(TextFont, Text, Position + new Vector2(10, 5), frontColor * alpha, 0, Vector2.Zero, new Vector2(scale), SpriteEffects.None, 0);
        }

        public override void OnHover()
        {
            if (IsActive)
                isHover = true;
        }

        public void OnClick()
        {
            if (IsActive)
                ClickAction();
        }
    }

    public class ImageButton : Button
    {
        private Image image;
        public ImageButton(Image image) : base(null)
        {
            this.image = image;
        }

        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                this.Bounds = new Rectangle(position.ToPoint(), image.Size.ToPoint());
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime, float alpha)
        {
            var rescaleRect = new Rectangle(Bounds.Location, new Point((int)(Bounds.Width * scale), (int)(Bounds.Height * scale)));


            spriteBatch.DrawLine(rescaleRect.Location.ToVector2(), new Vector2(rescaleRect.Right, rescaleRect.Top), BackColor * alpha, 1);
            spriteBatch.DrawLine(rescaleRect.Location.ToVector2(), new Vector2(rescaleRect.Left, rescaleRect.Bottom), BackColor * alpha, 1);
            spriteBatch.DrawLine(new Vector2(rescaleRect.Right, rescaleRect.Top), (rescaleRect.Location + rescaleRect.Size).ToVector2(), BackColor * alpha, 1);
            spriteBatch.DrawLine(new Vector2(rescaleRect.Left, rescaleRect.Bottom), (rescaleRect.Location + rescaleRect.Size).ToVector2(), BackColor * alpha, 1);

            //spriteBatch.Draw(spriteBatch.SolidTexture(), rescaleRect, backColor * alpha);
            spriteBatch.Draw(image.Texture, new Rectangle(Position.ToPoint(), new Vector2(Bounds.Width * scale, Bounds.Height * scale).ToPoint()), image.SourceRectangle, frontColor * alpha);
            //spriteBatch.DrawString(TextFont, Text, Position + new Vector2(10, 5), frontColor * alpha, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
        }

        public override void Update(GameTime gameTime)
        {
            backColor = isHover ? HoverColor : BackColor;
            scale = isHover ? (float)(1.0f + 0.15 * Math.Abs(Math.Sin((double)gameTime.TotalGameTime.TotalSeconds * 2))) : 1.0f;

            frontColor = FrontColor;
            isHover = false;
        }
    }
}