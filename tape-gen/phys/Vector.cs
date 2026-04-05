using System;

namespace Phys
{
    public struct Vector3D
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vector3D(double x = 0, double y = 0, double z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public Vector3D(Point3D start, Point3D end)
        {
            X = end.X - start.X;
            Y = end.Y - start.Y;
            Z = end.Z - start.Z;
        }
        public static Vector3D Cross(Vector3D a, Vector3D b) => new Vector3D(
    a.Y * b.Z - a.Z * b.Y,
    a.Z * b.X - a.X * b.Z,
    a.X * b.Y - a.Y * b.X
);

        public static double Dot(Vector3D a, Vector3D b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

        // Builds a stable "up" vector lying in the plane of a frame given its normal and a hint direction.
        // (Projects the hint onto the plane.)
        public static Vector3D MakeUpInPlane(Vector3D normal, Vector3D hint)
        {
            // up = hint - normal * dot(hint, normal)
            double d = Dot(hint, normal);
            Vector3D up = new Vector3D(
                hint.X - normal.X * d,
                hint.Y - normal.Y * d,
                hint.Z - normal.Z * d
            );

            // If hint was parallel to normal, fall back to another hint.
            double upLen2 = Dot(up, up);
            if (upLen2 < 1e-12)
            {
                // try X axis as fallback
                Vector3D hint2 = new Vector3D(1, 0, 0);
                d = Dot(hint2, normal);
                up = new Vector3D(
                    hint2.X - normal.X * d,
                    hint2.Y - normal.Y * d,
                    hint2.Z - normal.Z * d
                );
            }

            return up;
        }
    }
}