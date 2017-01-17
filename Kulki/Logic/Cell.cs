using System;

namespace Logic
{
    public class Cell
    {
        private Ball ball;

        public bool IsEmpty
        {
            get { return ball == null; }
        }

        public Ball GetBall()
        {
            return ball;
        }

        public void SetBall(Ball ball)
        {
            this.ball = ball;
        }

        public override string ToString()
        {
            return String.Format("{0}", this.ball == null ? "." : ball.ToString());
        }
    }
}