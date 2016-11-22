using HotelSim.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HotelSim
{
    public abstract class SimObject
    {
        public Bitmap image { get; set; }
        public Point location { get; set; }
        public PictureBox pb { get; set; }
        public enum SimType { Reception, Room, Gym, Cinema, Pool, Restaurant, Stairwell, ElevatorShaft, Guest, Maid, Elevator };
        public SimType Simtype { get; set; }

        /// <summary>
        /// Draws a simobject on the hotel
        /// </summary>
        /// <param name="target">Grapic of the hotel</param>
        /// <param name="AreaType">the type of area to draw</param>
        /// <param name="stars">(optional) number of stars to draw on a room</param>
        public abstract void DrawYourself(Graphics target, SimType AreaType, int stars = 0);

        public SimObject(Point _location)
        {
            // rooms
            if (this is Reception)
                image = new Bitmap(Resources.Reception);
            else if (this is Room)
                image = new Bitmap(Resources.Room);
            else if (this is Gym)
                image = new Bitmap(Resources.Gym);
            else if (this is Cinema)
                image = new Bitmap(Resources.Cinema);
            else if (this is Restaurant)
                image = new Bitmap(Resources.Restaurant);
            else if (this is Stairwell)
                image = new Bitmap(Resources.Stairwell);
            else if (this is ElevatorShaft)
                image = new Bitmap(Resources.Elevator_Shaft);
            else if (this is Elevator)
                image = new Bitmap(Resources.Elevator);
            else if (this is Pool)
                image = new Bitmap(Resources.Pool);
            // entities
            else if (this is Guest)
                image = new Bitmap(Resources.sim);
            else if (this is Maid)
                image = new Bitmap(Resources.maid);
            else if (this is Elevator)
                image = new Bitmap(Resources.Elevator);



            location = _location;
        }

        public bool IsNullOrEmpty(List<Area> list)
        {
            if (list == null)
            {
                return true;
            }
            Area[] array = list.ToArray();
            return (array.Length == 0);
        }
        public bool IsNullOrEmpty(List<Entity> list)
        {
            if (list == null)
            {
                return true;
            }
            Entity[] array = list.ToArray();
            return (array.Length == 0);
        }
        public bool IsNullOrEmpty(List<Guest> list)
        {
            if (list == null)
            {
                return true;
            }
            Guest[] array = list.ToArray();
            return (array.Length == 0);
        }
        public bool IsNullOrEmpty(List<ElevatorCall> list)
        {
            if (list == null)
            {
                return true;
            }
            ElevatorCall[] array = list.ToArray();
            return (array.Length == 0);
        }
    }
}
