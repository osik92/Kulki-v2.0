namespace Logic
{
    public struct TrackPosition
    {
        public Position Position;
        public int Value;

        public TrackPosition(Position position, int value)
        {
            Position = position;
            Value = value;
        }
    }
}