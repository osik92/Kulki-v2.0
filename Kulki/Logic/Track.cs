using System.Collections.Generic;

namespace Logic
{
    public class Track
    {
        public Position StartPosition;
        public Position EndPosition;
        public Stack<Position> Steps = new Stack<Position>();
    }
}