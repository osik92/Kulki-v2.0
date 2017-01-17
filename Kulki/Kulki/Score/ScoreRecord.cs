using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kulki.Score
{
    public class ScoreRecord
    { 
        public string Nick { get; set; }
        public int Score { get; set; }
        public string Date { get; set; }

        public override string ToString()
        {
            return Nick + " " + Score + " " + Date;
        }
    }
}
