using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelSim.Properties;
using System.Windows.Forms;

namespace HotelSim
{
    public class Reception : Area
    {
        private Area[,] hotelArray;
        private Hotel hotel;

        public Reception(int _ID, Point _location, Point _arrayLocation, int _width, int _height, Area[,] _hotelArray, Hotel _hotel) : base(_ID, _location, _arrayLocation, _width, _height)
        {
            Simtype = SimType.Reception;
            capacity = int.MaxValue; // oneindige capaciteit
            hotelArray = _hotelArray;
            hotel = _hotel;
        }

        /// <summary>
        /// gives a guest a requested room
        /// </summary>
        /// <param name="_guest">guest at the reception</param>
        /// <param name="_requestedClassification">stars of the room the guest wants</param>
        /// <returns>the given room</returns>
        public Room CheckIn(Guest _guest, int _requestedClassification)
        {
            List<Area> availableRooms = new List<Area>();
            Room returnValue = null;
            foreach (Area room in hotelArray)
            {
                if (room != null && room.Simtype == SimType.Room)
                {
                    if ((room as Room).classification == _requestedClassification && !(room as Room).occupied)
                    {
                        availableRooms.Add(room);
                    }
                }
            }
            if (!IsNullOrEmpty(availableRooms))
            {
                Dijkstra ds = new Dijkstra();
                double shortestDistance = double.MaxValue;
                foreach (Room room in availableRooms)
                {
                    double distance = ds.GetDistance(this, room);
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        returnValue = room;
                    }
                }
            }
            returnValue.guests.Add(_guest);
            returnValue.occupied = true;
            return returnValue;
        }

        /// <summary>
        /// checks a guest out of the hotel
        /// </summary>
        /// <param name="guest">guest to check out</param>
        public void CheckOut(Guest guest)
        {
            guest.bedroom.dirty = true;
            guest.bedroom.guests.Remove(guest);
            guest.bedroom.occupied = false;
            Maid maid = hotel.GetLeastBusyMaid();
            maid.AddAreaToClean(guest.bedroom);
            hotel.guestsCheckingOut.Add(guest);
        }

        public override void ExitArea(Guest guest)
        {
            throw new NotImplementedException();
        }
    }
}
