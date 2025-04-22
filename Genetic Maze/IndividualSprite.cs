using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Genetic_Maze
{
    public sealed class IndividualSprite : Panel
    {
        private int _stepY;
        private int _stepX;
        private object locker = new object();

        public IndividualSprite(Point location, Size size)
        {
            Location = location;
            Size = size;
            BackColor = CustomColors.Individual;

            _stepX = Convert.ToInt32(Size.Width / 60.0 * 100);
            _stepY = Convert.ToInt32(Size.Height / 60.0 * 100);
        }

        public void MoveDown()
        {
            lock (locker)
            {
                for (int i = 0; i < _stepY; i++)
                {
                    Location = new Point(Location.X, Location.Y + 1);
                    Thread.Sleep(1);
                }
            }
        }
        public void MoveUp()
        {
            lock (locker)
            {
                for (int i = 0; i < _stepY; i++)
                {
                    Location = new Point(Location.X, Location.Y - 1);
                }
            }
        }
        public void MoveLeft()
        {
            lock (locker)
            {
                for (int i = 0; i < _stepX; i++)
                {
                    Location = new Point(Location.X + 1, Location.Y);
                }
            }
        }
        public void MoveRight()
        {
            lock (locker)
            {
                for (int i = 0; i < _stepX; i++)
                {
                    Location = new Point(Location.X - 1, Location.Y);
                }
            }
        }
    }
}