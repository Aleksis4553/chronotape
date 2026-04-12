using System;
namespace Phys
{
    public struct Plane
    {
        public Vector3D Normal { get; set; }

        public Point3D Point { get; set; }

        public Plane(Vector3D normal, Point3D point)
        {
            Normal = normal;
            Point = point;
        }
    }
}