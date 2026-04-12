using System;
using System.IO;
using System.Text.Json;
using Phys;

namespace Configuration
{
    // --- Data Models ---

    public class PathsConfig
    {
        public required string TapeOutput { get; set; }
        public required string DeadzoneFont { get; set; }
        public required string MainFont { get; set; }
        public required string Render { get; set; }
    }

    public class TapeConfig
    {
        public required string DeadzoneCharacters { get; set; }
        public required string MainCharacters { get; set; }
        public int Offset { get; set; }
        public int Dpi { get; set; }
        public double MainGlyphFontSizeMm { get; set; }
        public double DeadGlyphFontSizeMm { get; set; }

        public double SegmentWidthMm { get; set; }
        public double SegmentHeightMm { get; set; }
        public double TopMarginMm { get; set; }
        public double MainHorizontalPaddingMm { get; set; }
        public double MainVerticalPaddingMm { get; set; }
        public double SlitCenterYOffsetMm { get; set; }
    }

    public class WorldGeometryConfig
    {
        public double SlitWidthMm { get; set; }
        public double SlitHeightMm { get; set; }
        public int SlitCount { get; set; }
        public double SlitSegmentCenterDistanceMm { get; set; }
        public double TapeTopHeightFromGroundMm { get; set; }
        public double DisplayedSegmentWidthMm { get; set; }
        public double DisplayedSegmentHeightMm { get; set; }
        public double DisplayedSegmentCenterDistanceMm { get; set; }

        public Point3D TapeOriginMm { get; set; }
        public Vector3D SlitDirection { get; set; }
        public Vector3D SlitNormal { get; set; }
        public Vector3D SlitUpDirection { get; set; }

        public Point3D DisplayPlanePointMm { get; set; }
        public Vector3D DisplayPlaneNormal { get; set; }
        public Vector3D DisplayPlaneUpDirection { get; set; }

    }

    // --- Configuration Manager ---

    // ... [Keep all the data classes here: PathsConfig, TapeConfig, etc.] ...

    public static class Config
    {
        // Static properties replace the Singleton Instance
        public static PathsConfig Paths { get; private set; } = null!;
        public static TapeConfig Tape { get; private set; } = null!;
        public static WorldGeometryConfig WorldGeometry { get; private set; } = null!;

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        /// <summary>
        /// Loads all configuration files. Call this at the start of your application.
        /// </summary>
        public static void LoadAll(string pathsFilePath, string tapeFilePath, string geometryFilePath)
        {
            Paths = LoadConfig<PathsConfig>(pathsFilePath);
            Tape = LoadConfig<TapeConfig>(tapeFilePath);
            WorldGeometry = LoadConfig<WorldGeometryConfig>(geometryFilePath);
        }

        private static T LoadConfig<T>(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Configuration file not found: {filePath}");
            }

            try
            {
                string jsonString = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<T>(jsonString, _jsonOptions)
                       ?? throw new InvalidOperationException($"Deserialization returned null for {filePath}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load configuration from {filePath}: {ex.Message}", ex);
            }
        }
    }
}