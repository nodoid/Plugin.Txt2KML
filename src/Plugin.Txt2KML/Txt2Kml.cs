using System.Globalization;
using System.Xml.Linq;

namespace Plugin.Txt2KML;

/// <summary>
/// Converts simple delimited text into KML (Keyhole Markup Language) documents.
///
/// <para>Input format: one waypoint per line, comma-separated:</para>
/// <code>latitude,longitude[,name][,description]</code>
/// <para>
/// Blank lines and lines beginning with '#' are ignored. Whitespace around
/// fields is trimmed. Numbers are parsed using the invariant culture.
/// </para>
/// </summary>
public static class Txt2Kml
{
    private static readonly XNamespace Kml = "http://www.opengis.net/kml/2.2";

    /// <summary>
    /// Parses delimited text into a list of <see cref="Waypoint"/>s.
    /// </summary>
    /// <exception cref="FormatException">A non-comment line is malformed.</exception>
    public static IReadOnlyList<Waypoint> Parse(string? text)
    {
        var waypoints = new List<Waypoint>();
        if (string.IsNullOrWhiteSpace(text))
            return waypoints;

        var lines = text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        for (var i = 0; i < lines.Length; i++)
        {
            var raw = lines[i].Trim();
            if (raw.Length == 0 || raw[0] == '#')
                continue;

            var fields = raw.Split(',');
            if (fields.Length < 2)
                throw new FormatException(
                    $"Line {i + 1}: expected at least 'latitude,longitude' but got '{raw}'.");

            if (!TryParseCoordinate(fields[0], out var lat) || lat is < -90 or > 90)
                throw new FormatException($"Line {i + 1}: invalid latitude '{fields[0].Trim()}'.");

            if (!TryParseCoordinate(fields[1], out var lon) || lon is < -180 or > 180)
                throw new FormatException($"Line {i + 1}: invalid longitude '{fields[1].Trim()}'.");

            var name = fields.Length > 2 ? Nullify(fields[2]) : null;
            var description = fields.Length > 3 ? Nullify(string.Join(",", fields[3..])) : null;

            waypoints.Add(new Waypoint(lat, lon, name, description));
        }

        return waypoints;
    }

    /// <summary>
    /// Parses delimited text and returns a complete KML document as a string.
    /// </summary>
    public static string Convert(string? text, string? documentName = null)
        => ToKml(Parse(text), documentName);

    /// <summary>
    /// Builds a complete KML document from a set of waypoints.
    /// </summary>
    public static string ToKml(IEnumerable<Waypoint> waypoints, string? documentName = null)
    {
        ArgumentNullException.ThrowIfNull(waypoints);

        var document = new XElement(Kml + "Document");
        if (!string.IsNullOrWhiteSpace(documentName))
            document.Add(new XElement(Kml + "name", documentName));

        foreach (var wp in waypoints)
            document.Add(BuildPlacemark(wp));

        var kml = new XElement(Kml + "kml", document);
        var doc = new XDocument(new XDeclaration("1.0", "utf-8", null), kml);

        using var writer = new StringWriter();
        doc.Save(writer);
        return writer.ToString();
    }

    private static XElement BuildPlacemark(Waypoint wp)
    {
        var placemark = new XElement(Kml + "Placemark");
        if (!string.IsNullOrWhiteSpace(wp.Name))
            placemark.Add(new XElement(Kml + "name", wp.Name));
        if (!string.IsNullOrWhiteSpace(wp.Description))
            placemark.Add(new XElement(Kml + "description", wp.Description));

        placemark.Add(new XElement(Kml + "Point",
            new XElement(Kml + "coordinates", FormatCoordinates(wp))));

        return placemark;
    }

    // KML coordinate order is longitude,latitude[,altitude].
    private static string FormatCoordinates(Waypoint wp)
    {
        var lon = wp.Longitude.ToString(CultureInfo.InvariantCulture);
        var lat = wp.Latitude.ToString(CultureInfo.InvariantCulture);
        var alt = (wp.Altitude ?? 0d).ToString(CultureInfo.InvariantCulture);
        return $"{lon},{lat},{alt}";
    }

    private static bool TryParseCoordinate(string value, out double result)
        => double.TryParse(value.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out result);

    private static string? Nullify(string value)
    {
        var trimmed = value.Trim();
        return trimmed.Length == 0 ? null : trimmed;
    }
}
