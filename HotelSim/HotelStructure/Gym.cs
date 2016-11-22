using HotelSim.Properties;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System;

namespace HotelSim
{
    public class Gym : Area
    {
        

        public Gym(int _ID, Point _location, Point _arrayLocation, int _width, int _height) : base(_ID, _location, _arrayLocation, _width, _height)
        {
            Simtype = SimType.Gym;
            capacity = int.MaxValue;
    }

        public override void ExitArea(Guest guest)
        {
            guests.Remove(guest);
            guest.drawMe = true;
            guest.ChangeDestination(guest.bedroom);
        }
    }
}
