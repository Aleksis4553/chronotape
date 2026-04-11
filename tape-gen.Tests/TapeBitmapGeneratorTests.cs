using SkiaSharp;
using Xunit;

public sealed class TapeBitmapGeneratorTests
{
    [Fact]
    public void GenerateTapeBitmap_UsesDeadzoneApertureForProjectionAndAppliesPaddingAsClip()
    {
        var baselineSpec = new TapeSpec
        {
            SegmentCharacters = "8",
            MainCharacters = "8",
            Offset = 0,
            SlitCount = 1,
            SegmentWidthPx = 140,
            SegmentHeightPx = 210,
            TopMarginPx = 0,
            DeadzoneRectPx = new SKRectI(52, 148, 88, 184),
            FontFamily = "monospace",
            FontStyle = SKFontStyle.Normal,
            ForegroundColor = SKColors.White,
            BackgroundColor = SKColors.Black,
            MainPaddingPx = 8,
            DeadzonePaddingPx = 0
        };

        var clippedSpec = new TapeSpec
        {
            SegmentCharacters = baselineSpec.SegmentCharacters,
            MainCharacters = baselineSpec.MainCharacters,
            Offset = baselineSpec.Offset,
            SlitCount = baselineSpec.SlitCount,
            SegmentWidthPx = baselineSpec.SegmentWidthPx,
            SegmentHeightPx = baselineSpec.SegmentHeightPx,
            TopMarginPx = baselineSpec.TopMarginPx,
            DeadzoneRectPx = baselineSpec.DeadzoneRectPx,
            FontFamily = baselineSpec.FontFamily,
            FontStyle = baselineSpec.FontStyle,
            ForegroundColor = baselineSpec.ForegroundColor,
            BackgroundColor = baselineSpec.BackgroundColor,
            MainPaddingPx = baselineSpec.MainPaddingPx,
            DeadzonePaddingPx = 4
        };

        using SKBitmap baseline = TapeBitmapGenerator.GenerateTapeBitmap(baselineSpec);
        using SKBitmap clipped = TapeBitmapGenerator.GenerateTapeBitmap(clippedSpec);

        SKRectI clipRect = new(
            baselineSpec.DeadzoneRectPx.Left + clippedSpec.DeadzonePaddingPx,
            baselineSpec.DeadzoneRectPx.Top + clippedSpec.DeadzonePaddingPx,
            baselineSpec.DeadzoneRectPx.Right - clippedSpec.DeadzonePaddingPx,
            baselineSpec.DeadzoneRectPx.Bottom - clippedSpec.DeadzonePaddingPx);

        for (int y = clipRect.Top; y < clipRect.Bottom; y++)
        {
            for (int x = clipRect.Left; x < clipRect.Right; x++)
            {
                Assert.Equal(baseline.GetPixel(x, y), clipped.GetPixel(x, y));
            }
        }
    }
}
