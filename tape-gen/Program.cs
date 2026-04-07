using System;
using System.Collections.Generic;
using Phys;

const double DISPLAYED_WIDTH = 150;
const double DISPLAYED_HEIGHT = 300;
const double DISPLAYED_SEGMENT_CENTER_DISTANCE = 160;

const double SLIT_WIDTH = 5;
const double SLIT_HEIGHT = 10;
const double SLIT_SEGMENT_CENTER_DISTANCE = 50;

const int SLIT_AMOUNT = 4;

const double TAPE_TOP_HEIGHT_FROM_GROUND = 0;
/*
 *          +Y (Up)
 *             ^
 *             |
 *             |
 *             |
 *   -X <------+------> +X (Right)
 *             | (0,0,0)
 *             |
 *             |
 *             v
 *           -Y
 *
 */

Point3D chronotapeFrameOrigin = new Point3D(0, 0, 0);
Vector3D slitFramesDirection = new Vector3D(1, 0, 0);
Vector3D slitFramesUp = new Vector3D(0, 1, 0);
Vector3D slitFrameNormal = new Vector3D(0, 0, 1);

Vector3D surfaceNormal = new Vector3D(0, 0, 1);
Point3D surfacePoint = new Point3D(0, 0, 2000);
Plane displaySurface = new Plane(surfaceNormal, surfacePoint);

var slits = new List<Frame>();
// Calculate the exact middle index of the slit sequence
// For 4 slits, this is (4 - 1) / 2.0 = 1.5
double middleIndex = (SLIT_AMOUNT - 1) / 2.0;

for (int i = 0; i < SLIT_AMOUNT; i++)
{
    double currentOffset = (i - middleIndex) * SLIT_SEGMENT_CENTER_DISTANCE;

    Point3D slitCenter = new Point3D(
        chronotapeFrameOrigin.X + (slitFramesDirection.X * currentOffset),

        chronotapeFrameOrigin.Y + (slitFramesDirection.Y * currentOffset),

        chronotapeFrameOrigin.Z + (slitFramesDirection.Z * currentOffset)
        + TAPE_TOP_HEIGHT_FROM_GROUND
    );

    // 4. Create and add the frame
    Vector3D slitFrameUp = new Vector3D(0, 1, 0);

    Frame newSlit = new Frame(
        slitCenter,
        slitFrameNormal,
        slitFrameUp,
        SLIT_WIDTH,
        SLIT_HEIGHT
    );

    slits.Add(newSlit);
}

// --- Quick test ---
foreach (var slit in slits)
{
    Console.WriteLine($"X: {slit.Center.X} Y: {slit.Center.Y}  Z: {slit.Center.Z} ");
}

// --- Ceiling Displayed Segments Setup ---

// Calculate the "Up" direction for frames lying flat on the display surface,
// aligned with the tape track direction.
Vector3D surfaceUp = Vector3D.Cross(displaySurface.Normal, slitFramesDirection);

var displayedSegments = new List<Frame>();

for (int i = 0; i < SLIT_AMOUNT; i++)
{
    double currentOffset = (i - middleIndex) * DISPLAYED_SEGMENT_CENTER_DISTANCE;

    // Place each display frame center directly on the display surface at the appropriate offset
    // along the track direction. No light-source assumption is needed here.
    Point3D segmentCenter = new Point3D(
        displaySurface.Point.X + (slitFramesDirection.X * currentOffset),
        displaySurface.Point.Y + (slitFramesDirection.Y * currentOffset),
        displaySurface.Point.Z + (slitFramesDirection.Z * currentOffset)
    );

    Frame newSegment = new Frame(
        segmentCenter,
        displaySurface.Normal,
        surfaceUp,
        DISPLAYED_WIDTH,
        DISPLAYED_HEIGHT
    );

    displayedSegments.Add(newSegment);
}

// --- Quick test for Ceiling Segments ---
Console.WriteLine("\n--- Displayed Segments ---");
foreach (var segment in displayedSegments)
{
    Console.WriteLine($"X: {segment.Center.X}  Y: {segment.Center.Y}  Z: {segment.Center.Z}");
}

// --- Light Source Position Detection ---
// For each (display frame, tape slit) pair, cast rays from each display frame corner
// through the corresponding tape frame corner. Where these rays converge is the
// light source position — no predefined light surface plane needed.

Console.WriteLine("\n--- Light Source Positions ---");
for (int i = 0; i < SLIT_AMOUNT; i++)
{
    Frame display = displayedSegments[i];
    Frame slit = slits[i];

    var rays = new List<Ray>
    {
        new Ray(display.TopRight,    new Vector3D(display.TopRight,    slit.TopRight)),
        new Ray(display.TopLeft,     new Vector3D(display.TopLeft,     slit.TopLeft)),
        new Ray(display.BottomRight, new Vector3D(display.BottomRight, slit.BottomRight)),
        new Ray(display.BottomLeft,  new Vector3D(display.BottomLeft,  slit.BottomLeft)),
    };

    if (!GeometryMath.GetClosestPointToRays(rays, out Point3D lightSource))
    {
        Console.WriteLine($"Warning: Could not determine light source for slit {i} — rays may be parallel.");
        continue;
    }

    Console.WriteLine($"Slit {i}: X: {lightSource.X:F2}  Y: {lightSource.Y:F2}  Z: {lightSource.Z:F2}");
}