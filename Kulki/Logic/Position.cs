using System;

namespace Logic
{
    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return String.Format("{0} x {1}", X, Y);
        }

        public override bool Equals(object obj)
        {
            if (obj is Position)
            {
                Position position = obj as Position;
                return position.X.Equals(X) && position.Y.Equals(Y);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode();
        }

        public static Position operator +(Position pos1, Position pos2)
        {
            return new Position(pos1.X + pos2.X, pos1.Y + pos2.Y);
        }

        public bool IsInTheRange(Position minimum, Position maximum)
        {
            return (this.X >= minimum.X && this.Y >= minimum.Y)
                   && (this.X < maximum.X && this.Y < maximum.Y);
        }

        public static Position Zero
        {
            get { return new Position(0, 0); }
        }
    }
}