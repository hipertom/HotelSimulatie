using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelSim
{
    public class LayoutJsonModel
    {
        public int ID { get; set; }
        public string classification { get; set; }
        public string areaType { get; set; }
        public Point position { get; set; }
        public Point dimension { get; set; }
        public int capacity { get; set; }
    }
}
