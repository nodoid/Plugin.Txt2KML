namespace Plugin.Txt2KML;

/// <summary>
/// Thrown when a line of input text cannot be parsed into a <see cref="Waypoint"/>.
/// Derives from <see cref="FormatException"/> so existing <c>catch (FormatException)</c>
/// handlers keep working, while exposing the offending <see cref="LineNumber"/>.
/// </summary>
public sealed class Txt2KmlFormatException : FormatException
{
    /// <summary>The 1-based line number that failed to parse, or 0 if not line-specific.</summary>
    public int LineNumber { get; }

    public Txt2KmlFormatException(int lineNumber, string message)
        : base(message)
    {
        LineNumber = lineNumber;
    }

    public Txt2KmlFormatException(int lineNumber, string message, Exception innerException)
        : base(message, innerException)
    {
        LineNumber = lineNumber;
    }
}
