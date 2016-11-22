using HotelSim.Properties;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System;

namespace HotelSim
{
    public class Room : Area
    {
        public int classification { get; set; }
        public bool occupied { get; set; }

        public Room(int _ID, Point _location, Point _arrayLocation, int _width, int _height, int _stars = 0) : base(_ID, _location, _arrayLocation, _width, _height)
        {
            Simtype = SimType.Room;
            classification = _stars;
            occupied = false;

            capacity = 1;
    }

        public override void ExitArea(Guest guest)
        {
            //throw new NotImplementedException();
        }
    }
}
