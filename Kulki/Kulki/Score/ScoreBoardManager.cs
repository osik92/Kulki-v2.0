using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Kulki.Score
{
    public class ScoreBoardManager
    {
        private const string filename = "scores.dat";
        private Dictionary<string, List<ScoreRecord>> scores;

        public ScoreBoardManager()
        {
            scores = CreateScorecDicionary(5, 11, 10, 20);
            if (File.Exists(filename))
            {
                Load(filename);
            }
            else
            {
                GenerateEmptyScoresData();
                Save(filename);
            }
        }

        public List<ScoreRecord> GetScoreBoard(int ballColor, int boardSize)
        {
            string key = "b" + boardSize + "x" + boardSize + "x" + ballColor;
            if (scores.ContainsKey(key))
                return scores[key];
            else
                return null;
        }

        private void GenerateEmptyScoresData()
        {
            foreach (KeyValuePair<string, List<ScoreRecord>> pair in scores)
            {
                for (int i = 0; i < 10; i++)
                {
                    pair.Value.Add(new ScoreRecord() { Date = "", Score = 0, Nick = String.Empty });
                }
            }
        }

        private Dictionary<string, List<ScoreRecord>> CreateScorecDicionary(int minBallColor, int maxBallColor, int minBoardSize, int maxBoardSize)
        {
            var dict = new Dictionary<string, List<ScoreRecord>>();

            for (int ballColor = minBallColor; ballColor <= maxBallColor; ballColor++)
                for (int size = minBoardSize; size <= maxBoardSize; ++size)
                    dict.Add("b" + size + "x" + size + "x" + ballColor, new List<ScoreRecord>());
            return dict;
        }

        public void Save()
        {
            Save(filename);
        }
        private void Save(string filename)
        {
            XmlWriterSettings settings = new XmlWriterSettings { Indent = true };

            using (XmlWriter writer = XmlWriter.Create(filename, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Scores");
                foreach (KeyValuePair<string, List<ScoreRecord>> pair in scores)
                {
                    writer.WriteStartElement(pair.Key);

                    foreach (ScoreRecord scoreRecord in pair.Value)
                    {
                        writer.WriteStartElement("score");
                        writer.WriteAttributeString("nick", scoreRecord.Nick);
                        writer.WriteAttributeString("score", scoreRecord.Score.ToString());
                        writer.WriteAttributeString("date", scoreRecord.Date);
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void Load(string filename)
        {
            using (XmlReader reader = XmlReader.Create(filename))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        string node = reader.Name;
                        if (node == "Scores")
                            continue;

                        if (scores.ContainsKey(node))
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                reader.Read();
                                if (reader.IsStartElement())
                                {
                                    scores[node].Add(new ScoreRecord() { Nick = reader["nick"], Score = Int32.Parse(reader["score"]), Date = reader["date"] });
                                }
                            }

                        }


                    }
                }
            }
        }
    }
}