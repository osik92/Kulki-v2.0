using System.Collections.Generic;
using Logic;
using Microsoft.Xna.Framework;

namespace Kulki
{
    public class BallsColors
    {
        private static List<Color> ballsColors = new List<Color>();

        static BallsColors()
        {
            ballsColors.Add(Color.CornflowerBlue);
            ballsColors.Add(Color.Red);
            ballsColors.Add(new Color(231, 232, 94, 255));
            ballsColors.Add(Color.Violet);
            ballsColors.Add(Color.Green);
            ballsColors.Add(Color.DarkViolet);
            ballsColors.Add(Color.Gray);
            ballsColors.Add(Color.White);
            ballsColors.Add(new Color(35, 135, 117, 255));
            ballsColors.Add(Color.Orange);
            ballsColors.Add(new Color(96, 57, 18, 255));
        }

        public static Color BallColor(eColor color)
        {
            return BallColor((int) color);
        }

        public static Color BallColor(int color)
        {
            return ballsColors[color];
        }
    }
}