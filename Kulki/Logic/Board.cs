using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Logic
{
    public class Board // mo¿liwe, ¿e to fasada logiki
    {
        private Cell[,] gameboard;
        private Random random = new Random();
        private int colorsCounts;
        public int Width
        {
            get { return gameboard.GetLength(0); }
        }

        public int Height
        {
            get { return gameboard.GetLength(1); }
        }
        public Board(int Width, int Height, int colorsCounts)
        {
            gameboard = new Cell[Width, Height];
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    gameboard[x, y] = new Cell();
            ColorsCounts = colorsCounts;
        }

        public Cell[,] GameBoard
        {
            get { return this.gameboard; }
        }

        public bool GameOver
        {
            get
            {
                foreach (Cell cell in GameBoard)
                {
                    if (cell.IsEmpty)
                        return false;
                }
                return true;
            }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine(string.Format("Board size {0} x {1}", gameboard.GetLength(1), gameboard.GetLength(0)));
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    sb.Append(gameboard[x, y]);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }



        public int ColorsCounts
        {
            get { return colorsCounts; }
            set { colorsCounts = value.Clamp(1, Enum.GetNames(typeof(eColor)).Length); }
        }

        public Ball RandomBall()
        {
            eColor color = (eColor)random.Next(0, colorsCounts);
            return RandomBall(color);
        }

        public Ball RandomBall(eColor color)
        {
            if (GameOver)
                return null;
            int x = random.Next(0, Width);
            int y = random.Next(0, Height);

            if (!gameboard[x, y].IsEmpty)
            {
                return RandomBall(color);
            }

            Debug.WriteLine(String.Format("{0} x {1} : {2}", x, y, color));
            Ball ball = new Ball(color, new Position(x, y));
            gameboard[x, y].SetBall(ball);
            return ball;
        }

        public Track FindTrack(Position start, Position destination)
        {
            #region krok 1 - skopiuj obecn¹ plansze
            int[,] board = new int[Width, Height];
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    if (gameboard[x, y].IsEmpty) { board[x, y] = 0; }
                    else { board[x, y] = -1; }
            #endregion
            #region krok 2 - definiujemy kolejke, dodajemy do niej aktualn¹ pozycje
            Queue<TrackPosition> trackQueue = new Queue<TrackPosition>();

            board[start.X, start.Y] = 1;
            trackQueue.Enqueue(new TrackPosition(start, 1));
            bool findTrack = false;
            #endregion
            #region krok 3 - jeœli w kolejce s¹ elementy to sprawdŸ s¹siednie elementy, dodaj im wartoœæ o jeden wiêksz¹ ni¿ obecny element kolejki oraz dodaj je do kolejki
            while (trackQueue.Count > 0)
            {
                var elemnt = trackQueue.Dequeue();

                if (elemnt.Position.Equals(destination))
                {
                    findTrack = true;
                    break;
                }
                Direction direction = new Direction();

                for (int i = 0; i < direction.GetDirectionCount(); i++)
                {
                    Position temporaryPosition = elemnt.Position + direction.NextDirection();
                    if (temporaryPosition.IsInTheRange(new Position(0, 0), new Position(Width, Height)) && board[temporaryPosition.X, temporaryPosition.Y] == 0)
                    {
                        board[temporaryPosition.X, temporaryPosition.Y] = elemnt.Value + 1;
                        trackQueue.Enqueue(new TrackPosition(temporaryPosition, elemnt.Value + 1));
                    }
                }

            }
            #endregion
            #region krok 4 - Gdy droga istnieje, tworzymy element Track, gdy nie istnieje zwracany element NullTrack

            if (findTrack)
            {
                Direction direction = new Direction();
                TrackPosition currentPosition = new TrackPosition(destination, board[destination.X, destination.Y]);
                Track track = new Track
                {
                    StartPosition = start,
                    EndPosition = destination,
                };
                track.Steps.Push(currentPosition.Position);
                while (!currentPosition.Position.Equals(start))
                {
                    Position tempPosition = currentPosition.Position + direction.GetDirection();
                    if (tempPosition.IsInTheRange(new Position(0, 0), new Position(Width, Height)))
                        if (currentPosition.Value - 1 == board[tempPosition.X, tempPosition.Y])
                        {
                            track.Steps.Push(tempPosition);
                            currentPosition = new TrackPosition(tempPosition, board[tempPosition.X, tempPosition.Y]);
                        }
                        else
                            direction.NextDirection();
                    else
                        direction.NextDirection();
                }
                return track;
            }
            else
            {
                return null;
            }


            #endregion
        }

        public bool MoveBall(Position start, Position destination)
        {
            Track track;
            return MoveBall(start, destination, out track);
        }
        public bool MoveBall(Position start, Position destination, out Track track)
        {
            track = null;
            if (start == null || destination == null)
                return false;
            if (!start.IsInTheRange(Position.Zero, new Position(Width, Height)) || !destination.IsInTheRange(Position.Zero, new Position(Width, Height)))
                return false;

            Cell startCell = gameboard[start.X, start.Y];
            Cell destinationCell = gameboard[destination.X, destination.Y];

            if (!destinationCell.IsEmpty || startCell.IsEmpty)
                return false;

            Ball ball = startCell.GetBall();
            if (ball == null)
                return false;

            track = FindTrack(start, destination);
            if (track == null)
                return false;
            ball = new Ball(ball.color, destination);
            destinationCell.SetBall(ball);
            startCell.SetBall(null);
            return true;
        }

        public int RemovePointBallsAndGetPoints()
        {
            HashSet<Ball> pointBalls = new HashSet<Ball>();
            CheckCells(pointBalls);
            CheckRows(pointBalls);
            CheckLeftCross(pointBalls);
            CheckRightCross(pointBalls);
            int points = pointBalls.Count;
            RemovePointBalls(pointBalls);

            return points;
        }

        private void CheckRightCross(HashSet<Ball> balls)
        {
            for (int x = 0; x < Width; x++)
            {
                Ball firstBall = null;
                List<Ball> ballsList = new List<Ball>();
                for (int i = 0; i <= x; i++)
                {
                    Position pos = new Position(i, (Height - 1 - x) + i);
                    if (pos.IsInTheRange(new Position(0, 0), new Position(Width, Height)))
                    {
                        if (!gameboard[pos.X, pos.Y].IsEmpty)
                        {
                            Ball currentBall = gameboard[pos.X, pos.Y].GetBall();

                            if (firstBall == null)
                            {
                                ballsList.Add(currentBall);
                                firstBall = currentBall;
                            }
                            else
                            {
                                if (currentBall.color == firstBall.color)
                                    ballsList.Add(currentBall);
                                else
                                {
                                    ballsList.Clear();
                                    firstBall = currentBall;
                                    ballsList.Add(currentBall);
                                }
                            }

                            if (ballsList.Count >= 5)
                            {
                                foreach (Ball ball in ballsList)
                                {
                                    balls.Add(ball);
                                }
                            }
                        }
                        else
                        {
                            ballsList.Clear();
                            firstBall = null;
                        }
                    }
                }
            }
            for (int x = 0; x < Width - 1; x++)
            {
                List<Ball> ballsList = new List<Ball>();
                Ball firstBall = null;
                for (int i = 0; i <= x; i++)
                {
                    Position pos = new Position((Height - 1 - x) + i, i);
                    if (pos.IsInTheRange(new Position(0, 0), new Position(Width, Height)))
                    {
                        if (!gameboard[pos.X, pos.Y].IsEmpty)
                        {
                            Ball currentBall = gameboard[pos.X, pos.Y].GetBall();

                            if (firstBall == null)
                            {
                                ballsList.Add(currentBall);
                                firstBall = currentBall;
                            }
                            else
                            {
                                if (currentBall.color == firstBall.color)
                                    ballsList.Add(currentBall);
                                else
                                {
                                    ballsList.Clear();
                                    firstBall = currentBall;
                                    ballsList.Add(currentBall);
                                }
                            }

                            if (ballsList.Count >= 5)
                            {
                                foreach (Ball ball in ballsList)
                                {
                                    balls.Add(ball);
                                }
                            }
                        }
                        else
                        {
                            ballsList.Clear();
                            firstBall = null;
                        }
                    }
                }
            }
        }

        private void CheckLeftCross(HashSet<Ball> balls)
        {
            for (int x = 0; x <= Width; x++)
            {
                Ball firstBall = null;
                List<Ball> ballsList = new List<Ball>();
                for (int i = 0; i <= x; i++)
                {
                    Position pos = new Position(Height - i, (Height - 1 - x) + i);
                    if (pos.IsInTheRange(new Position(0, 0), new Position(Width, Height)))
                    {
                        if (!gameboard[pos.X, pos.Y].IsEmpty)
                        {
                            Ball currentBall = gameboard[pos.X, pos.Y].GetBall();

                            if (firstBall == null)
                            {
                                ballsList.Add(currentBall);
                                firstBall = currentBall;
                            }
                            else
                            {
                                if (currentBall.color == firstBall.color)
                                {

                                    ballsList.Add(currentBall);
                                }
                                else
                                {
                                    ballsList.Clear();
                                    firstBall = currentBall;
                                    ballsList.Add(currentBall);
                                }
                            }

                            if (ballsList.Count >= 5)
                            {
                                foreach (Ball ball in ballsList)
                                {
                                    balls.Add(ball);
                                }
                            }
                        }
                        else
                        {
                            ballsList.Clear();
                            firstBall = null;
                        }
                    }
                }
            }
            for (int x = 0; x < Width - 1; x++)
            {
                List<Ball> ballsList = new List<Ball>();
                Ball firstBall = null;
                for (int i = 0; i <= x; i++)
                {
                    Position pos = new Position(i, x - i);
                    if (pos.IsInTheRange(new Position(0, 0), new Position(Width, Height)))
                    {
                        if (!gameboard[pos.X, pos.Y].IsEmpty)
                        {
                            Ball currentBall = gameboard[pos.X, pos.Y].GetBall();

                            if (firstBall == null)
                            {
                                ballsList.Add(currentBall);
                                firstBall = currentBall;
                            }
                            else
                            {
                                if (currentBall.color == firstBall.color)
                                    ballsList.Add(currentBall);
                                else
                                {
                                    ballsList.Clear();
                                    firstBall = currentBall;
                                    ballsList.Add(currentBall);
                                }
                            }

                            if (ballsList.Count >= 5)
                            {
                                foreach (Ball ball in ballsList)
                                {
                                    balls.Add(ball);
                                }
                            }
                        }
                        else
                        {
                            ballsList.Clear();
                            firstBall = null;
                        }
                    }
                }
            }
        }

        private void CheckRows(HashSet<Ball> balls)
        {
            for (int y = 0; y < Height; y++)
            {
                List<Ball> ballsList = new List<Ball>();
                Ball firstBall = null;
                for (int x = 0; x < Width; x++)
                {
                    if (!gameboard[x, y].IsEmpty)
                    {
                        Ball currentBall = gameboard[x, y].GetBall();

                        if (firstBall == null)
                        {
                            ballsList.Add(currentBall);
                            firstBall = currentBall;
                        }
                        else
                        {
                            if (currentBall.color == firstBall.color)
                                ballsList.Add(currentBall);
                            else
                            {
                                ballsList.Clear();
                                firstBall = currentBall;
                                ballsList.Add(currentBall);
                            }
                        }

                        if (ballsList.Count >= 5)
                        {
                            foreach (Ball ball in ballsList)
                            {
                                balls.Add(ball);
                            }
                        }
                    }
                    else
                    {
                        ballsList.Clear();
                        firstBall = null;
                    }
                }
            }
        }

        private void RemovePointBalls(HashSet<Ball> pointBalls)
        {
            foreach (Ball ball in pointBalls)
            {
                gameboard[ball.Position.X, ball.Position.Y].SetBall(null);
            }
            pointBalls.Clear();
        }
        private void CheckCells(HashSet<Ball> balls)
        {
            for (int x = 0; x < Width; x++)
            {
                List<Ball> ballsList = new List<Ball>();
                Ball firstBall = null;
                for (int y = 0; y < Height; y++)
                {
                    if (!gameboard[x, y].IsEmpty)
                    {
                        Ball currentBall = gameboard[x, y].GetBall();

                        if (firstBall == null)
                        {
                            ballsList.Add(currentBall);
                            firstBall = currentBall;
                        }
                        else
                        {
                            if (currentBall.color == firstBall.color)
                            {
                                ballsList.Add(currentBall);
                            }
                            else
                            {
                                ballsList.Clear();
                                firstBall = currentBall;
                                ballsList.Add(currentBall);
                            }
                        }

                        if (ballsList.Count >= 5)
                        {
                            foreach (Ball ball in ballsList)
                            {
                                balls.Add(ball);
                            }
                        }
                    }
                    else
                    {
                        ballsList.Clear();
                        firstBall = null;
                    }
                }
            }
        }
    }
}