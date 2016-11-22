using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelSim
{
    class Pool : Area
    {
        public Pool(int _ID, Point _location, Point _arrayLocation, int _width, int _height) : base(_ID, _location, _arrayLocation, _width, _height)
        {
            Simtype = SimType.Pool;
        }

        public override void ExitArea(Guest guest)
        {
            throw new NotImplementedException();
        }
    }
}
