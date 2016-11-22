using HotelSim.Properties;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System;

namespace HotelSim
{
    public class ElevatorShaft : Area
    {
        public List<Entity> queue = new List<Entity>();
        
        public Elevator elevator { get; set; }
        public Neighbour top { get; set; }
        public Neighbour bottom { get; set; }

        public ElevatorShaft(int _ID, Point _location, Point _arrayLocation) : base(_ID, _location, _arrayLocation, 1, 1)
        {
            Simtype = SimType.ElevatorShaft;
            image = new Bitmap(Resources.Elevator_Shaft);
            capacity = int.MaxValue;
        }

        public override void ExitArea(Guest guest)
        {
            throw new NotImplementedException();
        }
    }
}
