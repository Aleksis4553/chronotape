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

        public Frame(Point3D center, Vector3D normal, Vector3D up, double width, double height = 0)
        {
            if (height == 0) height = width;

            Center = center;

            Vector3D right = new Vector3D(
                (up.Y * normal.Z) - (up.Z * normal.Y),
                (up.Z * normal.X) - (up.X * normal.Z),
                (up.X * normal.Y) - (up.Y * normal.X)
            );

            double halfW = width / 2.0;
            double halfH = height / 2.0;

            TopRight = new Point3D(
                center.X + (right.X * halfW) + (up.X * halfH),
                center.Y + (right.Y * halfW) + (up.Y * halfH),
                center.Z + (right.Z * halfW) + (up.Z * halfH)
            );

            TopLeft = new Point3D(
                center.X - (right.X * halfW) + (up.X * halfH),
                center.Y - (right.Y * halfW) + (up.Y * halfH),
                center.Z - (right.Z * halfW) + (up.Z * halfH)
            );

            BottomRight = new Point3D(
                center.X + (right.X * halfW) - (up.X * halfH),
                center.Y + (right.Y * halfW) - (up.Y * halfH),
                center.Z + (right.Z * halfW) - (up.Z * halfH)
            );

            BottomLeft = new Point3D(
                center.X - (right.X * halfW) - (up.X * halfH),
                center.Y - (right.Y * halfW) - (up.Y * halfH),
                center.Z - (right.Z * halfW) - (up.Z * halfH)
            );
        }

        /// <summary>
        /// Maps a 2D pixel coordinate to a 3D point precisely on the surface of this Frame.
        /// </summary>
        public Point3D MapPixelTo3D(int pixelX, int pixelY, int width, int height)
        {
            double u = width > 1 ? (double)pixelX / (width - 1) : 0;
            double v = height > 1 ? (double)pixelY / (height - 1) : 0;

            Vector3D topEdge = new Vector3D(TopRight.X - TopLeft.X, TopRight.Y - TopLeft.Y, TopRight.Z - TopLeft.Z);
            Vector3D bottomEdge = new Vector3D(BottomRight.X - BottomLeft.X, BottomRight.Y - BottomLeft.Y, BottomRight.Z - BottomLeft.Z);

            Point3D topPoint = new Point3D(
                TopLeft.X + topEdge.X * u,
                TopLeft.Y + topEdge.Y * u,
                TopLeft.Z + topEdge.Z * u);

            Point3D bottomPoint = new Point3D(
                BottomLeft.X + bottomEdge.X * u,
                BottomLeft.Y + bottomEdge.Y * u,
                BottomLeft.Z + bottomEdge.Z * u);

            Vector3D verticalEdge = new Vector3D(
                bottomPoint.X - topPoint.X,
                bottomPoint.Y - topPoint.Y,
                bottomPoint.Z - topPoint.Z);

            return new Point3D(
                topPoint.X + verticalEdge.X * v,
                topPoint.Y + verticalEdge.Y * v,
                topPoint.Z + verticalEdge.Z * v);
        }

        /// <summary>
        /// Maps a 3D point resting on this Frame back to a 2D pixel coordinate.
        /// </summary>
        internal ProjectedPoint Map3DToPixel(Point3D point3D, int targetWidth, int targetHeight)
        {
            Vector3D pointVector = new Vector3D(
                point3D.X - TopLeft.X,
                point3D.Y - TopLeft.Y,
                point3D.Z - TopLeft.Z);

            Vector3D right = new Vector3D(
                TopRight.X - TopLeft.X,
                TopRight.Y - TopLeft.Y,
                TopRight.Z - TopLeft.Z);

            Vector3D down = new Vector3D(
                BottomLeft.X - TopLeft.X,
                BottomLeft.Y - TopLeft.Y,
                BottomLeft.Z - TopLeft.Z);

            double rightMagSq = (right.X * right.X) + (right.Y * right.Y) + (right.Z * right.Z);
            double downMagSq = (down.X * down.X) + (down.Y * down.Y) + (down.Z * down.Z);

            double dotRight = (pointVector.X * right.X) + (pointVector.Y * right.Y) + (pointVector.Z * right.Z);
            double dotDown = (pointVector.X * down.X) + (pointVector.Y * down.Y) + (pointVector.Z * down.Z);

            double u = rightMagSq > 0 ? dotRight / rightMagSq : 0;
            double v = downMagSq > 0 ? dotDown / downMagSq : 0;

            int px = (int)Math.Round(u * (targetWidth - 1));
            int py = (int)Math.Round(v * (targetHeight - 1));

            return new ProjectedPoint { PixelX = px, PixelY = py };
        }
    }
}