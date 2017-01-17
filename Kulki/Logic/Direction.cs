using System;

namespace Logic
{
    public class Direction
    {
        private enum eDirection
        {
            up,
            right,
            down,
            left
        }
        private eDirection direction = eDirection.up;

        public int GetDirectionCount()
        {
            return Enum.GetNames(typeof(eDirection)).Length;
        }
        public Position GetDirection()
        {
            switch (direction)
            {
                case eDirection.up:
                    return new Position(0, -1);
                case eDirection.right:
                    return new Position(1, 0);
                case eDirection.down:
                    return new Position(0, 1);
                case eDirection.left:
                    return new Position(-1, 0);
            }
            return new Position(0, 0);
        }

        public Position NextDirection()
        {
            if ((int)direction < GetDirectionCount() - 1)
            {
                direction++;
            }
            else
            {
                direction = 0;
            }
            return GetDirection();
        }
    }
}