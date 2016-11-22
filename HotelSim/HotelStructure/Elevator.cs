using HotelSim.Properties;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System;
using System.Windows.Forms;

namespace HotelSim
{
    public class Elevator : Entity
    {
        // public
        public enum Directions { Up, Down };
        public ElevatorShaft currentlyAt { get; set; }
        public List<Entity> peopleInElevator { get; set; }
        public List<ElevatorCall> elevatorCalls { get; set; }
        // private
        private enum State { Load, Unload, Move, Waiting };
        private State state { get; set; }

        public Elevator(ElevatorShaft _currentlyAt, int _HTEPerSecond) : base(_HTEPerSecond)
        {
            Simtype = SimType.Elevator;
            ChangeLocation(_currentlyAt);
            state = State.Waiting;
            elevatorCalls = new List<ElevatorCall>();
            peopleInElevator = new List<Entity>();
        }

        /// <summary>
        /// runs every hte
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public override void HTEElapsed(object source, EventArgs e)
        {
            if (!IsNullOrEmpty(elevatorCalls) || !IsNullOrEmpty(peopleInElevator))
            {
                switch (state) // state switch
                {
                    case State.Load: // pickup guests from elevatorshaft
                        foreach (Entity person in currentlyAt.queue.ToList())
                        {
                            peopleInElevator.Add(person);
                            currentlyAt.queue.Remove(person);
                            foreach (ElevatorCall item in elevatorCalls.ToList())
                            {
                                if (item.person == person)
                                {
                                    elevatorCalls.Remove(item);
                                }
                            }
                        }

                        DrawGuestsInElevator();
                        state = State.Move;
                        break;
                    case State.Unload: // guests leave the elevator
                        List<Entity> peopleToKick = new List<Entity>();
                        foreach (Entity person in peopleInElevator)
                        {
                            if (currentlyAt == person.shaftToGoTo)
                            {
                                peopleToKick.Add(person);
                            }
                        }
                        KickGuestsFromElevator(peopleToKick);
                        foreach (Entity person in peopleToKick)
                        {
                            peopleInElevator.Remove(person);
                        }
                        peopleToKick.Clear();
                        state = State.Load;
                        break;
                    case State.Move: // move up or down

                        if (!IsNullOrEmpty(peopleInElevator))
                        {
                            GoToFloor(peopleInElevator.FirstOrDefault().shaftToGoTo);
                        }
                        else
                        {
                            GoToFloor(elevatorCalls.FirstOrDefault().floor);
                        }

                        break;
                    case State.Waiting: // wait for guest elevator call
                        if (currentlyAt == elevatorCalls.FirstOrDefault().floor)
                        {
                            state = State.Load;
                        }
                        else
                        {
                            GoToFloor(elevatorCalls.FirstOrDefault().floor);
                        }
                        break;
                    default:
                        break;
                }
            }

        }

        /// <summary>
        /// will tell the elevator to go to a given floor
        /// </summary>
        /// <param name="floor">floor to go to</param>
        private void GoToFloor(ElevatorShaft floor)
        {
            if (currentlyAt.ID < floor.ID)
            {
                MoveUp();
            }
            else if (currentlyAt.ID > floor.ID)
            {
                MoveDown();
            }
            else
            {
                state = State.Unload;
            }
        }

        public override void move(int millisSinceLastFrame)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// sets new location off the elevator
        /// </summary>
        /// <param name="_currentlyAt">new location of the elevator</param>
        private void ChangeLocation(ElevatorShaft _currentlyAt)
        {
            currentlyAt = _currentlyAt;
            location = currentlyAt.location;

            if (peopleInElevator != null)
            {
                foreach (Entity person in peopleInElevator.ToList())
                {
                    person.currentArea = _currentlyAt;
                }
            }
        }

        /// <summary>
        /// moves the elevator up one floor
        /// </summary>
        private void MoveUp()
        {
            if (currentlyAt.top != null) // if not at the top
            {
                // move to top neighbour
                ChangeLocation(currentlyAt.top.neighbour as ElevatorShaft);
            }

            DrawGuestsInElevator();
        }

        /// <summary>
        /// moves the elevator down one floor
        /// </summary>
        private void MoveDown()
        {
            if (currentlyAt.bottom != null) // if not at bottom
            {
                // move to bottom neighbour
                ChangeLocation(currentlyAt.bottom.neighbour as ElevatorShaft);
            }

            DrawGuestsInElevator();
        }

        /// <summary>
        /// places the guests visually in the elevator
        /// </summary>
        private void DrawGuestsInElevator()
        {
            foreach (Entity person in peopleInElevator)
            {
                Point newLocation = new Point(this.location.X + 75, this.location.Y);
                person.location = newLocation;
            }
        }

        /// <summary>
        /// place guests outside of the elevator and removes them from elevator inventory
        /// </summary>
        /// <param name="peopleToKick">list of guests to kick</param>
        private void KickGuestsFromElevator(List<Entity> peopleToKick)
        {
            foreach (Entity person in peopleToKick)
            {
                // place guest outside of elevator
                Point newLocation = new Point(this.location.X, this.location.Y);
                person.location = newLocation;
                // let the guest walk again
                person.currentArea = currentlyAt;
                if (person is Guest)
                {
                    (person as Guest).interacting = false;
                    // recalculate path for guest to go to destination
                    (person as Guest).ChangeDestination(person.destination);
                }
                else
                {
                    (person as Maid).onTheElevator = false;
                    // recalculate path for guest to go to destination
                    //(person as Maid).
                }
                // remove guest from elevator
                peopleInElevator.Remove(person);
            }
        }

        /// <summary>
        /// adds a elevator call
        /// </summary>
        /// <param name="call">give a ElevatorCallModel with the floor on witch the guest is waiting</param>
        public void CallToFloor(ElevatorCall call)
        {
            elevatorCalls.Add(call);
        }
    }
}