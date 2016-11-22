using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace HotelSim
{
    public class AreaFactory
    {
        public static Area createArea(string criteria, int ID, Point location, Point arrayLocation, int width, int height, int capacity, Timer HTETimer = null, Area[,] hotelArray = null, int classification = 0, Hotel hotel = null)
        {
            Area output = null;
            // moet nog aangepast worden -> factory filmpje vincent uitbreidbaardheid???
            switch (criteria)
            {
                // in layout file
                case "Room":
                    output = new Room(ID, location, arrayLocation, width, height, classification);
                    break;
                case "Cinema":
                    output = new Cinema(ID, location, arrayLocation, width, height);
                    HTETimer.Tick += new EventHandler((output as Cinema).HTEElapsed);
                    break;
                case "Restaurant":
                    output = new Restaurant(ID, location, arrayLocation, width, height, capacity);
                    break;
                case "Fitness":
                    output = new Gym(ID, location, arrayLocation, width, height);
                    break;
                case "Pool":
                    output = new Pool(ID, location, arrayLocation, width, height);
                    break;
                // niet in layout file
                case "Reception":
                    output = new Reception(ID, location, arrayLocation, width, height, hotelArray, hotel);
                    break;
                case "Stairwell":
                    output = new Stairwell(ID, location, arrayLocation);
                    break;
                case "ElevatorShaft":
                    output = new ElevatorShaft(ID, location, arrayLocation);
                    break;
                default:
                    MessageBox.Show(criteria + " criteria was not found in the AreaFactory! waarschuw uw IT-beheerder.");
                    break;
            }
            return output;
        } // end createobject()
    }
}
