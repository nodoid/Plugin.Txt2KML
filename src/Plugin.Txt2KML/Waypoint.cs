namespace Plugin.Txt2KML;

/// <summary>
/// A single geographic point to be written to KML.
/// </summary>
/// <param name="Latitude">Latitude in decimal degrees (-90..90).</param>
/// <param name="Longitude">Longitude in decimal degrees (-180..180).</param>
/// <param name="Name">Optional placemark name.</param>
/// <param name="Description">Optional placemark description.</param>
/// <param name="Altitude">Optional altitude in metres.</param>
public sealed record Waypoint(
    double Latitude,
    double Longitude,
    string? Name = null,
    string? Description = null,
    double? Altitude = null);
