using System;
using System.Linq;
using HotelEvents;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace HotelSim
{

    class myEventListener : HotelEventListener
    {
        private Hotel hotel { get; set; }
        private List<Guest> guests = new List<Guest>();

        public myEventListener(Hotel currentHotel)
        {
            hotel = currentHotel;
            guests = hotel.guests;
        }

        /// <summary>
        /// method that runs on every event from the DLL
        /// </summary>
        /// <param name="evt">event object that comes from the DLL</param>
        public void Notify(HotelEvent evt)
        {
            string guestName;
            Guest guest;
            int id;
            int hte;
            
            switch (evt.EventType)
            {
                case HotelEventType.NONE:
                    break;
                case HotelEventType.CHECK_IN:
                    #region guest check in
                    // When a guest checks in event
                    // creating variables for clarity
                    guestName = evt.Data.FirstOrDefault().Key;
                    var makeInt = evt.Data.Values.FirstOrDefault();
                    int requestedClassification = int.Parse(Regex.Replace(makeInt, "[^0-9]+", string.Empty));
                    // add guest to hotel
                    hotel.AddGuest(guestName, requestedClassification);
                    // log
                    Console.WriteLine($"Guest {guestName} arrived and wants a room with {requestedClassification} stars.");
                    #endregion
                    break;
                case HotelEventType.CHECK_OUT:
                    #region guest check out

                    guestName = evt.Data.Keys.FirstOrDefault() + evt.Data.Values.FirstOrDefault();
                    guest = hotel.FindGuest(guestName);
                    guest.eventLocation = guest.GetClosestAreaOfType(SimObject.SimType.Reception);

                    Console.WriteLine("guest " + guestName + " checked out");
                    
                    #endregion
                    break;
                case HotelEventType.CLEANING_EMERGENCY:
                    #region Cleaning emergencies
                    Console.WriteLine(evt);
                    id = int.Parse(evt.Data.FirstOrDefault().Value);
                    hte = int.Parse(evt.Data.Last().Value);

                    Maid maid = hotel.GetLeastBusyMaid();

                    Area cleaningEmergancy = null;
                    foreach (Area area in hotel.hotelArray)
                    {
                        if (area != null && area.ID == id)
                        {
                            cleaningEmergancy = area;
                            break;
                        }
                    }

                    maid.AddCleaningEmergency(cleaningEmergancy);
                    // log
                    Console.WriteLine($"Maid: {maid.name} gets Cleaning Emergency of roomid: {id} and it takes {hte} HTE!");
                    #endregion
                    break;
                case HotelEventType.EVACUATE:
                    break;
                case HotelEventType.GODZILLA:
                    #region godzilla event whaaa!
                    MessageBox.Show("WHAAAA! GODZILLA ARRIVED.");
                    #endregion
                    break;
                case HotelEventType.NEED_FOOD:
                    #region guest needs food

                    guestName = evt.Data.Keys.FirstOrDefault() + evt.Data.Values.FirstOrDefault();
                    guest = hotel.FindGuest(guestName);
                    guest.eventLocation = guest.GetClosestAreaOfType(SimObject.SimType.Restaurant);

                    Console.WriteLine($"{guestName} need food");
                    #endregion
                    break;
                case HotelEventType.GOTO_CINEMA:
                    #region Guest go to Cinema event
                    guestName = evt.Data.FirstOrDefault().Key + evt.Data.FirstOrDefault().Value;
                    guest = hotel.FindGuest(guestName);

                    if( guest != null)
                    {
                        guest.eventLocation = guest.GetClosestAreaOfType(SimObject.SimType.Cinema);
                    }

                    // log 
                    Console.WriteLine(evt);
                    Console.WriteLine($"{guestName} is now going to the Cinema!");
                    #endregion
                    break;
                case HotelEventType.GOTO_FITNESS:
                    #region guest go to fitness
                    guestName = evt.Data.FirstOrDefault().Key + evt.Data.FirstOrDefault().Value;
                    guest = hotel.FindGuest(guestName);
                    Area gym = guest.GetClosestAreaOfType(SimObject.SimType.Gym);
                    int AmountOfHTE = int.Parse(evt.Data.ElementAt(1).Value);
                    int gymTime = AmountOfHTE;
                    if (guest != null)
                    {
                        guest.gymTime = gymTime;
                        guest.ChangeDestination(gym);
                    }

                    // log
                    Console.WriteLine(guestName + " is going to the gym for " + AmountOfHTE + " hte.");
                    #endregion
                    break;
                case HotelEventType.START_CINEMA:
                    #region cinema start movie
                    id = int.Parse(evt.Data.FirstOrDefault().Value);

                    Cinema cinema = null;
                    foreach (Area area in hotel.hotelArray)
                    {
                        if (area != null && area.ID == id && area is Cinema)
                        {
                            cinema = (area as Cinema);
                            break;
                        }
                    }
                    cinema.StartMovie();
                    // log
                    Console.WriteLine("Cinema movie start with ID: "+ id);
                    #endregion
                    break;
                default:
                    break;
            }
        }
    }
}
