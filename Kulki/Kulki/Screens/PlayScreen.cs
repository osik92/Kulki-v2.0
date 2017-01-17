using System;
using System.Collections.Generic;
using GameEngine.GUI;
using GameEngine.Screens;
using Kulki.Score;
using Kulki.ServiceReference1;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using EngineMath = GameEngine.Utilities.Math;

namespace Kulki.Screens
{
    internal class PlayScreen : GameScreen
    {
        private static List<Color> ballsColors = new List<Color>();
        private bool pauseEnable = false;

        private Rectangle boardRectangle;
        private Color bgColor;

        private GameBoard board;
        private int points = 0;
        private GUI gui;

        private ScoreBoardManager scoreManager = new ScoreBoardManager();
        private List<ScoreRecord> records = new List<ScoreRecord>();
        private int combo = 0;
        private bool showCombo = false;
        TimeSpan comboTime = TimeSpan.Zero;
        private Vector2 comboPos = Vector2.Zero;

        private Image ballImage;
        static PlayScreen()
        {
            ballsColors.Add(Color.CornflowerBlue);
            ballsColors.Add(Color.Red);
            ballsColors.Add(Color.Yellow);
            ballsColors.Add(Color.Violet);
            ballsColors.Add(Color.Green);
            ballsColors.Add(Color.DarkViolet);
            ballsColors.Add(Color.Gray);
            ballsColors.Add(Color.White);
            ballsColors.Add(new Color(75, 75, 75, 255));
            ballsColors.Add(Color.Orange);
            ballsColors.Add(Color.Brown);
        }

        private Texture2D balls;
        private Texture2D background;
        private Texture2D explossion;
        private Song bgSong;
        private Point selectedBall;
        private bool gameover = false;

        public PlayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5f);
            TransitionOffTime = TimeSpan.FromSeconds(0.5f);
        }
        public override void Activate(bool instancePreserved)
        {
            balls = ScreenManager.Content.Load<Texture2D>("ball2");
            background = ScreenManager.Content.Load<Texture2D>("bg_game");
            bgSong = ScreenManager.Content.Load<Song>("music");
            explossion = ScreenManager.Content.Load<Texture2D>("explossion");
            gui = new GUI(ScreenManager.Input);

            var btn = new Button("Menu");
            btn.BackColor = Color.Transparent;

            btn.FrontColor = Color.White * 0.75f;
            btn.HoverColor = Color.White;
            btn.TextFont = ScreenManager.DefaultFont;
            btn.Position = new Vector2(10,10);
            btn.ClickAction += () =>
            {
                ScreenManager.AddScreen(new PauseMenuScreen());
                pauseEnable = true;
            };
            gui.AddControl(btn);

            

            ballImage = new Image(balls,
                new Rectangle((int) (balls.Width/4f*Config.Storage.Get<int>("ballStyle")), 0, (int) (balls.Width/4f),
                    balls.Height));



            board =
                    new GameBoard.Factory()
                        .Height(Config.Storage.Get<int>("boardSize"))
                        .Width(Config.Storage.Get<int>("boardSize"))
                        .Colors(Config.Storage.Get<int>("colors"))
                        .SetExplossionTexture(explossion)
                        .Input(ScreenManager.Input)
                        .Image(ballImage)
                        .Create();

            records = scoreManager.GetScoreBoard(Config.Storage.Get<int>("colors"), Config.Storage.Get<int>("boardSize"));

            board.AddPoints += delegate(int newPoints) { this.points += newPoints; };
            board.GameOver += delegate
            {
                gameover = true;
                if (points > 0)
                {
                    records.Add(new ScoreRecord()
                    {
                        Date = DateTime.Now.ToString(),
                        Nick = Environment.UserName,
                        Score = points
                    });
                    records.Sort((x2, x1) => x1.Score.CompareTo(x2.Score));
                    records = records.GetRange(0, 10);
                    scoreManager.Save();
                    ServiceReference1.GlobalScoreBoardClient client = new GlobalScoreBoardClient();
                    client.SendScoreToServer(new ServiceReference1.Score()
                    {
                        BoardSize = Config.Storage.Get<int>("boardSize"),
                        ColorNumbers = Config.Storage.Get<int>("colors"),
                        Date = DateTime.Now,
                        Nick = Environment.UserName,
                        Scores = points
                    });
                }

                btn = new Button("Zagraj ponownie");
                btn.BackColor = Color.Transparent;

                btn.FrontColor = Color.White * 0.75f;
                btn.HoverColor = Color.White;
                btn.TextFont = ScreenManager.DefaultFont;
                btn.Position = new Vector2(ScreenManager.Viewport.Bounds.Right - 300, ScreenManager.Viewport.Bounds.Bottom -125);
                btn.ClickAction += () =>
                {
                    LoadingScreen.Load(ScreenManager, true, new PlayScreen());
                    pauseEnable = true;
                };
                gui.AddControl(btn);

                btn = new Button("Powrót do menu");
                btn.BackColor = Color.Transparent;

                btn.FrontColor = Color.White * 0.75f;
                btn.HoverColor = Color.White;
                btn.TextFont = ScreenManager.DefaultFont;
                btn.Position = new Vector2(ScreenManager.Viewport.Bounds.Right - 300, ScreenManager.Viewport.Bounds.Bottom - 75);
                btn.ClickAction += () =>
                {
                    LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                        new MainMenuScreen());
                };
                gui.AddControl(btn);



            };
            board.InitBoard();
            board.ComboIncreased += ShowCombo;

            MediaPlayer.Stop();
            MediaPlayer.Play(bgSong);
        }

        private void ShowCombo(int combo)
        {
            showCombo = true;
            this.combo = combo;
            comboTime = TimeSpan.FromSeconds(1);
            comboPos = ScreenManager.Input.GetMousePosition().ToVector2() + new Vector2(10,-20);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            
            base.Update(gameTime, otherScreenHasFocus, false);

                boardRectangle = new Rectangle(ScreenManager.Viewport.Bounds.Location + new Point(20, 80),
                new Point((int)(ScreenManager.Viewport.Bounds.Height - 100),
                    (int)(ScreenManager.Viewport.Bounds.Height - 100)));

            board.BoardArea = boardRectangle;

            

            if (State == eScreenState.Active && !pauseEnable)
            {
                if (ScreenManager.Input.IsKeyDown(Keys.Escape) || !ScreenManager.Game.IsActive)
                {
                    ScreenManager.AddScreen(new PauseMenuScreen());
                    pauseEnable = true;
                }

                if (showCombo)
                {
                    comboTime -= gameTime.ElapsedGameTime;
                    comboPos -= new Vector2(0, 0.05f);
                    if (comboTime <= TimeSpan.Zero)
                    {
                        showCombo = false;
                        comboTime = TimeSpan.Zero;
                    }
                }
                gui.Update(gameTime);
                board.Update(gameTime);
            }
            if (ScreenManager.GetScreens().Length == 1)
                pauseEnable = false;

        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(background, spriteBatch.GraphicsDevice.Viewport.Bounds, null, Color.White * TransitionAlpha);
            board.Draw(spriteBatch, gameTime, TransitionAlpha);
            var pointsPositon =
                new Vector2(board.BoardArea.Right - ScreenManager.DefaultFont.MeasureString(points.ToString()).X,
                    board.BoardArea.Top - ScreenManager.DefaultFont.MeasureString(points.ToString()).Y);
            spriteBatch.DrawString(ScreenManager.DefaultFont, points.ToString(), pointsPositon + new Vector2(-1, -1), Color.Black * TransitionAlpha);
            spriteBatch.DrawString(ScreenManager.DefaultFont, points.ToString(), pointsPositon + new Vector2(1,-1), Color.Black * TransitionAlpha);
            spriteBatch.DrawString(ScreenManager.DefaultFont, points.ToString(), pointsPositon + new Vector2(-1, 1), Color.Black * TransitionAlpha);
            spriteBatch.DrawString(ScreenManager.DefaultFont, points.ToString(), pointsPositon + new Vector2(1, 1), Color.Black * TransitionAlpha);
            spriteBatch.DrawString(ScreenManager.DefaultFont, points.ToString(), pointsPositon, Color.White * TransitionAlpha );

            int yPos = 250;
            int xPos = board.BoardArea.Right + 50;
            foreach (ScoreRecord record in records)
            {
                spriteBatch.DrawString(ScreenManager.DefaultFont, record.Nick + "    " + record.Score + "    " + record.Date, new Vector2(xPos, yPos) + new Vector2(-1, -1), Color.Black*TransitionAlpha);
                spriteBatch.DrawString(ScreenManager.DefaultFont, record.Nick + "    " + record.Score + "    " + record.Date, new Vector2(xPos, yPos) + new Vector2(1,-1), Color.Black * TransitionAlpha);
                spriteBatch.DrawString(ScreenManager.DefaultFont, record.Nick + "    " + record.Score + "    " + record.Date, new Vector2(xPos, yPos) + new Vector2(-1, 1), Color.Black * TransitionAlpha);
                spriteBatch.DrawString(ScreenManager.DefaultFont, record.Nick + "    " + record.Score + "    " + record.Date, new Vector2(xPos, yPos) + new Vector2(1, 1), Color.Black * TransitionAlpha);
                spriteBatch.DrawString(ScreenManager.DefaultFont, record.Nick + "    " + record.Score + "    " + record.Date, new Vector2(xPos, yPos), Color.White * TransitionAlpha);
                yPos += 30;
            }

            if (showCombo)
            {
                spriteBatch.DrawString(ScreenManager.DefaultFont, "x" + combo, comboPos + new Vector2(+10, +10), Color.Black * TransitionAlpha, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
                spriteBatch.DrawString(ScreenManager.DefaultFont, "x" + combo, comboPos + new Vector2(+5, +5), Color.Red * TransitionAlpha, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
                spriteBatch.DrawString(ScreenManager.DefaultFont, "x" + combo, comboPos, Color.Orange * TransitionAlpha, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
            }

            spriteBatch.Draw(ballImage.Texture, new Rectangle(boardRectangle.Right + 30, boardRectangle.Top + 10, 40,40), ballImage.SourceRectangle, BallsColors.BallColor(board.NextColors[0]) * TransitionAlpha, 0, ballImage.Center, SpriteEffects.None, 0);
            spriteBatch.Draw(ballImage.Texture, new Rectangle(boardRectangle.Right + 90, boardRectangle.Top + 10, 40, 40), ballImage.SourceRectangle, BallsColors.BallColor(board.NextColors[1]) * TransitionAlpha, 0, ballImage.Center, SpriteEffects.None, 0);
            spriteBatch.Draw(ballImage.Texture, new Rectangle(boardRectangle.Right + 150, boardRectangle.Top + 10, 40, 40), ballImage.SourceRectangle, BallsColors.BallColor(board.NextColors[2]) * TransitionAlpha, 0, ballImage.Center, SpriteEffects.None, 0);

            gui.Draw(spriteBatch, gameTime, TransitionAlpha);

            spriteBatch.End();
        }

        public override void Unload()
        {
            balls = null;
            background = null;
        }
    }
}