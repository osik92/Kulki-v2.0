using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameEngine;
using GameEngine.GUI;
using GameEngine.Input;
using GameEngine.Utilities;
using Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EngineMath = GameEngine.Utilities.Math;

namespace Kulki
{

    class GameBoard
    {
        class Ball
        {
            GameBoard board;
            private Vector2 Size;
            public Vector2 Position;
            private  Color BallColor;
            private Vector2 Scale = Vector2.One;

            private AnimationImage explossion;


            public delegate void Die();

            public Die ImDie;

            private enum eBallAnimationState
            {
                unselected,
                select,
                selected,
                move,
                unselect,
                die
            }

            private eBallAnimationState animationState = eBallAnimationState.unselected;
            private TimeSpan animationTime = TimeSpan.Zero;
            
            public Ball(GameBoard owner, Color color)
            {
                this.board = owner;
                BallColor = color;

                explossion = new AnimationImage(board.Explossion, new Point(11,1), 11, false);

            }

            public void Kill(TimeSpan animTime)
            {
                animationState = eBallAnimationState.die;
                animationTime = animTime;
                explossion.AnimationTime = animationTime;
            }

            public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
            {

                Vector2 ballPosition = new Vector2(this.board.boardRectangle.Left + Size.X*Position.X + 0.5f * Size.X,
                    this.board.boardRectangle.Top + Size.Y*Position.Y + 0.5f * Size.Y);

                if (animationState == eBallAnimationState.die)
                {
                    explossion.Draw(spriteBatch, gameTime, new Rectangle(ballPosition.ToPoint(), this.board.CellSize.ToPoint()),BallColor );
                }
                else
                {
                    spriteBatch.Draw(this.board.Image.Texture, ballPosition, this.board.Image.SourceRectangle, BallColor, 0f,
                        this.board.Image.Center, this.board.CellSize.Scale(this.board.Image.Size) * Scale, SpriteEffects.None, 0);
                }

                
            }

            public void Update(GameTime gameTime)
            {
                
                if (animationState != eBallAnimationState.unselected)
                {
                    animationTime -= gameTime.ElapsedGameTime;

                    switch (animationState)
                    {
                        case eBallAnimationState.select:
                            if (animationTime <= TimeSpan.Zero)
                            {
                                animationTime = TimeSpan.Zero;
                                animationState = eBallAnimationState.selected;
                            }
                            else
                            {
                                Scale =
                                    new Vector2(EngineMath.Map((float)animationTime.TotalSeconds, (float)TimeSpan.Zero.TotalSeconds,
                                        (float)TimeSpan.FromSeconds(0.2f).TotalSeconds, 1.2f, 1.0f));
                            }

                            break;

                        case eBallAnimationState.unselect:
                            if (animationTime <= TimeSpan.Zero)
                            {
                                animationTime = TimeSpan.Zero;
                                animationState = eBallAnimationState.unselected;
                            }
                            else
                            {
                                Scale =
                                    new Vector2(EngineMath.Map((float)animationTime.TotalSeconds, (float)TimeSpan.Zero.TotalSeconds,
                                        (float)TimeSpan.FromSeconds(0.2f).TotalSeconds, 1.0f, 1.2f));
                            }
                            break;
                            case eBallAnimationState.die:
                            
                                if (animationTime <= TimeSpan.Zero)
                                    if(ImDie != null)
                                        ImDie();
                            break;
                    }

                }

                Size = this.board.CellSize;
            }

            public void Select()
            {
                animationTime = TimeSpan.FromSeconds(0.2f);
                animationState = eBallAnimationState.select;
                Debug.WriteLine("Select ball");
            }

            public void Unselect()
            {
                animationTime = TimeSpan.FromSeconds(0.2f);
                animationState = eBallAnimationState.unselect;
                Debug.WriteLine("Unelect ball");
            }
        }

        #region BoardConstructing
        public class Factory
        {
            private int width;
            private int height;
            private int colors;
            private InputHandler input;
            private Image ballImage;
            private Image image;
            private Texture2D texture;


            public Factory Width(int width)
            {
                this.width = width;
                return this;
            }
            public Factory Height(int height)
            {
                this.height = height;
                return this;
            }
            public Factory Colors(int colors)
            {
                this.colors = colors;
                return this;
            }

            public Factory Input(InputHandler input)
            {
                this.input = input;
                return this;
            }

            public Factory Image(Image image)
            {
                this.image = image;
                return this;
            }

            public Factory SetExplossionTexture(Texture2D texture)
            {
                this.texture = texture;
                return this;
            }

            public GameBoard Create()
            {
                GameBoard board = new GameBoard(width, height, colors)
                {
                    Input = input,
                    Image = image,
                    Explossion = texture
                };
                return board;
            }
        }

        

        #endregion

        private enum eBoardState
        {
            selectBall,
            noSelectBall,
            moveBall,
            boardFull
        }

        private Board board;
        private int width, height;
        private eBoardState state = eBoardState.noSelectBall;
        private TimeSpan moveAnimation;
        private TimeSpan gameOverRemovePauseTime = TimeSpan.Zero;
        private int killBallIndex = -1;
        public delegate void NewPoints(int points);

        private eColor[] nextColors = new[] {eColor.White, eColor.White, eColor.White };

        private Random random = new Random();

        private int combo = 1;

        public delegate void BoardFull();

        public delegate void ComboPointsIncrease(int combo);

        public ComboPointsIncrease ComboIncreased;

        public NewPoints AddPoints;
        public BoardFull GameOver;

        private Rectangle boardRectangle;
        protected Image Image { get; set; }
        protected Texture2D Explossion { get; set; }
        public Rectangle BoardArea
        {
            get { return boardRectangle; }
            set
            {
                boardRectangle = value; 
                cellSize = new Vector2(boardRectangle.Width / (float)width, boardRectangle.Height / (float)height);
            }
        }

        private Vector2 cellSize;

        public Vector2 CellSize
        {
            get { return cellSize; }
        }
        private int colors;
        protected InputHandler Input;
        private bool hoverBoard;
        private Point selectedBall;

        private List<Ball> balls;

        private GameBoard(int width, int height, int colors)
        {
            board = new Board(width, height, colors);
            this.width = width;
            this.height = height;
            this.colors = colors;

            balls = new List<Ball>();
        }

        public void InitBoard()
        {
            for (int i = 0; i < 5; i++)
            {
                var createdBall = board.RandomBall((eColor) random.Next(this.colors));
                Ball b = new Ball(this, BallsColors.BallColor(createdBall.color));
                b.Position = new Vector2(createdBall.Position.X, createdBall.Position.Y);
                b.ImDie += delegate { balls.Remove(b); };
                balls.Add(b);
            }

            for (int i = 0; i < 3; i++)
            {
                nextColors[i] = (eColor) random.Next(this.colors);
            }
        }

        public eColor[] NextColors
        {
            get { return nextColors; }
        }

        public void Update(GameTime gameTime)
        {
            
            hoverBoard = boardRectangle.Contains(Input.GetMousePosition());

            for (int i = 0; i < balls.Count; i++)
            {
                balls[i].Update(gameTime);
            }

            if (state == eBoardState.boardFull)
            {
                gameOverRemovePauseTime -= gameTime.ElapsedGameTime;
                if (gameOverRemovePauseTime <= TimeSpan.Zero)
                {
                    if (killBallIndex >= 0 && balls.Count > 0)
                    {
                        gameOverRemovePauseTime = TimeSpan.FromSeconds(0.05f);
                        balls[killBallIndex].Kill(TimeSpan.FromSeconds(0.35f));
                        killBallIndex --;
                    }
                }
            }


            if (hoverBoard && state != eBoardState.boardFull)
            {
                var cellX = EngineMath.Map(Input.GetMousePosition().X, boardRectangle.Location.X,
                        boardRectangle.Location.X + boardRectangle.Width, 0, (float)width);

                    var cellY = EngineMath.Map(Input.GetMousePosition().Y, boardRectangle.Location.Y,
                        boardRectangle.Location.Y + boardRectangle.Height, 0, (float)height);

                    HoveredCell = new Vector2(cellX, cellY).ToPoint();

                if (Input.IsMouseButtonDown(eMouseButton.Left))
                {
                    switch (state)
                    {
                        case eBoardState.noSelectBall:

                            if (!board.GameBoard[(int)cellX, (int)cellY].IsEmpty)
                            {
                                selectedBall = new Vector2(cellX, cellY).ToPoint();
                                state = eBoardState.selectBall;

                                int index = balls.FindIndex(item => item.Position.ToPoint().Equals(selectedBall));
                                var ball = balls[index];
                                balls.RemoveAt(index);
                                ball.Select();
                                balls.Add(ball);

                            }
                        break;
                        case eBoardState.selectBall:
                            if (selectedBall.Equals(new Vector2(cellX, cellY).ToPoint()))
                            {
                                int index = balls.FindIndex(item => item.Position.ToPoint().Equals(selectedBall));
                                balls[index].Unselect();
                                selectedBall = new Point(-1, -1);
                                state = eBoardState.noSelectBall;

                            }
                            else if (board.GameBoard[(int) cellX, (int) cellY].IsEmpty)
                            {
                                if (board.MoveBall(board.GameBoard[selectedBall.X, selectedBall.Y].GetBall().Position,
                                    new Position((int) cellX, (int) cellY)))
                                {
                                    int index = balls.FindIndex(item => item.Position.ToPoint().Equals(selectedBall));
                                    var ball = balls[index];
                                    ball.Position = HoveredCell.ToVector2();
                                    ball.Unselect();
                                    state = eBoardState.moveBall;
                                    moveAnimation = TimeSpan.Zero;
                                }
                            }
                            else
                            {
                                int index = balls.FindIndex(item => item.Position.ToPoint().Equals(selectedBall));
                                balls[index].Unselect();
                                selectedBall = new Vector2(cellX, cellY).ToPoint();
                                index = balls.FindIndex(item => item.Position.ToPoint().Equals(selectedBall));
                                var ball = balls[index];
                                balls.RemoveAt(index);
                                ball.Select();
                                balls.Add(ball);
                            }
                        break;
                    }
                }

                if (state == eBoardState.moveBall)
                {
                    moveAnimation -= gameTime.ElapsedGameTime;
                    if (moveAnimation <= TimeSpan.Zero)
                    {
                        int points = board.RemovePointBallsAndGetPoints();
                        if (points != 0 && AddPoints != null)
                        {
                            AddPoints(points * combo);
                            if (combo > 1)
                            {
                                ComboIncreased(combo);
                            }
                            combo++;
                            
                            foreach (Ball ball in balls)
                            {
                                var temp = ball;
                                Point ballPos = temp.Position.ToPoint();

                                if (board.GameBoard[ballPos.X, ballPos.Y].IsEmpty)
                                {
                                    ball.Kill(TimeSpan.FromSeconds(0.75f));
                                }
                            }                            
                        }
                        else
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                var createdBall = board.RandomBall(nextColors[i]);
                                Ball b = new Ball(this, BallsColors.BallColor(createdBall.color));
                                b.Position = new Vector2(createdBall.Position.X, createdBall.Position.Y);
                                b.ImDie += delegate { balls.Remove(b); };
                                balls.Add(b);


                                points = board.RemovePointBallsAndGetPoints();
                                if (points != 0 && AddPoints != null)
                                {

                                    AddPoints(points * combo);
                                    if (combo > 1)
                                    {
                                        ComboIncreased(combo);
                                    }
                                    combo++;
                                    foreach (Ball ball in balls)
                                    {
                                        var temp = ball;
                                        Point ballPos = temp.Position.ToPoint();

                                        if (board.GameBoard[ballPos.X, ballPos.Y].IsEmpty)
                                        {
                                            ball.Kill(TimeSpan.FromSeconds(0.75f));
                                        }
                                    }
                                }
                                
                                if (board.GameOver)
                                {
                                    state = eBoardState.boardFull;
                                    selectedBall = new Point(-1, -1);
                                    killBallIndex = balls.Count - 1;

                                    balls.Sort( (b2, b1)  => (b1.Position.Y * board.Width + b1.Position.X).CompareTo(b2.Position.Y * board.Width + b2.Position.X));

                                    if (GameOver != null)
                                        GameOver();
                                    return;
                                }

                                nextColors[i] = (eColor) random.Next(this.colors);
                            }
                            combo = 1;

                        }
                        selectedBall = new Point(-1, -1);
                        state = eBoardState.noSelectBall;
                    }
                }
            }
        }

        private Point HoveredCell { get; set; }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, float alpha)
        {
            spriteBatch.Draw(spriteBatch.SolidTexture(), boardRectangle, null, Color.Black * 0.75f * alpha);


            if (hoverBoard && state != eBoardState.boardFull)
                spriteBatch.Draw(spriteBatch.SolidTexture(), new Rectangle(new Point((int)(boardRectangle.X + HoveredCell.X * cellSize.X), (int)(boardRectangle.Y + HoveredCell.Y * cellSize.Y)), cellSize.ToPoint()), null, Color.White * 0.25f * alpha);
            if (state == eBoardState.selectBall)
                spriteBatch.Draw(spriteBatch.SolidTexture(), new Rectangle(new Point((int)(boardRectangle.X + selectedBall.X * cellSize.X), (int)(boardRectangle.Y + selectedBall.Y * cellSize.Y)), cellSize.ToPoint()), null, Color.White * 0.75f * alpha);

            for (int y = 0; y <= height; ++y)
            {
                spriteBatch.DrawLine(new Vector2(boardRectangle.X, boardRectangle.Y + y * cellSize.Y), new Vector2(boardRectangle.Width + boardRectangle.X, boardRectangle.Y + y * cellSize.Y), Color.Black * alpha);
                spriteBatch.DrawLine(new Vector2(boardRectangle.X, 1+ boardRectangle.Y + y * cellSize.Y), new Vector2(boardRectangle.Width + boardRectangle.X, 1+boardRectangle.Y + y * cellSize.Y), Color.White * 0.25f * alpha);
            }
            for (int x = 0; x <= width; ++x)
            {
                spriteBatch.DrawLine(new Vector2(boardRectangle.X + x * cellSize.X, boardRectangle.Y), new Vector2(boardRectangle.X + x * cellSize.X, boardRectangle.Height + boardRectangle.Y), Color.Black * alpha);
                spriteBatch.DrawLine(new Vector2(1 + boardRectangle.X + x * cellSize.X, boardRectangle.Y), new Vector2(1 + boardRectangle.X + x * cellSize.X, boardRectangle.Height + boardRectangle.Y), Color.White * 0.25f * alpha);
            }
            for (int i = 0; i < balls.Count; i++)
            {
                balls[i].Draw(spriteBatch, gameTime);
            }

            

        }

    }
}
