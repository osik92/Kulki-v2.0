using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace KulkiGlobalScoreBoard
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IGlobalScoreBoard" in both code and config file together.
    [ServiceContract]
    public interface IGlobalScoreBoard
    {
        [OperationContract]
        bool SendScoreToServer(Score score);
        [OperationContract]
        IEnumerable<Score> GetAllScores();
        [OperationContract]
        IEnumerable<Score> GetScoresByNick(string nick);
        [OperationContract]
        IEnumerable<Score> GetScoresByBoardSize(int boardSize);
        [OperationContract]
        IEnumerable<Score> GetScoresByColorNumbers(int colors);

    }
}
