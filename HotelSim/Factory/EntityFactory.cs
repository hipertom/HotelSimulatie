using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HotelSim
{
    public class EntityFactory
    {
        private Area[,] hotelArray { get; set; }
        private ConfigJsonModel config { get; set; }

        public EntityFactory(Area[,] _hotelArray, ConfigJsonModel _config)
        {
            hotelArray = _hotelArray;
            config = _config;
        }
        public Entity CreateEntity(string criteria, string name, int requestedClassification = 0, ElevatorShaft currentlyAt = null)
        {
            Entity output = null;
            switch (criteria)
            {
                case "Maid":
                    output = new Maid(name, hotelArray, (int)config.RoomCleaningHTE, (int)config.StairDistanceHTE, (int)config.HtesPerSecond);
                    break;
                case "Guest":
                    output = new Guest(name, hotelArray, requestedClassification, (int)config.StairDistanceHTE, (int)config.HtesPerSecond, config.EatHTE);
                    break;
                default:
                    MessageBox.Show(criteria + " criteria was not found in the EntityFactory! waarschuw uw IT-beheerder.");
                    break;
            } // end switch

            return output;
        }
    }
}
