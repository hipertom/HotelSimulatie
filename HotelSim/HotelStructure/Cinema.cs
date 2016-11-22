using HotelSim.Properties;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System;
using System.Linq;

namespace HotelSim
{
    public class Cinema : Area
    {
        public bool moviePlaying { get; set; }
        public int movieDuration { get; set; }
        private int movieTimer { get; set; }

        public Cinema(int _ID, Point _location, Point _arrayLocation, int _height, int _width) : base(_ID, _location, _arrayLocation, _width, _height)
        {
            Simtype = SimType.Cinema;
            capacity = int.MaxValue;
            moviePlaying = false;
            movieDuration = 10;
            movieTimer = 0;
        }

        /// <summary>
        /// starts the movie
        /// </summary>
        public void StartMovie()
        {
            movieTimer = movieDuration;
            moviePlaying = true;
        }

        public override void ExitArea(Guest guest)
        {
            guests.Remove(guest);
            guest.drawMe = true;
            guest.ChangeDestination(guest.bedroom);
        }

        public void HTEElapsed(object source, EventArgs e)
        {
            if (moviePlaying)
            {
                movieTimer--;
                if (movieTimer == 0)
                {
                    moviePlaying = false;
                    foreach (Guest guest in guests.ToList())
                    {
                        guest.currentArea.ExitArea(guest);
                    }
                }
            }
        }
    }
}
