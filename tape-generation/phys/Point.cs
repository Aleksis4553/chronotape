using System;

namespace Phys
{
    public struct Point3D
    {

        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        public Point3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3D operator -(Point3D left, Point3D right)
        => new Vector3D(
            right.X - left.X,
            right.Y - left.Y,
            right.Z - left.Z);

        public override string ToString()
        {
            return $"({X.ToString("0.00")}, {Y.ToString("0.00")}, {Z.ToString("0.00")})";
        }

    }
}