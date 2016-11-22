using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HotelSim
{
    public class Hotel
    {
        // public variables
        public enum MoveHotelDirection { Up, Down, Left, Right, TestUp, TestDown }
        public PictureBox hotelPB = new PictureBox();
        public int HtesPerSecond { get; set; }
        public List<Guest> guests = new List<Guest>();
        public List<Guest> guestsCheckingOut = new List<Guest>();
        public Area[,] hotelArray { get; set; }
        // privates variables
        private const int roomWidth = 120;
        private const int roomHeight = 55;
        private const int scrollDistance = 10;
        private EntityFactory entityFactory { get; set; }
        private Elevator elevator { get; set; }
        private List<Maid> maids = new List<Maid>();
        private Point StartLocation = new Point();
        private int hotelWidth { get; set; }
        private int hotelHeight { get; set; }
        private Bitmap btm { get; set; }
        private Graphics cgi { get; set; }
        private Timer HTETimer { get; set; }
        private int MaidNameCount = 1;

        /***** Place elevators yes of no *****/
        bool placeElevators = true;
        /***** We kregen de elevator niet goed werken binnen de tijd *****/

        public Hotel(List<LayoutJsonModel> layout, ConfigJsonModel ConfigFile, Timer _timer) // ctor
        {
            HTETimer = _timer;
            HtesPerSecond = ConfigFile.HtesPerSecond;

            hotelArray = CreateHotelFromLayout(layout);
            hotelArray = AssignNeighboursToHotel(hotelArray, ConfigFile.StairDistanceHTE, ConfigFile.ElevatorDistanceHTE);
            entityFactory = new EntityFactory(hotelArray, ConfigFile);

            // set movie duration
            foreach (Area item in hotelArray)
            {
                if (item != null && item.Simtype == SimObject.SimType.Cinema)
                {
                    Cinema cin = item as Cinema;
                    cin.movieDuration = ConfigFile.MovieDurationHTE;
                }
            }
            
        }

        /// <summary>
        /// creates the hotel array from the given layout json model
        /// </summary>
        /// <param name="layout">give a LayoutJsonModel list to be build</param>
        /// <returns>hotelarray from layoutmodel</returns>
        private Area[,] CreateHotelFromLayout(List<LayoutJsonModel> layout)
        {
            // Calculating width and height of hotel and layout file
            int lowestX = layout.Min(t => t.position.X);
            int lowestY = layout.Min(t => t.position.Y);
            int layoutFileX = layout.Max(t => t.position.X + (t.dimension.X - 1));
            int layoutFileY = layout.Max(t => t.position.Y + (t.dimension.Y - 1));

            int XOffset = (lowestX == 0 ? 1 : 0);
            int YOffset = (lowestY == 0 ? 1 : 0);

            int elevatorWidth = 1;
            int stairsWidth = 1;

            hotelWidth = elevatorWidth + layoutFileX + stairsWidth + XOffset + 1;
            hotelHeight = layoutFileY + YOffset + 1;
            Area[,] hotelArray = new Area[hotelWidth, hotelHeight];

            // default values for rooms
            Point location = new Point(0, hotelHeight * roomHeight);
            Point arrayLocation;
            int star;
            int ID = 100;

            for (int i = 0; i <= layoutFileY; i++) // place elevators
            {
                int width = 1;
                int height = 1;

                if (placeElevators)
                {
                    Area temp = AreaFactory.createArea("ElevatorShaft", ID++, location, new Point(0, i), width, height, 0);
                    hotelArray[0, i] = temp;
                    if (i == 0)
                    {
                        elevator = new Elevator(temp as ElevatorShaft, HtesPerSecond);
                        HTETimer.Tick += new EventHandler(elevator.HTEElapsed);
                    }
                }
                else
                {
                    hotelArray[0, i] = AreaFactory.createArea("Stairwell", ID++, location, new Point(0, i), width, height, 0);
                }

            }

            // placing of reception
            hotelArray[1, 0] = AreaFactory.createArea("Reception", ID++, location, new Point(1, 0), layoutFileX + XOffset, 1, 0, null, hotelArray, 0, this);

            // placing of rest of the hotel based on layout file
            foreach (var item in layout)
            {
                if (item.ID == 0)
                    item.ID = ID++;

                if (!string.IsNullOrWhiteSpace(item.classification))
                    star = int.Parse(Regex.Replace(item.classification, "[^0-9]+", string.Empty));
                else
                    star = 0;

                arrayLocation = new Point(item.position.X + XOffset, item.position.Y + YOffset);
                hotelArray[arrayLocation.X, arrayLocation.Y] = AreaFactory.createArea(item.areaType, item.ID, location, arrayLocation, item.dimension.X, item.dimension.Y, item.capacity, HTETimer, null, star);
            }



            for (int i = 0; i <= layoutFileY; i++) // place elevators and stairs
            {
                Point StairwellLocation = new Point(elevatorWidth + layoutFileX + XOffset, i);
                hotelArray[StairwellLocation.X, StairwellLocation.Y] = AreaFactory.createArea("Stairwell", ID++, location, StairwellLocation, 1, 1, 0);
            }

            return hotelArray;
        }

        /// <summary>
        /// displays the hotel on a given form item
        /// </summary>
        /// <param name="c">give a panel or form or something to display the hotel on</param>
        public void Display(Control c)
        {
            StartLocation.X = (c.Width - ((hotelWidth - 1) * roomWidth)) / 2;
            StartLocation.Y = (c.Height - ((hotelHeight) * roomHeight)) / 2;

            btm = new Bitmap((hotelWidth - 1) * roomWidth, hotelHeight * roomHeight);
            cgi = Graphics.FromImage(btm);
            Refresh(0);
            hotelPB.Size = btm.Size;
            c.Controls.Add(hotelPB);
        }

        /// <summary>
        /// refreshes the hotel every frame
        /// </summary>
        /// <param name="millisSinceLastFrame">calculated time between frames</param>
        public void Refresh(int millisSinceLastFrame)
        {
            foreach (Area place in hotelArray)
            {
                if (place != null)
                {
                    if (place is Room)
                    {
                        Room room = place as Room;
                        place.DrawYourself(cgi, room.Simtype, room.classification);
                    }
                    else
                    {
                        place.DrawYourself(cgi, place.Simtype);
                    }
                }
            }
            DrawObjects(millisSinceLastFrame);

            hotelPB.Image = btm;
            hotelPB.Location = new Point(StartLocation.X, StartLocation.Y);
            hotelPB.BringToFront();

        }

        /// <summary>
        /// (re)draws every object in the hotel
        /// </summary>
        /// <param name="millisSinceLastFrame">Calculated time between frames</param>
        private void DrawObjects(int millisSinceLastFrame)
        {
            if (guestsCheckingOut.Count != 0)
            {
                foreach (Guest guest in guestsCheckingOut)
                {
                    if (guest.currentArea == hotelArray[0, 0])
                    {
                        guests.Remove(guest);
                    }
                }
            }
            if (placeElevators)
            {
                // elevator
                elevator.DrawYourself(cgi, SimObject.SimType.Elevator);
            }

            // maids and guests
            List<Entity> entities = maids.Concat<Entity>(guests).ToList(); // TO test
            foreach (Entity ent in entities)
            {
                ent.move(millisSinceLastFrame);
                ent.DrawYourself(cgi, ent.Simtype, 0);
            }


        }

        /// <summary>
        /// Will give each area in the hotel the needed neighbours
        /// </summary>
        /// <param name="hotelArray">give the filled in hotel array</param>
        /// <param name="stairDistance">distance between stairs</param>
        /// <param name="elevatorDistance">speed between elevatorshafts</param>
        /// <returns></returns>
        private Area[,] AssignNeighboursToHotel(Area[,] hotelArray, double stairDistance, double elevatorDistance)
        {
            for (int i = 0; i < hotelWidth; i++)
            {
                for (int j = 0; j < hotelHeight; j++)
                {
                    // add top and bottom to elevatorshaft and add elevator

                    if (hotelArray[i, j] is ElevatorShaft)
                    {
                        // add elevator to elevatorshaft
                        ElevatorShaft current = hotelArray[i, j] as ElevatorShaft;
                        current.elevator = elevator;

                        // top and bottom to elevator
                        if (j > 0)
                        {
                            (hotelArray[i, j] as ElevatorShaft).bottom = new Neighbour()
                            {
                                neighbour = (ElevatorShaft)hotelArray[i, j - 1],
                                distance = elevatorDistance
                            };
                        }
                        if (j < hotelHeight - 1)
                        {
                            (hotelArray[i, j] as ElevatorShaft).top = new Neighbour()
                            {
                                neighbour = (ElevatorShaft)hotelArray[i, j + 1],
                                distance = elevatorDistance
                            };
                        }
                    }
                    // bij trappen top en bottom toevoegen
                    if (hotelArray[i, j] is Stairwell)
                    {
                        if (j > 0)
                        {
                            (hotelArray[i, j] as Stairwell).bottom = new Neighbour()
                            {
                                neighbour = (Stairwell)hotelArray[i, j - 1],
                                distance = stairDistance
                            };
                        }
                        if (j < hotelHeight - 1)
                        {
                            (hotelArray[i, j] as Stairwell).top = new Neighbour()
                            {
                                neighbour = (Stairwell)hotelArray[i, j + 1],
                                distance = stairDistance
                            };
                        }
                    }
                    // bij alle kamers left toevoegen
                    if (i > 0 && hotelArray[i, j] != null)
                    {
                        int count = 1;
                        while (hotelArray[i, j].left == null)
                        {
                            if (hotelArray[i - count, j] != null)
                            {
                                //left aanmaken
                                hotelArray[i, j].left = new Neighbour
                                {
                                    neighbour = hotelArray[i - count, j],
                                    distance = count
                                };
                            }
                            else
                            {
                                if (count > hotelWidth + 1) // om oneindige loop te voorkomen
                                {
                                    MessageBox.Show($"Kamer afstand naar links bij kamer \"{hotelArray[i, j]}\" is groter dan de hotel breedte. Waarschuw uw IT-Beheer!");
                                    Application.Exit();
                                    break; // niet nodig maar ja
                                }
                                count++;
                            }
                        }
                    }
                    // bij alle kamers right toevoegen
                    if (i < hotelHeight && hotelArray[i, j] != null)
                    {
                        int count = 1;
                        while (hotelArray[i, j].right == null)
                        {
                            if (hotelArray[i + count, j] != null)
                            {
                                // right aanmaken
                                hotelArray[i, j].right = new Neighbour
                                {
                                    neighbour = hotelArray[i + count, j],
                                    distance = count
                                };
                            }
                            else
                            {
                                if (count > hotelWidth + 1) // om oneindige loop te voorkomen
                                {
                                    MessageBox.Show($"Kamer afstand naar rechts bij kamer \"{hotelArray[i, j]}\" is groter dan de hotel breedte. Waarschuw uw IT-Beheer!");
                                    Application.Exit();
                                    break; // niet nodig maar ja
                                }
                                count++;
                            }
                        }
                    }
                }
            }
            return hotelArray;
        }

        /// <summary>
        /// Moves the hotel in a given direction
        /// </summary>
        /// <param name="direction">give enum of direction to move in</param>
        public void Scroll(MoveHotelDirection direction)
        {
            switch (direction)
            {
                case MoveHotelDirection.Up:
                    StartLocation.Y -= scrollDistance;
                    break;
                case MoveHotelDirection.Down:
                    StartLocation.Y += scrollDistance;
                    break;
                case MoveHotelDirection.Left:
                    StartLocation.X -= scrollDistance;
                    break;
                case MoveHotelDirection.Right:
                    StartLocation.X += scrollDistance;
                    break;
                case MoveHotelDirection.TestUp:
                    //elevator.MoveUp(1);
                    break;
                case MoveHotelDirection.TestDown:
                    //elevator.MoveDown(1);
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// Mehthod that gets called when u click on the hotel panel. If mouse click was on the reception, then pause simulation.
        /// </summary>
        /// <param name="mouseLocation">Point on the screen where the mouse click was</param>
        /// <returns>true if mouse click was on the reception, else false</returns>
        public bool DidIClickOnTheReception(Point mouseLocation)
        {
            Reception reception = hotelArray[1, 0] as Reception;
            // reception area
            Point receptionLocation = new Point(reception.location.X, reception.location.Y + roomHeight / 2);
            Size receptionSize = new Size(reception.width * roomWidth, reception.height * roomHeight);
            Rectangle receptionArea = new Rectangle(receptionLocation, receptionSize);

            // mouse click point position
            mouseLocation.X -= StartLocation.X;
            mouseLocation.Y -= StartLocation.Y;

            if (receptionArea.Contains(mouseLocation)) // check if mouseposition is on receptionarea
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a maid to the hotel
        /// </summary>
        /// <param name="cleanTime">CLeaning speed of this maid</param>
        public void AddMaid(int cleanTime)
        {
            string name = "Tim" + MaidNameCount++;
            Entity maid = entityFactory.CreateEntity("Maid", name);
            HTETimer.Tick += new EventHandler(maid.HTEElapsed);
            maids.Add(maid as Maid);
        }

        /// <summary>
        /// Finds the maid with the least amount of items in emergencies. (TODO should also consider distance to walk)
        /// </summary>
        /// <returns>Maid with least amount of items in emergencies</returns>
        public Maid GetLeastBusyMaid()
        {
            Maid output = null;
            int emergancy = int.MaxValue;
            int cleaning = int.MaxValue;

            foreach (Maid maid in maids)
            {
                if (maid.cleaningEmergency.Count < emergancy)
                {
                    output = maid;
                    emergancy = maid.cleaningEmergency.Count;
                    cleaning = maid.toClean.Count;
                }
                else if (maid.cleaningEmergency.Count == emergancy && maid.toClean.Count < cleaning)
                {
                    output = maid;
                    emergancy = maid.cleaningEmergency.Count;
                    cleaning = maid.toClean.Count;
                }
            }

            return output;
        }

        /// <summary>
        /// Adds a guest to the hotel
        /// </summary>
        /// <param name="name">Name of the guest</param>
        /// <param name="requestedClassification">Number of stars of the room that the guests wants</param>
        public void AddGuest(string name, int requestedClassification)
        {
            Entity guest = entityFactory.CreateEntity("Guest", name, requestedClassification);
            HTETimer.Tick += new EventHandler(guest.HTEElapsed);
            guests.Add(guest as Guest);
            //return (guest as Guest);
        }

        /// <summary>
        /// finds a guest in the hotel by name
        /// </summary>
        /// <param name="guestName">the name of the guest you want to find</param>
        /// <returns>Guest or null</returns>
        public Guest FindGuest(string guestName)
        {
            foreach (Guest guest in guests)
            {
                if (guest.name.Equals(guestName))
                {
                    return guest;
                }
            }

            return null;
        }

        /// <summary>
        /// Find all the areas of a given type
        /// </summary>
        /// <param name="SimType">Area.Simtype enum of the rooms you want to find</param>
        /// <returns></returns>
        public List<Area> FindArea(Area.SimType SimType)
        {
            List<Area> output = new List<Area>();

            foreach (Area area in hotelArray)
            {
                if (area != null && area.Simtype == SimType)
                {
                    output.Add(area);
                }
            }

            return output;
        }

        /// <summary>
        /// Creates a string with info of the hotel
        /// </summary>
        /// <returns>String of all the info of the hotel</returns>
        public string Info()
        {
            string output = null;

            if (placeElevators)
            {
                // position elevator
                output += $"\nPlaats van de lift: {elevator.currentlyAt.ID - 100} \n";

                // destenation of elevator
                if (elevator.elevatorCalls.Count > 0 || elevator.peopleInElevator.Count > 0)
                {
                    if (elevator.peopleInElevator.Count > 0)
                    {
                        output += $"\nBestemming van de lift: {elevator.peopleInElevator.FirstOrDefault().shaftToGoTo.ID - 100}\n";
                    }
                    else
                    {
                        output += $"\nBestemming van de lift: {elevator.elevatorCalls.FirstOrDefault().floor.ID - 100}\n";
                    }
                }
                else
                {
                    output += $"\nBestemming van de lift: lift is aan het wachten\n";
                }
                
                // Guests in the elevator
                string guestsInTheElevator = "";
                foreach (Entity item in elevator.peopleInElevator.ToList())
                {
                    if (item is Guest)
                    {
                        var temp = item as Guest;
                        guestsInTheElevator += temp.name + ", ";
                    }

                    if (item is Maid)
                    {
                        var temp = item as Maid;
                        guestsInTheElevator += temp.name + ", ";
                    }

                }
                output += $"\nInhoudt van de lift: {guestsInTheElevator}\n";
            }
            else
            {
                output += "\nDe lift is op dit moment uitgeschakeld omdat we deze niet werkend gekregen hebben\n";
            }

            // Guests display
            output += "\nGasten in het hotel: \n";
            output += "".PadRight(4) + "Naam:".PadRight(15) + "Locatie".PadRight(20) + "Destinatie".PadRight(20) + "\n";
            foreach (Guest guest in guests)
            {
                string dest = guest.destination != null ? guest.destination.Simtype.ToString() : "Geen Bestemming";
                output += "".PadRight(4) + guest.name.PadRight(15) + guest.currentArea.Simtype.ToString().PadRight(20) + dest.PadRight(20) + "\n";
            }

            // Maids display
            output += "\nSchoonmakers in het hotel: \n";
            output += "".PadRight(4) + "Naam:".PadRight(15) + "Locatie".PadRight(20) + "Destinatie".PadRight(20) + "\n";
            foreach (Maid maid in maids)
            {
                string dest = maid.destination != null ? maid.destination.Simtype.ToString() : "Geen Bestemming";
                output += "".PadRight(4) + maid.name.PadRight(15) + maid.currentArea.Simtype.ToString().PadRight(20) + dest.PadRight(20) + "\n";
            }
            return output;
        }
    }
}
