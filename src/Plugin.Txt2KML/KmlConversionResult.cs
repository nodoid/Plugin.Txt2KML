namespace Plugin.Txt2KML;

/// <summary>
/// The outcome of converting a text source into KML.
/// </summary>
/// <param name="Kml">The generated KML document.</param>
/// <param name="SuggestedFileName">A suggested output file name (source name with a .kml extension).</param>
/// <param name="WaypointCount">The number of waypoints found in the source.</param>
/// <param name="SourceFileName">The original source file name, when known.</param>
public sealed record KmlConversionResult(
    string Kml,
    string SuggestedFileName,
    int WaypointCount,
    string? SourceFileName = null);
