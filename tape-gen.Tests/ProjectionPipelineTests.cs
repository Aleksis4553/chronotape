using Phys;
using Xunit;

public sealed class ProjectionPipelineTests
{
    [Fact]
    public void ProjectThroughSlitGlyphToDisplay_ProjectsCenterPixelToDisplayCenter()
    {
        Frame slit = new(
            new Point3D(0, 0, 0),
            new Vector3D(0, 0, 1),
            new Vector3D(0, 1, 0),
            width: 2,
            height: 2);
        Frame display = new(
            new Point3D(0, 0, 10),
            new Vector3D(0, 0, 1),
            new Vector3D(0, 1, 0),
            width: 4,
            height: 4);
        var pixels = new List<SampledPixel>
        {
            new SampledPixel { X = 0, Y = 0, BitmapWidth = 1, BitmapHeight = 1 }
        };

        SlitProjectionResult result = ProjectionPipeline.ProjectThroughSlitGlyphToDisplay(
            slitIndex: 0,
            sampledPixels: pixels,
            slit: slit,
            display: display,
            lightSource: new Point3D(0, 0, -10));

        Assert.Single(result.Points);
        ProjectedPoint point = result.Points[0];
        Assert.Equal(0, point.DisplayLocalX, 6);
        Assert.Equal(0, point.DisplayLocalY, 6);
        Assert.Equal(0, point.DisplayWorldX, 6);
        Assert.Equal(0, point.DisplayWorldY, 6);
        Assert.Equal(10, point.DisplayWorldZ, 6);
    }

    [Fact]
    public void ProjectThroughSlitGlyphToDisplay_ProjectsOffCenterPixelToExpectedDisplayLocation()
    {
        Frame slit = new(
            new Point3D(0, 0, 0),
            new Vector3D(0, 0, 1),
            new Vector3D(0, 1, 0),
            width: 2,
            height: 2);
        Frame display = new(
            new Point3D(0, 0, 10),
            new Vector3D(0, 0, 1),
            new Vector3D(0, 1, 0),
            width: 4,
            height: 4);
        var pixels = new List<SampledPixel>
        {
            new SampledPixel { X = 1, Y = 0, BitmapWidth = 2, BitmapHeight = 2 }
        };

        SlitProjectionResult result = ProjectionPipeline.ProjectThroughSlitGlyphToDisplay(
            slitIndex: 0,
            sampledPixels: pixels,
            slit: slit,
            display: display,
            lightSource: new Point3D(0, 0, -10));

        Assert.Single(result.Points);
        ProjectedPoint point = result.Points[0];
        Assert.Equal(0.5, point.SlitLocalX, 6);
        Assert.Equal(0.5, point.SlitLocalY, 6);
        Assert.Equal(1, point.DisplayLocalX, 6);
        Assert.Equal(1, point.DisplayLocalY, 6);
        Assert.Equal(1, point.DisplayWorldX, 6);
        Assert.Equal(1, point.DisplayWorldY, 6);
        Assert.Equal(10, point.DisplayWorldZ, 6);
    }

    [Fact]
    public void ProjectThroughSlitGlyphToDisplay_PreservesPixelAspectForNonSquareBitmapWithoutSlitStretching()
    {
        Frame slit = new(
            new Point3D(0, 0, 0),
            new Vector3D(0, 0, 1),
            new Vector3D(0, 1, 0),
            width: 8,
            height: 2);
        Frame display = new(
            new Point3D(0, 0, 10),
            new Vector3D(0, 0, 1),
            new Vector3D(0, 1, 0),
            width: 20,
            height: 20);
        var pixels = new List<SampledPixel>
        {
            new SampledPixel { X = 0, Y = 2, BitmapWidth = 2, BitmapHeight = 4 },
            new SampledPixel { X = 1, Y = 2, BitmapWidth = 2, BitmapHeight = 4 },
            new SampledPixel { X = 0, Y = 1, BitmapWidth = 2, BitmapHeight = 4 }
        };

        SlitProjectionResult result = ProjectionPipeline.ProjectThroughSlitGlyphToDisplay(
            slitIndex: 0,
            sampledPixels: pixels,
            slit: slit,
            display: display,
            lightSource: new Point3D(0, 0, -10));

        Assert.Equal(3, result.Points.Count);

        ProjectedPoint centerLeft = result.Points[0];
        ProjectedPoint centerRight = result.Points[1];
        ProjectedPoint topLeft = result.Points[2];

        Assert.Equal(-0.5, centerLeft.SlitLocalX, 6);
        Assert.Equal(-0.5, centerLeft.SlitLocalY, 6);
        Assert.Equal(0.5, centerRight.SlitLocalX, 6);
        Assert.Equal(-0.5, centerRight.SlitLocalY, 6);
        Assert.Equal(-0.5, topLeft.SlitLocalX, 6);
        Assert.Equal(0.5, topLeft.SlitLocalY, 6);

        Assert.Equal(2.0, centerRight.DisplayLocalX - centerLeft.DisplayLocalX, 6);
        Assert.Equal(2.0, topLeft.DisplayLocalY - centerLeft.DisplayLocalY, 6);
        Assert.Equal(centerLeft.DisplayLocalY, centerRight.DisplayLocalY, 6);
        Assert.Equal(centerLeft.DisplayLocalX, topLeft.DisplayLocalX, 6);
    }
}
