using System.Text;
using CommunityToolkit.Maui.Storage;
using Plugin.Txt2KML;

namespace Txt2KML.Sample;

/// <summary>
/// App-side export helper: opens a native "save as" dialog (CommunityToolkit.Maui
/// <see cref="FileSaver"/>) so the user can name and place the generated KML.
/// Kept in the app rather than the library so the library only depends on MAUI Essentials.
/// </summary>
public static class KmlExporter
{
    /// <summary>
    /// Prompts the user for a location and saves the converted KML.
    /// </summary>
    /// <returns>The saved file path, or <c>null</c> if the user cancelled.</returns>
    public static async Task<string?> SaveAsync(
        KmlConversionResult result,
        string? fileName = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(result);

        var name = string.IsNullOrWhiteSpace(fileName) ? result.SuggestedFileName : fileName!;
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(result.Kml));

        var saveResult = await FileSaver.Default
            .SaveAsync(name, stream, cancellationToken)
            .ConfigureAwait(false);

        return saveResult.IsSuccessful ? saveResult.FilePath : null;
    }
}
