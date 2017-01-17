using System.Linq;
using System.Threading.Tasks;

namespace Logic
{
    public class Ball
    {
        private Position position;
        public eColor color;

        public Ball(eColor color, Position position)
        {
            this.color = color;
            this.position = position;
        }

        public Position Position
        {
            get { return position; }
        }

        public override string ToString()
        {
            return ((int)color).ToString();
        }
    }
}