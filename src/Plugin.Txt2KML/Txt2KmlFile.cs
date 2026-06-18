namespace Plugin.Txt2KML;

/// <summary>
/// File-oriented conversion helpers. These work with plain file system paths and
/// have no MAUI dependency, so they can be driven directly from a view model
/// (for example, when a file path is already known).
/// </summary>
public static class Txt2KmlFile
{
    /// <summary>
    /// Derives the KML output path for a given text file path by replacing the
    /// extension with <c>.kml</c> (e.g. <c>waypoints.txt</c> → <c>waypoints.kml</c>).
    /// </summary>
    public static string GetKmlPath(string textFilePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(textFilePath);
        return Path.ChangeExtension(textFilePath, ".kml");
    }

    /// <summary>
    /// Reads a text file, converts it to KML, and writes the result.
    /// </summary>
    /// <param name="textFilePath">Path to the source <c>.txt</c> file.</param>
    /// <param name="outputPath">
    /// Optional explicit output path. When omitted, the source path is used with its
    /// extension replaced by <c>.kml</c>.
    /// </param>
    /// <param name="documentName">
    /// Optional KML document name. Defaults to the source file name (without extension).
    /// </param>
    /// <returns>The path the KML file was written to.</returns>
    public static string ConvertFile(string textFilePath, string? outputPath = null, string? documentName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(textFilePath);

        var text = File.ReadAllText(textFilePath);
        var output = outputPath ?? GetKmlPath(textFilePath);
        var kml = Txt2Kml.Convert(text, documentName ?? Path.GetFileNameWithoutExtension(textFilePath));
        File.WriteAllText(output, kml);
        return output;
    }

    /// <inheritdoc cref="ConvertFile(string, string?, string?)"/>
    public static async Task<string> ConvertFileAsync(
        string textFilePath,
        string? outputPath = null,
        string? documentName = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(textFilePath);

        var text = await File.ReadAllTextAsync(textFilePath, cancellationToken).ConfigureAwait(false);
        var output = outputPath ?? GetKmlPath(textFilePath);
        var kml = Txt2Kml.Convert(text, documentName ?? Path.GetFileNameWithoutExtension(textFilePath));
        await File.WriteAllTextAsync(output, kml, cancellationToken).ConfigureAwait(false);
        return output;
    }
}
