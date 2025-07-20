using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace RobotNavigator
{
    public abstract class Tile
    {
        private Point _position;

        public Tile(int x, int y)
        {
            _position = new Point(x, y);
        }

        public Point Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }
    }
}
