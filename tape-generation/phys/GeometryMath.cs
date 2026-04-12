using System;
using System.Collections.Generic;

namespace Phys
{
    public static class GeometryMath
    {
        /// <summary>
        /// Finds the point in 3D space that is closest to all of the given rays (least-squares
        /// line intersection). For a perfect point light source all rays will converge exactly;
        /// in the general case this returns the best-fit intersection.
        /// Returns false when the system is under-determined (fewer than 2 rays, or all rays
        /// are parallel).
        /// </summary>
        public static bool GetClosestPointToRays(IList<Ray> rays, out Point3D intersection)
        {
            if (rays.Count < 2)
            {
                intersection = default;
                Console.WriteLine("Too little rays to calculate the point, must be >=2 ");
                return false;
            }

            // Build the 3x3 normal-equation system  A·P = b
            //   A = Σ  (I − d̂ᵢ d̂ᵢᵀ)
            //   b = Σ  (I − d̂ᵢ d̂ᵢᵀ) oᵢ
            // where d̂ᵢ is the unit direction of ray i and oᵢ its origin.
            double a00 = 0, a01 = 0, a02 = 0;
            double a10 = 0, a11 = 0, a12 = 0;
            double a20 = 0, a21 = 0, a22 = 0;
            double b0 = 0, b1 = 0, b2 = 0;

            foreach (var ray in rays)
            {
                double len = Math.Sqrt(
                    ray.Direction.X * ray.Direction.X +
                    ray.Direction.Y * ray.Direction.Y +
                    ray.Direction.Z * ray.Direction.Z);

                if (len < 1e-10) continue;

                double dx = ray.Direction.X / len;
                double dy = ray.Direction.Y / len;
                double dz = ray.Direction.Z / len;

                // Row of  (I − d d^T):
                double m00 = 1 - dx * dx, m01 = -dx * dy, m02 = -dx * dz;
                double m10 = -dy * dx, m11 = 1 - dy * dy, m12 = -dy * dz;
                double m20 = -dz * dx, m21 = -dz * dy, m22 = 1 - dz * dz;

                double ox = ray.Origin.X, oy = ray.Origin.Y, oz = ray.Origin.Z;

                a00 += m00; a01 += m01; a02 += m02;
                a10 += m10; a11 += m11; a12 += m12;
                a20 += m20; a21 += m21; a22 += m22;

                b0 += m00 * ox + m01 * oy + m02 * oz;
                b1 += m10 * ox + m11 * oy + m12 * oz;
                b2 += m20 * ox + m21 * oy + m22 * oz;
            }

            // Solve with Gaussian elimination (augmented matrix [A | b]).
            double[,] aug = {
                { a00, a01, a02, b0 },
                { a10, a11, a12, b1 },
                { a20, a21, a22, b2 }
            };

            for (int col = 0; col < 3; col++)
            {
                // Partial pivoting
                int pivot = col;
                double maxVal = Math.Abs(aug[col, col]);
                for (int row = col + 1; row < 3; row++)
                {
                    double v = Math.Abs(aug[row, col]);
                    if (v > maxVal) { maxVal = v; pivot = row; }
                }

                if (maxVal < 1e-10)
                {
                    intersection = default;
                    return false;
                }

                if (pivot != col)
                {
                    for (int j = 0; j <= 3; j++)
                    {
                        double tmp = aug[col, j];
                        aug[col, j] = aug[pivot, j];
                        aug[pivot, j] = tmp;
                    }
                }

                for (int row = col + 1; row < 3; row++)
                {
                    double factor = aug[row, col] / aug[col, col];
                    for (int j = col; j <= 3; j++)
                        aug[row, j] -= factor * aug[col, j];
                }
            }

            // Back substitution
            double[] x = new double[3];
            for (int i = 2; i >= 0; i--)
            {
                x[i] = aug[i, 3];
                for (int j = i + 1; j < 3; j++)
                    x[i] -= aug[i, j] * x[j];
                x[i] /= aug[i, i];
            }

            intersection = new Point3D(x[0], x[1], x[2]);
            return true;
        }

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