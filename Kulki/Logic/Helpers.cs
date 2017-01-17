namespace Logic
{
    public static class Helpers
    {
        public static int Clamp(this int value, int min, int max)
        {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }
    }
}