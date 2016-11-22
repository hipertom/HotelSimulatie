using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using HotelEvents;
using System.Diagnostics;

namespace HotelSim
{
    public partial class Simulation : Form
    {
        private Hotel hotel { get; set; }

        // Creating timers
        private Timer timer = new Timer();
        private Timer frameTimer = new Timer();
        private int count = 0;
        private Stopwatch sw = new Stopwatch();
        private long timeStamp = 0;

        public Simulation(List<LayoutJsonModel> layout, ConfigJsonModel ConfigJson)
        {
            InitializeComponent();
            // Setting form to fullscreen
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            this.Size = resolution.Size;
            this.WindowState = FormWindowState.Maximized;

            // Placing Pauze panel on right location and hiding it
            panelPauze.Visible = false;
            double PauzeWidth = resolution.Width * 0.20;
            double PauzeHeight = resolution.Height * 0.75;
            this.panelPauze.Size = new Size((int)PauzeWidth, (int)PauzeHeight);
            this.panelPauze.Top = (this.ClientSize.Height - panelPauze.Height) / 2;
            this.panelPauze.Left = 10;
            this.labelGamePauzed.Width = this.panelPauze.Width;
            this.labelInfo.Width = this.panelPauze.Width;
            this.labelInfo.Height = this.panelPauze.Height - this.labelGamePauzed.Height;

            // initialize timer for hte 
            timer.Tick += new EventHandler(OnTimedEvent);
            double MilisecondsPerHte = 1.0 / ConfigJson.HtesPerSecond;
            int intervalOne = Convert.ToInt32(MilisecondsPerHte * 1000); // van seconde milisecondes maken
            timer.Interval = (intervalOne >= 1) ? (int)intervalOne : 1;


            // initialize timer for frame updates
            frameTimer.Tick += new EventHandler(OnFrameTimerUpdate);
            int FPS = 60; // fps instelbaar
            double interval2 = (double)1000 / FPS;
            frameTimer.Interval = (interval2 >= 1) ? (int)interval2 : 1;

            // create hotel
            hotel = new Hotel(layout, ConfigJson, timer);
            hotel.Display(MainSimDisplay);
            hotel.hotelPB.Click += HotelPB_Click;
            hotel.AddMaid((int)ConfigJson.RoomCleaningHTE);
            hotel.AddMaid((int)ConfigJson.RoomCleaningHTE);

            // create eventmanager
            HotelEventManager.Register(new myEventListener(hotel));
            HotelEventManager.HTE_Factor = (int)ConfigJson.HtesPerSecond;

            // start simulation
            timer.Start();
            frameTimer.Start();
            sw.Start();
            HotelEventManager.Start();

        }

        private void HotelPB_Click(object sender, EventArgs e)
        {
            Point pos = new Point(Cursor.Position.X, Cursor.Position.Y);
            if (hotel.DidIClickOnTheReception(pos))
            {
                if (timer.Enabled) // if timer is enabled then stop timer
                {
                    timer.Stop();
                    frameTimer.Stop();
                    sw.Stop();
                    HotelEventManager.Pauze();
                    Console.WriteLine("Timer gestopt");
                    panelPauze.Visible = true;
                    panelPauze.BringToFront();
                    labelInfo.Text = hotel.Info();
                }
                else // if timer is disabled then start timer
                {
                    timer.Start();
                    frameTimer.Start();
                    sw.Start();
                    HotelEventManager.Start();
                    Console.WriteLine("Timer gestart");
                    panelPauze.Visible = false;
                    labelInfo.Text = null;
                }
            }
        }

        #region oude code
        //public void HireAMaid(int cleaningTime)
        //{
        //    Maid maid = new Maid(new Point(0, (hotel.hotelHeight * hotel.roomHeight)), hotel.hotelArray, cleaningTime);
        //    timer.Tick += new EventHandler(maid.HTEElapsed);
        //    hotel.AddThisEntity(maid);
        //}

        //public void AddAGuest()
        //{
        //    Entity guest = entityFactory.CreateEntity("Guest", new Point(0,0), hotel.hotelArray);
        //    hotel.AddThisEntity(guest);
        //}

        //public Simulation(List<LayoutJsonModel> layout, ConfigJsonModel HTE) // constructor 
        //{
        //    //scherm en panel fullscreen maken
        //    InitializeComponent();
        //    Rectangle resolution = Screen.PrimaryScreen.Bounds;
        //    this.Size = resolution.Size;
        //    this.WindowState = FormWindowState.Maximized;

        //    // aanmaken, neerzetten en inrichten van het hotel

        //    hotel = GetHotel(layout);
        //    HireMaid();
        //    //HireMaid();
        //    DrawHotel();
        //    SettleHotel(HTE.ElevatorDistanceHTE, HTE.StairDistanceHTE);
        //    hotelPB.BringToFront();

        //    //methode aan timer hangen en timer tijd instellen na aanleiding van de ingestelde HTE
        //    timer.Tick += new EventHandler(OnTimedEvent);
        //    int tijd = Convert.ToInt32(HTE.HTEinSeconds * 1000); // van seconde milisecondes maken
        //    timer.Interval = tijd;
        //    timer.Enabled = true;

        //    // timer voor scherm updates
        //    FrameTimer.Tick += new EventHandler(OnFrameTimerUpdate);
        //    int FPS = 60; // fps instelbaar
        //    FrameTimer.Interval = 1000 / FPS;
        //    FrameTimer.Enabled = true;

        //} // end constructor

        //public Area[,] GetHotel(List<LayoutJsonModel> layout)
        //{
        //    // berekend de hoogte en breedte van het hotel n.a.v layout file
        //    int layoutFileX = layout.Max(t => t.position.X);
        //    int layoutFileY = layout.Max(t => t.position.Y);
        //    hotelWidth = layoutFileX + 3;
        //    hotelHeight = layoutFileY + 1;


        //    Area[,] hotel = new Area[hotelWidth, hotelHeight];


        //    //Point location = new Point(drawX, drawY);
        //    Point location = new Point(0, hotelHeight * roomHeight);
        //    Point arrayLocation;
        //    int star;

        //    // statische basis plaatsen hotel (lift, receptie en trap)
        //    // liften
        //    int ID = 100;
        //    for (int i = 0; i <= layoutFileY; i++)
        //    {
        //        hotel[0, i] = AreaFactory.createArea("ElevatorShaft", ID++, location, new Point(0, i), 1, 1, 0);
        //        // bij eerste kan lift worden aangemaakt en geplaatst in eerste shaft
        //    }
        //    // receptie
        //    hotel[1, 0] = AreaFactory.createArea("Reception", ID++, location, new Point(1, 0), layoutFileX, 1, 0);

        //    //trappen
        //    for (int i = 0; i <= layoutFileY; i++)
        //    {
        //        hotel[layoutFileX + 1, i] = AreaFactory.createArea("Stairwell", ID++, location, new Point(layoutFileX + 1, i), 1, 1, 0);
        //        // bij eerste kan lift worden aangemaakt en geplaatst in eerste shaft
        //    }
        //    // layout (file) afdrukken
        //    foreach (var item in layout)
        //    {
        //        if (item.ID == 0)
        //            item.ID = ID++;

        //        if (!string.IsNullOrWhiteSpace(item.classification))
        //            star = int.Parse(Regex.Replace(item.classification, "[^0-9]+", string.Empty));
        //        else
        //            star = 0;

        //        arrayLocation = new Point(item.position.X, item.position.Y);
        //        hotel[item.position.X, item.position.Y] = AreaFactory.createArea(item.areaType, item.ID, location, arrayLocation, item.dimension.X, item.dimension.Y, star);
        //    }

        //    drawX = (panel1.Width - ((hotelWidth - 1) * roomWidth)) / 2;
        //    drawY = (panel1.Height - ((hotelHeight) * roomHeight)) / 2;
        //    return hotel;
        //}

        //public void DrawHotel()
        //{
        //    btm = new Bitmap((hotelWidth - 1) * roomWidth, hotelHeight * roomHeight);
        //    cgi = Graphics.FromImage(btm);
        //    RefreshHotel();
        //    hotelPB.Click += new EventHandler(ReceptionClickCheck);
        //    hotelPB.Size = btm.Size;
        //    hotelPB.Location = new Point(drawX, drawY);
        //    panel1.Controls.Add(hotelPB);
        //}

        //public void RefreshHotel()
        //{
        //    foreach (Area place in hotel)
        //    {
        //        if (place != null)
        //        {
        //            place.DrawYourself(cgi);
        //        }
        //    }
        //    DrawObjects();

        //    hotelPB.Image = btm;
        //}

        //public void SettleHotel(double stairDistance, double elevatorDistance)
        //{
        //    for (int i = 0; i < hotelWidth; i++)
        //    {
        //        for (int j = 0; j < hotelHeight; j++)
        //        {
        //            // bij liften top en bottom toevoegen
        //            if (hotel[i, j] is ElevatorShaft)
        //            {
        //                if (j > 0)
        //                {
        //                    (hotel[i, j] as ElevatorShaft).bottom = new Neighbour()
        //                    {
        //                        neighbour = (ElevatorShaft)hotel[i, j - 1],
        //                        distance = elevatorDistance
        //                    };
        //                }
        //                if (j < hotelHeight - 1)
        //                {
        //                    (hotel[i, j] as ElevatorShaft).top = new Neighbour()
        //                    {
        //                        neighbour = (ElevatorShaft)hotel[i, j + 1],
        //                        distance = elevatorDistance
        //                    };
        //                }
        //            }
        //            // bij trappen top en bottom toevoegen
        //            if (hotel[i, j] is Stairwell)
        //            {
        //                if (j > 0)
        //                {
        //                    (hotel[i, j] as Stairwell).bottom = new Neighbour()
        //                    {
        //                        neighbour = (Stairwell)hotel[i, j - 1],
        //                        distance = stairDistance
        //                    };
        //                }
        //                if (j < hotelHeight - 1)
        //                {
        //                    (hotel[i, j] as Stairwell).top = new Neighbour()
        //                    {
        //                        neighbour = (Stairwell)hotel[i, j + 1],
        //                        distance = stairDistance
        //                    };
        //                }
        //            }
        //            // bij alle kamers left toevoegen
        //            if (i > 0 && hotel[i, j] != null)
        //            {
        //                int count = 1;
        //                while (hotel[i, j].left == null)
        //                {
        //                    if (hotel[i - count, j] != null)
        //                    {
        //                        //left aanmaken
        //                        hotel[i, j].left = new Neighbour
        //                        {
        //                            neighbour = hotel[i - count, j],
        //                            distance = count
        //                        };
        //                    }
        //                    else
        //                    {
        //                        if (count > hotelWidth + 1) // om oneindige loop te voorkomen
        //                        {
        //                            MessageBox.Show($"Kamer afstand naar links bij kamer \"{hotel[i, j]}\" is groter dan de hotel breedte. Waarschuw uw IT-Beheer!");
        //                            Application.Exit();
        //                            break; // niet nodig maar ja
        //                        }
        //                        count++;
        //                    }
        //                }
        //            }
        //            // bij alle kamers right toevoegen
        //            if (i <= hotelHeight && hotel[i, j] != null)
        //            {
        //                int count = 1;
        //                while (hotel[i, j].right == null)
        //                {
        //                    if (hotel[i + count, j] != null)
        //                    {
        //                        // right aanmaken
        //                        hotel[i, j].right = new Neighbour
        //                        {
        //                            neighbour = hotel[i + count, j],
        //                            distance = count
        //                        };
        //                    }
        //                    else
        //                    {
        //                        if (count > hotelWidth + 1) // om oneindige loop te voorkomen
        //                        {
        //                            MessageBox.Show($"Kamer afstand naar rechts bij kamer \"{hotel[i, j]}\" is groter dan de hotel breedte. Waarschuw uw IT-Beheer!");
        //                            Application.Exit();
        //                            break; // niet nodig maar ja
        //                        }
        //                        count++;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        //public void HireMaid()
        //{
        //    Maid maid = new Maid(new Point(0, (hotelHeight * roomHeight)), hotel);
        //    timer.Tick += new EventHandler(maid.HTEElapsed);
        //    hotelMaids.Add(maid);
        //}

        //public void DrawObjects()
        //{
        //    foreach (Maid maid in hotelMaids)
        //    {
        //        maid.DrawYourself(cgi);
        //    }
        //}
        #endregion

        /// <summary>
        /// On keyboard press mehtod, runs when you press the keyboard with the simulation open
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Simulation_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Q: // q, om te testen

                    //var test = new myEventListener(hotel);
                    //HotelEvent evt = new HotelEvent();
                    //evt.Data = new Dictionary<string, string>();
                    //evt.Data.Add("kamer", "103");
                    //evt.Data.Add("HTE", "10");
                    //evt.EventType = HotelEventType.CLEANING_EMERGENCY;
                    //test.Notify(evt);

                    //Dijkstra ds = new Dijkstra();
                    //ds.GetPath(hotel.hotelArray[1, 0], hotel.hotelArray[1, 0]);

                    var test = new myEventListener(hotel);
                    int i = 0;
                    foreach (Guest guest in hotel.guests)
                    {
                        i++;
                        HotelEvent evt = new HotelEvent();
                        evt.Data = new Dictionary<string, string>();
                        evt.Data.Add("Gast", "" + i);
                        evt.EventType = HotelEventType.NEED_FOOD;
                        test.Notify(evt);
                    }
                    break;
                case Keys.A:
                    hotel.Scroll(Hotel.MoveHotelDirection.Left);
                    break;
                case Keys.D:
                    hotel.Scroll(Hotel.MoveHotelDirection.Right);
                    break;
                case Keys.W:
                    hotel.Scroll(Hotel.MoveHotelDirection.Up);
                    break;
                case Keys.S:
                    hotel.Scroll(Hotel.MoveHotelDirection.Down);
                    break;
                case Keys.Up:
                    hotel.Scroll(Hotel.MoveHotelDirection.TestUp);
                    break;
                case Keys.Down:
                    hotel.Scroll(Hotel.MoveHotelDirection.TestDown);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Function that runs on every HTE (timer)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnTimedEvent(object source, EventArgs e)
        {
            Console.WriteLine($"{count++} HTE elapsed");
        }

        /// <summary>
        /// function that updates the frame (fps adjustable)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnFrameTimerUpdate(object source, EventArgs e)
        {
            hotel.Refresh(Convert.ToInt32(sw.ElapsedMilliseconds - timeStamp));
            timeStamp = sw.ElapsedMilliseconds;
        }
    }
}
