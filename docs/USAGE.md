# Usage guide

Worked examples for `Plugin.Txt2KML`. All snippets assume:

```csharp
using Plugin.Txt2KML;
```

## 1. Convert a string (sync)

```csharp
string text = """
    -12.4634,130.8456,Darwin,Capital of the NT
    -37.8136,144.9631,Melbourne
    """;

string kml = Txt2Kml.Convert(text, documentName: "Australian cities");
```

`Convert` throws `Txt2KmlFormatException` if any non-comment line is malformed.

## 2. Convert a string (async)

The async methods run the parsing/serialisation off the calling thread, which keeps
the UI responsive for large inputs and supports cancellation.

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
string kml = await Txt2Kml.ConvertAsync(text, "Australian cities", cts.Token);
```

## 3. Convert without exceptions

Prefer the `Try` methods when the input is user-supplied.

```csharp
if (Txt2Kml.TryConvert(text, out string? kml, out Txt2KmlFormatException? error))
{
    // use kml
}
else
{
    Console.WriteLine($"Failed on line {error!.LineNumber}: {error.Message}");
}
```

Async variant returns a tuple:

```csharp
var (success, kml, error) = await Txt2Kml.TryConvertAsync(text);
```

## 4. Work with waypoints directly

```csharp
IReadOnlyList<Waypoint> waypoints = Txt2Kml.Parse(text);

waypoints = waypoints
    .Append(new Waypoint(-31.9523, 115.8613, "Perth"))
    .ToList();

string kml = Txt2Kml.ToKml(waypoints, "Edited set");
```

## 5. Convert a file from a view model

`Txt2KmlFile` has no MAUI dependency, so it can be called from a view model when the
path is already known. The output file name replaces the `.txt` extension with `.kml`.

```csharp
// places.txt -> places.kml (next to the source)
string outputPath = await Txt2KmlFile.ConvertFileAsync(inputPath);

// or specify an explicit destination
string outputPath2 = Txt2KmlFile.ConvertFile(inputPath, @"C:\export\custom.kml");
```

## 6. Pick a file with MAUI Essentials

`Txt2KmlFilePicker` (platform targets only) wraps `FilePicker`.

```csharp
KmlConversionResult? result = await Txt2KmlFilePicker.PickAndConvertAsync();
if (result is not null)
{
    // result.Kml, result.SuggestedFileName, result.WaypointCount, result.SourceFileName
}
```

## 7. Save with a native dialog (app side)

Saving is left to the app so the library stays dependency-free beyond MAUI Essentials.
The sample app uses CommunityToolkit.Maui's `FileSaver`:

```csharp
using System.Text;
using CommunityToolkit.Maui.Storage;

using var stream = new MemoryStream(Encoding.UTF8.GetBytes(result.Kml));
var saveResult = await FileSaver.Default.SaveAsync(result.SuggestedFileName, stream);
string? savedPath = saveResult.IsSuccessful ? saveResult.FilePath : null;
```

## Error handling reference

`Txt2KmlFormatException : FormatException` is thrown by the non-`Try` parsing methods.

| Member | Meaning |
| ------ | ------- |
| `LineNumber` | 1-based line that failed (0 if not line-specific) |
| `Message` | Human-readable description including the line number |

Because it derives from `FormatException`, existing `catch (FormatException)`
handlers continue to work.
