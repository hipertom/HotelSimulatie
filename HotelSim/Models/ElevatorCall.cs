using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelSim
{
    public class ElevatorCall
    {
        public Entity person { get; set; }
        public ElevatorShaft floor { get; set; }
        public Elevator.Directions direction { get; set; }

        public ElevatorCall(Entity _entity, ElevatorShaft _floor, Elevator.Directions _direction)
        {
            floor = _floor;
            direction = _direction;
            person = _entity;
        }
    }
}
