using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelSim
{
    public abstract class Entity : SimObject
    {
        // public
        public ElevatorShaft shaftToGoTo { get; set; }
        public Area currentArea { get; set; }
        public Area next { get; set; }
        public Area destination { get; set; }
        public Area[,] hotelArray { get; set; }
        public List<Area> path { get; set; }
        public Dijkstra ds = new Dijkstra();
        public double timeLeftForRoom { get; set; }
        public int HTEPerSecond { get; set; }
        public bool drawMe = true;

        // private
        private enum WalkingDirection { left, right };
        private WalkingDirection facingPrevious { get; set; }
        private WalkingDirection facing { get; set; }
        private double deltaX { get; set; }
        private double deltaY { get; set; }

        public Entity(int _HTEPerSecond) : base(new Point(0, 0))
        {
            HTEPerSecond = _HTEPerSecond;
            drawMe = true;
            //Size pixelSize = new Size(image.Width, image.Height);
            //location = new Point(_location.X, _location.Y - pixelSize.Height);
        }

        public abstract void HTEElapsed(object source, EventArgs e);
        public abstract void move(int millisSinceLastFrame);

        public void Walk(int millisSinceLastFrame)
        {
            if (facingPrevious != facing)
            {
                image.RotateFlip(RotateFlipType.RotateNoneFlipX);
            }
            deltaX += ((next.entrance.X - location.X) / Math.Abs(timeLeftForRoom)) * ((double)millisSinceLastFrame);
            int moveX = (int)deltaX;
            deltaX -= moveX;
            deltaY += ((next.entrance.Y - location.Y) / Math.Abs(timeLeftForRoom)) * ((double)millisSinceLastFrame);
            int moveY = (int)deltaY;
            deltaY -= moveY;

            timeLeftForRoom -= millisSinceLastFrame;
            facingPrevious = facing;
            if (moveX != 0)
            {
                if (moveX < 0)
                {
                    facing = WalkingDirection.right;
                }
                else
                {
                    facing = WalkingDirection.left;
                }
            }
            location = new Point(location.X + moveX, location.Y + moveY);
        }

        public override void DrawYourself(Graphics target, SimType AreaType, int stars = 0)
        {
            if (AreaType == SimObject.SimType.Elevator)
            {
                target.DrawImage(image, location.X + 58, location.Y, image.Width, image.Height);
            }
            else
            {
                if (drawMe)
                {
                    target.DrawImage(image, location.X, location.Y + 25, image.Width, image.Height);
                }
            }
        }



    }
}
