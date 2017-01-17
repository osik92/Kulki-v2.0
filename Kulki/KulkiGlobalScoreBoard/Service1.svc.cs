using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using LiteDB;

namespace KulkiGlobalScoreBoard
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "GlobalScoreBoard" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select GlobalScoreBoard.svc or GlobalScoreBoard.svc.cs at the Solution Explorer and start debugging.
    public class GlobalScoreBoard : IGlobalScoreBoard
    {
        public bool SendScoreToServer(Score score)
        {
            using (var db = new LiteDatabase("database.db"))
            {
                try
                {
                    var col = db.GetCollection<Score>("scores");
                    col.Insert(score);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return false;
                }
            }
        }

        public IEnumerable<Score> GetAllScores()
        {
            using (var db = new LiteDatabase("database.db"))
            {
                try
                {
                    var col = db.GetCollection<Score>("scores");
                    return col.FindAll();


                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        public IEnumerable<Score> GetScoresByNick(string nick)
        {
            using (var db = new LiteDatabase("database.db"))
            {
                try
                {
                    var col = db.GetCollection<Score>("scores");
                    return col.Find(x => x.Nick == nick);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        public IEnumerable<Score> GetScoresByBoardSize(int boardSize)
        {
            using (var db = new LiteDatabase("database.db"))
            {
                try
                {
                    var col = db.GetCollection<Score>("scores");
                    return col.Find(x => x.BoardSize == boardSize);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        public IEnumerable<Score> GetScoresByColorNumbers(int colors)
        {
            using (var db = new LiteDatabase("database.db"))
            {
                try
                {
                    var col = db.GetCollection<Score>("scores");
                    return col.Find(x => x.ColorNumbers == colors);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            }
        }
    }
}
