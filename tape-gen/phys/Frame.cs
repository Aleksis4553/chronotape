using System;

namespace Phys
{
    public struct Frame
    {
        public Point3D TopLeft { get; set; }
        public Point3D TopRight { get; set; }
        public Point3D BottomLeft { get; set; }
        public Point3D BottomRight { get; set; }
        public Point3D Center { get; set; }

        /// <summary>
        /// Creates a rectangular frame in 3D space.
        /// </summary>
        /// <param name="center">The exact middle point of the frame.</param>
        /// <param name="normal">The direction the frame is facing (should be normalized).</param>
        /// <param name="up">Which direction is "top" for the frame (should be normalized).</param>
        /// <param name="width">The total horizontal distance.</param>
        /// <param name="height">The total vertical distance. If left blank, it creates a perfect square.</param>
        public Frame(Point3D center, Vector3D normal, Vector3D up, double width, double height = 0)
        {
            // If height isn't provided, default to a perfect square
            if (height == 0) height = width;

            Center = center;

            // 1. Calculate the 'Right' vector using the Cross Product of Up and Normal
            // This guarantees we have a vector pointing perfectly to the right side of the frame.
            Vector3D right = new Vector3D(
                (up.Y * normal.Z) - (up.Z * normal.Y),
                (up.Z * normal.X) - (up.X * normal.Z),
                (up.X * normal.Y) - (up.Y * normal.X)
            );

            // 2. Find the half-distances to reach the edges from the center
            double halfW = width / 2.0;
            double halfH = height / 2.0;

            // 3. Calculate the corners by starting at the center, and moving Up/Down and Left/Right

            // Top Right: Center + (Right * halfW) + (Up * halfH)
            TopRight = new Point3D(
                center.X + (right.X * halfW) + (up.X * halfH),
                center.Y + (right.Y * halfW) + (up.Y * halfH),
                center.Z + (right.Z * halfW) + (up.Z * halfH)
            );

            // Top Left: Center - (Right * halfW) + (Up * halfH)
            TopLeft = new Point3D(
                center.X - (right.X * halfW) + (up.X * halfH),
                center.Y - (right.Y * halfW) + (up.Y * halfH),
                center.Z - (right.Z * halfW) + (up.Z * halfH)
            );

            // Bottom Right: Center + (Right * halfW) - (Up * halfH)
            BottomRight = new Point3D(
                center.X + (right.X * halfW) - (up.X * halfH),
                center.Y + (right.Y * halfW) - (up.Y * halfH),
                center.Z + (right.Z * halfW) - (up.Z * halfH)
            );

            // Bottom Left: Center - (Right * halfW) - (Up * halfH)
            BottomLeft = new Point3D(
                center.X - (right.X * halfW) - (up.X * halfH),
                center.Y - (right.Y * halfW) - (up.Y * halfH),
                center.Z - (right.Z * halfW) - (up.Z * halfH)
            );
        }
    }
}