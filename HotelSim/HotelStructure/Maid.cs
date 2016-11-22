using HotelSim.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HotelSim
{
    public class Maid : Entity
    {
        // public
        public string name { get; set; }
        public bool onTheElevator { get; set; }

        public List<Area> cleaningEmergency = new List<Area>();
        public List<Area> toClean = new List<Area>();

        //private
        private bool cleaning = false;
        private int cleaningTimer = 0;
        private int cleaningTime;
        private double horizontalDuration;
        private double vertilcalDuration;
        private double HTEDuration;

        public Maid(string _name, Area[,] _hotel, int _cleaningTime, int _stairTime, int _HTEPerSecond) : base(_HTEPerSecond)
        {
            name = _name;
            Simtype = SimType.Maid;
            hotelArray = _hotel;
            currentArea = hotelArray[1, 0];
            location = currentArea.location;
            cleaningTime = _cleaningTime;
            //rdm = new Random();
            AddCleaningEmergency(hotelArray[5, 6]);

            HTEDuration = 1000 / HTEPerSecond;
            horizontalDuration = 1 * HTEDuration;
            vertilcalDuration = _stairTime * HTEDuration;
        }

        public void AddAreaToClean(Area _area)
        {
            if (!toClean.Contains(_area) && !cleaningEmergency.Contains(_area))
            {
                toClean.Add(_area);
                _area.dirty = true;
            }
        }

        public void AddCleaningEmergency(Area _area)
        {
            if (!cleaningEmergency.Contains(_area) && !toClean.Contains(_area))
            {
                cleaningEmergency.Add(_area);
                _area.dirty = true;
            }
        }

        private void CleanArea(Area _area)
        {
            _area.dirty = false;
        }

        private Area GetNextAreaToClean()
        {
            Area returnValue;
            if (IsNullOrEmpty(cleaningEmergency))
            {
                if (IsNullOrEmpty(toClean))
                {
                    returnValue = null;
                }
                else
                {
                    returnValue = toClean.First();
                    while (!IsNullOrEmpty(toClean) && toClean.First() == returnValue)
                    {
                        toClean.RemoveAt(0);
                    }
                }
            }
            else
            {
                returnValue = cleaningEmergency.First();
                while (!IsNullOrEmpty(cleaningEmergency) && cleaningEmergency.First() == returnValue)
                {
                    cleaningEmergency.RemoveAt(0);
                }
                while (!IsNullOrEmpty(toClean) && toClean.First() == returnValue)
                {
                    toClean.RemoveAt(0);
                }
            }
            return returnValue;
        }

        public override void move(int millisSinceLastFrame)
        {
            if (currentArea is ElevatorShaft && next is ElevatorShaft)
            { // when guest is going to use the elevator

                ElevatorShaft elevatorShaft = currentArea as ElevatorShaft;

                shaftToGoTo = null;
                foreach (Area item in path)
                {
                    if (item is ElevatorShaft)
                    {
                        shaftToGoTo = item as ElevatorShaft;
                    }
                }
                if (shaftToGoTo == null)
                {
                    shaftToGoTo = next as ElevatorShaft;
                }

                // check if guest is already in queue
                if (elevatorShaft.queue.Contains(this))
                {
                    // remove guest from queue to be added again
                    elevatorShaft.queue.Remove(this);
                }
                // guest is going into the elevator queue
                elevatorShaft.queue.Add(this);

                // call elevator
                Elevator.Directions dir;
                if (shaftToGoTo.ID > elevatorShaft.ID)
                {
                    dir = Elevator.Directions.Up;
                }
                else
                {
                    dir = Elevator.Directions.Down;
                }
                elevatorShaft.elevator.CallToFloor(new ElevatorCall(this, elevatorShaft, dir));

                // stop guest from moving 
                path = null;
                next = null;
                onTheElevator = true;
            }
            else
            {

                if (!cleaning && !onTheElevator)
                {
                    if (next == null || location == next.entrance)
                    {
                        if (next != null)
                        {
                            currentArea = next;
                        }
                        if (IsNullOrEmpty(path))
                        {
                            if (currentArea == destination && destination.dirty == true)
                            {
                                cleaning = true;
                                cleaningTimer = cleaningTime;
                            }
                            else
                            {
                                if (destination == null)
                                {
                                    destination = GetNextAreaToClean();
                                    while (destination == currentArea)
                                    {
                                        destination = GetNextAreaToClean();
                                    }
                                }
                                if (destination != null)
                                {
                                    path = ds.GetPath(currentArea, destination);
                                    path.Reverse();
                                }
                            }
                        }
                        if (!IsNullOrEmpty(path))
                        {
                            next = path.First();
                            if ((currentArea is Stairwell && next is Stairwell) || (currentArea is ElevatorShaft && next is ElevatorShaft))
                            {
                                timeLeftForRoom = vertilcalDuration;
                            }
                            else
                            {
                                if (currentArea.left != null && currentArea.left.neighbour == next)
                                {
                                    timeLeftForRoom = currentArea.left.distance * horizontalDuration;
                                    //timeLeftForRoom = horizontalDuration * next.width;
                                }
                                else
                                {
                                    timeLeftForRoom = currentArea.right.distance * horizontalDuration;
                                    //timeLeftForRoom = horizontalDuration * currentArea.width;
                                }
                            }
                            path.RemoveAt(0);
                        }
                    }
                    else
                    {
                        Walk(millisSinceLastFrame);
                    }
                }
            }
        }

        public override void HTEElapsed(object source, EventArgs e)
        {
            #region //Commented// randomly add rooms to clean, for testing only
            /*if (rdm.Next(0, 3) == 2)
            {
                if (rdm.Next(0, 20) == 2)
                {
                    Area toAdd = hotelArray[rdm.Next(0, hotelArray.GetLength(0)), rdm.Next(0, hotelArray.GetLength(1))];
                    while (toAdd == null)
                    {
                        toAdd = hotelArray[rdm.Next(0, hotelArray.GetLength(0)), rdm.Next(0, hotelArray.GetLength(1))];
                    }
                    AddCleaningEmergency(toAdd);
                }
                else
                {
                    Area toAdd = hotelArray[rdm.Next(0, hotelArray.GetLength(0)), rdm.Next(0, hotelArray.GetLength(1))];
                    while (toAdd == null)
                    {
                        toAdd = hotelArray[rdm.Next(0, hotelArray.GetLength(0)), rdm.Next(0, hotelArray.GetLength(1))];
                    }
                    AddAreaToClean(toAdd);
                }
            }*/
            #endregion

            if (cleaningTimer > 0)
            {
                cleaningTimer--;
                if (cleaningTimer == 0)
                {
                    cleaning = false;
                    CleanArea(currentArea);
                    //MessageBox.Show("done with cleaning");
                }
            }
        }
    }
}