using HotelSim.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelSim
{

    public class Dijkstra
    {
        List<Area> open;
        List<Area> reset;
        public Dijkstra()
        {

        }

        public List<Area> GetPath(Area start, Area end)
        {
            List<Area> returnValue;
            open = new List<Area>();
            reset = new List<Area>();
            start.distance = 0;
            Area current = start;
            while (!Bezoek(current, end))
            {
                //pak het tot nu toe kortste pad 
                if (open.Count > 0)
                {
                    current = open.Aggregate((l, r) => l.distance < r.distance ? l : r);
                }
                else
                {
                    //System.Diagnostics.Debugger.Break();
                }
            }
            returnValue = WritePath(start, end);
            foreach (Area x in reset)
            {
                if (x != null)
                {
                    x.ResetDijkstra();
                }
            }

            return returnValue;
        }
        public double GetDistance(Area start, Area end)
        {
            double returnValue;
            open = new List<Area>();
            reset = new List<Area>();
            start.distance = 0;
            Area current = start;
            while (!Bezoek(current, end))
            {
                //pak het tot nu toe kortste pad 
                if (open.Count > 0)
                {
                    current = open.Aggregate((l, r) => l.distance < r.distance ? l : r);
                }
                else
                {
                    //System.Diagnostics.Debugger.Break();
                }
            }
            returnValue = current.distance;
            foreach (Area x in reset)
            {
                if (x != null)
                {
                    x.ResetDijkstra();
                }
            }
            return returnValue;
        }
        public List<Area> WritePath(Area start, Area end)
        {
            Area current = end;
            List<Area> path = new List<Area>();
            path.Add(current);
            while (current.parent != null && current.parent != start)
            {
                path.Add(current.parent);
                current = current.parent;
            }

            return path;
        }

        private bool Bezoek(Area current, Area eind)
        {
            if (!reset.Contains(current))
            {
                reset.Add(current);
            }
            //checken op eind
            if (current == eind)
            {
                return true;
            } //niet meer bezoeken 
            if (open.Contains(current))
            {
                open.Remove(current);
            }
            //buren aflopen 
            Neighbour x = null;
            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0:
                        x = current.left;
                        break;
                    case 1:
                        x = current.right;
                        break;
                    case 2:
                        if (current is ElevatorShaft)
                        {
                            x = (current as ElevatorShaft).top;
                        }
                        else if (current is Stairwell)
                        {
                            x = (current as Stairwell).top;
                        }
                        else
                        {
                            x = null;
                        }
                        break;
                    case 3:
                        if (current is ElevatorShaft)
                        {
                            x = (current as ElevatorShaft).bottom;
                        }
                        else if (current is Stairwell)
                        {
                            x = (current as Stairwell).bottom;
                        }
                        else
                        {
                            x = null;
                        }
                        break;
                    default:
                        break;
                }
                if (x != null)
                {
                    double nieuweAfstand = current.distance + x.distance;
                    if (nieuweAfstand < x.neighbour.distance)
                    {
                        if (!reset.Contains(x.neighbour))
                        {
                            reset.Add(x.neighbour);
                        }
                        x.neighbour.distance = nieuweAfstand; // nieuwe afstand zetten 
                        x.neighbour.parent = current; // route van pad onthouden 
                        open.Add(x.neighbour); //nog bezoeken 
                    }

                }
            }
            return false;
        }
    }

}
