using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelSim.Properties;

namespace HotelSim
{
    class Stairwell : Area
    {
        public Neighbour top { get; set; }
        public Neighbour bottom { get; set; }

        public Stairwell(int _ID, Point _location, Point _arrayLocation) : base(_ID, _location, _arrayLocation, 1, 1)
        {
            Simtype = SimType.Stairwell;
            capacity = int.MaxValue; // oneindige capaciteit
    }

        public override void ExitArea(Guest guest)
        {
            throw new NotImplementedException();
        }
    }
}

