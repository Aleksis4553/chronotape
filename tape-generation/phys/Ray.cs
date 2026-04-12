using System;

namespace Phys
{
    public struct Ray
    {
        public Point3D Origin { get; set; }

        public Vector3D Direction { get; set; }

        public Ray(Point3D origin, Vector3D direction)
        {
            Origin = origin;
            Direction = direction;
        }
        public override string ToString()
        {
            return $"Ray [Origin: {Origin}, Direction: {Direction}]";
        }

    }

}