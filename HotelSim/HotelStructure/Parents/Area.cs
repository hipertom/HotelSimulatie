using HotelSim.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HotelSim
{
    public abstract class Area : SimObject
    {
        // public
        public int ID { get; set; }
        public int capacity { get; set; }
        public List<Guest> guests { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public Point entrance { get; set; }
        public bool dirty { get; set; }
        public Neighbour left { get; set; }
        public Neighbour right { get; set; }
        public double distance { get; set; }
        public Area parent { get; set; }
        // private
        private const int roomWidth = 120;
        private const int roomHeight = 55;

        public Area(int _ID, Point _location, Point _arrayLocation, int _width, int _height) : base(_location)
        {
            ID = _ID;
            width = _width;
            height = _height;
            location = new Point((location.X + (_arrayLocation.X * (roomWidth))), (location.Y - (roomHeight * height) - (_arrayLocation.Y * roomHeight)));
            parent = null;
            distance = double.MaxValue;
            entrance = new Point(location.X, location.Y + ((height - 1) * roomHeight));
            dirty = false;
            guests = new List<Guest>();
        }

        public override void DrawYourself(Graphics target, SimType SimType, int stars = 0)
        {
            Brush color = new SolidBrush(Color.FromArgb(255,0,0));
            // setting background color for each room
            switch (SimType)
            {
                case SimType.Reception:
                    break;
                case SimType.Room:
                    color = new SolidBrush(Color.FromArgb(180, 182, 110));
                    break;
                case SimType.Gym:
                    color = new SolidBrush(Color.FromArgb(211, 192, 142));
                    break;
                case SimType.Cinema:
                    color = new SolidBrush(Color.FromArgb(21, 90, 107));
                    break;
                case SimType.Pool:
                    color = new SolidBrush(Color.FromArgb(223, 209, 143));
                    break;
                case SimType.Restaurant:
                    break;
                case SimType.Stairwell:
                    break;
                case SimType.ElevatorShaft:
                    break;
                default:
                    break;
            }
            target.FillRectangle(color, location.X, location.Y, roomWidth * width, roomHeight * height);

            // placing tile image
            TextureBrush brush = new TextureBrush(image);
            target.FillRectangle(brush, location.X, location.Y + (roomHeight * (height -1)), roomWidth * width, roomHeight);

            // placing door and stars on room
            if ( SimType == SimType.Room)
            {
                //draw door and stars
                Image door = Resources.Door;
                Image star = Resources.star;
                int size = 10;

                target.DrawImage(door, location.X + 25, location.Y + (roomHeight * (height - 1)) + 8);

                for (int i = 0; i < stars; i++)
                {
                    target.DrawImage(star, location.X + 70 +((i * (size+3))) , location.Y + (roomHeight * (height - 1))+ 13  , size, size);
                }
            }
        }

        /// <summary>
        /// resets the dijksta functionallity 
        /// </summary>
        public void ResetDijkstra()
        {
            parent = null;
            distance = double.MaxValue;
        }
        public abstract void ExitArea(Guest guest);
    }
}
