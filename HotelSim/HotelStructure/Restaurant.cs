using HotelSim.Properties;
using System.Collections.Generic;
using System.Drawing;
using System;
using System.Windows.Forms;

namespace HotelSim
{
    public class Restaurant : Area
    {


        public List<Guest> queue = new List<Guest>();
        public Restaurant(int _ID, Point _location, Point _arrayLocation, int _width, int _height, int _capacity) : base(_ID, _location, _arrayLocation, _width, _height)
        {
            Simtype = SimType.Restaurant;
            capacity = _capacity;
        }

        public override void ExitArea(Guest guest)
        {
            guests.Remove(guest);
            guest.drawMe = true;
            guest.ChangeDestination(guest.bedroom);
            if (!IsNullOrEmpty(queue))
            {
                Guest nextInLine = queue[0];
                queue.RemoveAt(0);
                nextInLine.Interact();
            }
        }
    }
}
