using HotelSim.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Timers;
using System.Windows.Forms;

namespace HotelSim
{
    public class Guest : Entity
    {
        // public
        public string name { get; set; }
        public Room bedroom { get; set; }
        public int requestedClassification { get; set; }
        public bool interacting = false;
        public Area eventLocation = null;
        public int gymTime { get; set; }
        public int eatTime { get; set; }
        // private
        private System.Timers.Timer waitTimer = new System.Timers.Timer();
        private int WaitCount { get; set; }
        private double horizontalDuration { get; set; }
        private double vertilcalDuration { get; set; }
        private double HTEDuration { get; set; }

        private int actionTimer = 0;


        public Guest(string _name, Area[,] _hotelArray, int _requestedClassification, int _stairTime, int _HTEPerSecond, int _eatTime) : base(_HTEPerSecond)
        {
            Simtype = SimType.Guest;
            name = _name;
            requestedClassification = _requestedClassification;
            hotelArray = _hotelArray;
            currentArea = hotelArray[0, 0];
            location = currentArea.location;

            HTEDuration = 1000 / HTEPerSecond;
            horizontalDuration = 1 * HTEDuration;
            vertilcalDuration = _stairTime * HTEDuration;

            eventLocation = hotelArray[1, 0];
            eatTime = _eatTime;
        }

        /// <summary>
        /// change the destination of the guest
        /// </summary>
        /// <param name="_area">new area where this guest has to go to</param>
        public void ChangeDestination(Area _area)
        {
            destination = _area;
            RecalculatePath();
            interacting = false;
        }

        public Area GetClosestAreaOfType(SimType type, Area notThisArea = null)
        {
            Area returnValue = null;
            List<Area> availebleAreas = new List<Area>();

            foreach (Area area in hotelArray)
            {
                if (area != null && area.Simtype == type && area != notThisArea)
                {
                    availebleAreas.Add(area);
                }
            }
            if (!IsNullOrEmpty(availebleAreas))
            {
                Dijkstra ds = new Dijkstra();
                double shortestDistance = double.MaxValue;
                foreach (Area area in availebleAreas)
                {
                    double distance = ds.GetDistance(this.currentArea, area);
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        returnValue = area;
                    }
                }
            }

            return returnValue;
        }

        public void RecalculatePath()
        {
            path = ds.GetPath(currentArea, destination);
            path.Reverse();
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
                interacting = true;
            }
            else
            {
                if (eventLocation != null)
                {
                    ChangeDestination(eventLocation);
                    eventLocation = null;
                }
                if (!interacting)
                {
                    if (next == null || location == next.entrance)
                    {
                        if (next != null)
                        {
                            currentArea = next;
                        }
                        if (IsNullOrEmpty(path))
                        {
                            if (currentArea == destination)
                            {
                                Interact();
                            }
                        }
                        if (!IsNullOrEmpty(path))
                        {
                            next = path.First();
                            if ((currentArea is Stairwell && next is Stairwell))
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

        public void Interact()
        {
            interacting = true;

            switch (currentArea.Simtype)
            {
                case SimType.Reception:
                    #region reception check-in/check-out
                    if (bedroom == null)
                    {
                        //check-in
                        bedroom = (currentArea as Reception).CheckIn(this, requestedClassification);
                        if (bedroom != null)
                        {
                            ChangeDestination(bedroom);
                        }
                    }
                    else
                    {
                        //check-out
                        (currentArea as Reception).CheckOut(this);
                        bedroom = null;
                        ChangeDestination(hotelArray[0, 0]);
                    }
                    #endregion
                    break;
                case SimType.Room:
                    break;
                case SimType.Gym:

                    //enter Gym
                    drawMe = false;
                    currentArea.guests.Add(this);
                    actionTimer = gymTime;
                    
                    break;
                case SimType.Cinema:
                    if ((currentArea as Cinema).moviePlaying)
                    {
                        //go back to room
                        ChangeDestination(bedroom);
                    }
                    else
                    {
                        //enter cinema
                        drawMe = false;
                        currentArea.guests.Add(this);
                    }
                    break;
                case SimType.Restaurant:
                    if (currentArea.capacity > currentArea.guests.Count)
                    {
                        //enter restaurant
                        drawMe = false;
                        currentArea.guests.Add(this);
                        actionTimer = eatTime;
                    }
                    else
                    {
                        //wait or go to other restaurant
                        int waitTime = int.MaxValue;
                        // TODO: include other guests waiting in line
                        foreach (Guest guest in currentArea.guests)
                        {
                            if (guest.actionTimer < waitTime)
                            {
                                waitTime = guest.actionTimer;
                            }
                        }


                        Area nextRestaurant = GetClosestAreaOfType(SimType.Restaurant, currentArea);
                        Dijkstra ds = new Dijkstra();
                        if (ds.GetDistance(currentArea, nextRestaurant) < waitTime)
                        {
                            eventLocation = nextRestaurant;
                        }
                        else
                        {
                            (currentArea as Restaurant).queue.Add(this);

                        }
                    }
                    break;
                case SimType.Stairwell:
                    break;
                case SimType.ElevatorShaft:

                    break;
                default:
                    break;
            }
        }
        public override void HTEElapsed(object source, EventArgs e)
        {
            if (actionTimer > 0)
            {
                actionTimer--;
                if (actionTimer == 0)
                {
                    currentArea.ExitArea(this);
                }
            }
        }

    }
}