# Plugin.Txt2KML

A .NET MAUI library that converts simple delimited text into KML (Keyhole Markup
Language) documents, for Mac and Windows desktop apps.

[![NuGet](https://img.shields.io/badge/nuget-Plugin.Txt2KML-blue)](https://www.nuget.org/packages/Plugin.Txt2KML)

## Install

```bash
dotnet add package Plugin.Txt2KML
```

Targets `net10.0` (dependency-free core) plus `net10.0-android`, `net10.0-ios`,
`net10.0-maccatalyst`, and `net10.0-windows10.0.19041.0`.

## Input format

One waypoint per line, comma-separated:

```
latitude,longitude[,name][,description]
```

- Blank lines and lines beginning with `#` are ignored.
- Whitespace around fields is trimmed.
- Numbers are parsed using the invariant culture.
- Latitude must be in `-90..90`, longitude in `-180..180`.

Example:

```
# latitude,longitude,name,description
-12.4634,130.8456,Darwin,Capital of the Northern Territory
-37.8136,144.9631,Melbourne
-33.8688,151.2093,Sydney,Opera House nearby
```

## Quick start

```csharp
using Plugin.Txt2KML;

// Synchronous
string kml = Txt2Kml.Convert("-12.4634,130.8456,Darwin", documentName: "Places");

// Asynchronous (work runs off the calling thread)
string kml2 = await Txt2Kml.ConvertAsync("-12.4634,130.8456,Darwin", "Places");

// Non-throwing
if (Txt2Kml.TryConvert(text, out var kml3, out var error))
    Save(kml3);
else
    Console.WriteLine($"Line {error!.LineNumber}: {error.Message}");
```

## API

The library multi-targets the MAUI platforms plus a plain `net10.0` target that
carries the dependency-free conversion logic (so it can be referenced by ordinary
.NET consumers such as the test project).

### `Txt2Kml` — core conversion (no MAUI dependency)

| Sync | Async | Notes |
| ---- | ----- | ----- |
| `Parse(text)` | `ParseAsync(text, ct?)` | → `IReadOnlyList<Waypoint>` |
| `Convert(text, documentName?)` | `ConvertAsync(text, documentName?, ct?)` | → KML string |
| `ToKml(waypoints, documentName?)` | `ToKmlAsync(waypoints, documentName?, ct?)` | → KML string |
| `TryParse(text, out waypoints, out error)` | — | non-throwing |
| `TryConvert(text, out kml, out error, documentName?)` | `TryConvertAsync(text, documentName?, ct?)` | non-throwing; async returns `(bool Success, string? Kml, Txt2KmlFormatException? Error)` |

The throwing methods raise **`Txt2KmlFormatException`** (derives from
`FormatException`) which exposes the 1-based `LineNumber` of the offending line.

### `Txt2KmlFile` — file-path helpers (no MAUI dependency)

Usable directly from a view model when a file path is already known.

- `GetKmlPath(path)` — replaces the extension with `.kml` (`trip.txt` → `trip.kml`)
- `ConvertFile(path, outputPath?, documentName?)` / `ConvertFileAsync(...)` — reads
  the text file, converts, writes the `.kml`, and returns the output path.

### `Txt2KmlFilePicker` — MAUI Essentials workflow (platform targets only)

- `PickTextFileAsync()` — prompts for a `.txt` file
- `PickAndConvertAsync(documentName?)` — picks a file and converts it to a
  `KmlConversionResult`

Saving is left to the consuming app so the library only depends on MAUI Essentials.
The sample app demonstrates a native "save as" dialog via CommunityToolkit.Maui's
`FileSaver` (see `samples/Txt2KML.Sample/KmlExporter.cs`).

See [docs/USAGE.md](docs/USAGE.md) for worked examples.

## Layout

```
src/Plugin.Txt2KML/         The library
samples/Txt2KML.Sample/     MAUI sample app (Mac + Windows)
  Resources/Raw/Samples/    valid-waypoints.txt / invalid-waypoints.txt
tests/Plugin.Txt2KML.Tests/ xUnit tests
```

## Build & test

```bash
dotnet build
dotnet test
```

### Platform notes

- The **library** cross-builds for Windows from any host (it depends only on MAUI
  Essentials).
- The **sample app**'s Windows head must be built on Windows — a Windows MAUI app
  requires the Windows XAML compiler / WindowsAppSDK tooling. On macOS the sample
  builds the Mac Catalyst head; on Windows it builds the Windows head.
- The sample uses MAUI `10.0.60` (required by CommunityToolkit.Maui ≥ 14).
- Windows verification is done on a local Parallels Windows VM (no hosted CI).

## License

[DILLIGAF](LICENSE) © Paul F. Johnson
