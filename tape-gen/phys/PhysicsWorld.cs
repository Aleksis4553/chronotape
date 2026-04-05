using System;
using System.Collections.Generic;

namespace Phys
{
    public class PhysicsWorld
    {
        // A list to hold every plane in your world
        public List<Plane> Planes { get; private set; }

        public PhysicsWorld()
        {
            Planes = new List<Plane>();
        }

        public void AddPlane(Plane plane)
        {
            Planes.Add(plane);
        }



    }
}