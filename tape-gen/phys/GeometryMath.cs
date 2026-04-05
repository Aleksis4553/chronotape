using System;

namespace Phys
{
    public static class GeometryMath
    {
        /// <summary>
        /// Casts a ray from a start point, through a mid-point, to find an intersection on a target plane.
        /// </summary>
        public static bool GetProjectionPoint(Point3D startPoint, Point3D pointOnPlaneA, Plane targetPlane, out Point3D finalPoint)
        {
            // 1. Calculate the Direction Vector (Point A - Start Point)
            Vector3D direction = new Vector3D(
                pointOnPlaneA.X - startPoint.X,
                pointOnPlaneA.Y - startPoint.Y,
                pointOnPlaneA.Z - startPoint.Z
            );

            // 2. Calculate Dot Product of Direction and the Target Plane's Normal
            // This tells us if the ray is pointing at the plane, or running parallel to it.
            double dotDirNormal = (direction.X * targetPlane.Normal.X) +
                                  (direction.Y * targetPlane.Normal.Y) +
                                  (direction.Z * targetPlane.Normal.Z);

            // If the dot product is exactly 0, the ray is parallel to the plane and will never hit it.
            if (Math.Abs(dotDirNormal) < 0.000001)
            {
                finalPoint = default;
                return false;
            }

            // 3. Find the vector from the Start Point to the known point on the Target Plane
            Vector3D originToPlane = new Vector3D(
                targetPlane.Point.X - startPoint.X,
                targetPlane.Point.Y - startPoint.Y,
                targetPlane.Point.Z - startPoint.Z
            );

            // Calculate the Dot Product of this new vector and the plane normal
            double dotOriginNormal = (originToPlane.X * targetPlane.Normal.X) +
                                     (originToPlane.Y * targetPlane.Normal.Y) +
                                     (originToPlane.Z * targetPlane.Normal.Z);

            // 4. Calculate 't' (the distance factor along the ray)
            double t = dotOriginNormal / dotDirNormal;

            // If t is negative, the target plane is actually BEHIND the starting point.
            if (t < 0)
            {
                finalPoint = default;
                return false;
            }

            // 5. Calculate the final coordinate: StartPoint + (Direction * t)
            finalPoint = new Point3D(
                startPoint.X + (direction.X * t),
                startPoint.Y + (direction.Y * t),
                startPoint.Z + (direction.Z * t)
            );

            return true; // We successfully hit the target plane!
        }
    }
}