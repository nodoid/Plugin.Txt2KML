using Microsoft.Maui.Storage;

namespace Plugin.Txt2KML;

/// <summary>
/// Interactive, MAUI-only workflow built on <see cref="FilePicker"/> from
/// MAUI Essentials. Lets the user pick a <c>.txt</c> file and converts it to KML.
/// This type is only compiled for the platform targets (the plain <c>net10.0</c>
/// target excludes it).
///
/// <para>
/// Saving the result is left to the consuming app, so the library stays
/// dependency-free beyond MAUI Essentials. The sample app demonstrates a native
/// "save as" dialog using CommunityToolkit.Maui's <c>FileSaver</c>; alternatively
/// use <see cref="Txt2KmlFile.ConvertFile(string, string?, string?)"/> when a
/// destination path is already known.
/// </para>
/// </summary>
public static class Txt2KmlFilePicker
{
    private static readonly FilePickerFileType TextFileType = new(
        new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            [DevicePlatform.MacCatalyst] = new[] { "public.plain-text", "txt" },
            [DevicePlatform.WinUI] = new[] { ".txt" },
            [DevicePlatform.iOS] = new[] { "public.plain-text" },
            [DevicePlatform.Android] = new[] { "text/plain" },
        });

    /// <summary>
    /// Prompts the user to pick a <c>.txt</c> file.
    /// </summary>
    /// <returns>The picked file, or <c>null</c> if the user cancelled.</returns>
    public static Task<FileResult?> PickTextFileAsync()
        => FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Select a text file to convert",
            FileTypes = TextFileType,
        });

    /// <summary>
    /// Prompts the user to pick a <c>.txt</c> file and converts it to KML.
    /// </summary>
    /// <param name="documentName">Optional KML document name; defaults to the source file name.</param>
    /// <returns>The conversion result, or <c>null</c> if the user cancelled.</returns>
    public static async Task<KmlConversionResult?> PickAndConvertAsync(string? documentName = null)
    {
        var file = await PickTextFileAsync().ConfigureAwait(false);
        if (file is null)
            return null;

        using var stream = await file.OpenReadAsync().ConfigureAwait(false);
        using var reader = new StreamReader(stream);
        var text = await reader.ReadToEndAsync().ConfigureAwait(false);

        var sourceName = Path.GetFileNameWithoutExtension(file.FileName);
        var waypoints = Txt2Kml.Parse(text);
        var kml = Txt2Kml.ToKml(waypoints, documentName ?? sourceName);

        return new KmlConversionResult(
            kml,
            sourceName + ".kml",
            waypoints.Count,
            file.FileName);
    }
}
