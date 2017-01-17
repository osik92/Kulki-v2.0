using System;

namespace KulkiGlobalScoreBoard
{
    public class Score
    {
        public int Id { get; set; }
        public string Nick { get; set; }
        public int Scores { get; set; }
        public int BoardSize { get; set; }
        public int ColorNumbers { get; set; }
        public DateTime Date { get; set; }
    }
}