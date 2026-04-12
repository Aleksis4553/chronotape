using Phys;
using System.Collections.Generic;

// You need a class wrapper here
internal class ProjectionUtils
{
    public static bool[][] BuildSourceBitmap(int width, int height, IReadOnlyList<ProjectedPoint> points)
    {
        bool[][] bitmap = BuildEmptyBitmap(width, height);

        foreach (ProjectedPoint point in points)
        {
            if (point.PixelY < 0 || point.PixelY >= height || point.PixelX < 0 || point.PixelX >= width)
            {
                continue;
            }

            bitmap[point.PixelY][point.PixelX] = true;
        }

        return bitmap;
    }

    // Assuming this helper method exists or needs to be defined
    private static bool[][] BuildEmptyBitmap(int width, int height)
    {
        bool[][] bitmap = new bool[height][];
        for (int i = 0; i < height; i++)
        {
            bitmap[i] = new bool[width];
        }
        return bitmap;
    }


}