# Plugin.Txt2KML

A .NET MAUI library that converts simple delimited text into KML (Keyhole Markup
Language) documents, for Mac and Windows desktop apps.

## Input format

One waypoint per line, comma-separated:

```
latitude,longitude[,name][,description]
```

- Blank lines and lines beginning with `#` are ignored.
- Whitespace around fields is trimmed.
- Numbers are parsed using the invariant culture.

Example:

```
# latitude,longitude,name,description
-12.4634,130.8456,Darwin,Capital of the Northern Territory
-37.8136,144.9631,Melbourne
-33.8688,151.2093,Sydney,Opera House nearby
```

## Layout

```
src/Plugin.Txt2KML/        The library
samples/Txt2KML.Sample/    MAUI sample app (Mac + Windows)
tests/Plugin.Txt2KML.Tests/ xUnit tests
```

## API

The library multi-targets the MAUI platforms plus a plain `net10.0` target that
carries the dependency-free conversion logic (so it can be referenced by ordinary
.NET consumers such as the test project).

- **`Txt2Kml`** — core conversion (no MAUI dependency)
  - `Parse(text)` → `IReadOnlyList<Waypoint>`
  - `ToKml(waypoints, documentName?)` / `Convert(text, documentName?)` → KML string
- **`Txt2KmlFile`** — file-path helpers, usable directly from a view model
  - `GetKmlPath(path)` — replaces the extension with `.kml`
  - `ConvertFile(path, outputPath?, documentName?)` / `ConvertFileAsync(...)`
- **`Txt2KmlFilePicker`** (MAUI targets only) — MAUI Essentials `FilePicker` workflow
  - `PickTextFileAsync()`, `PickAndConvertAsync(documentName?)`

Saving the result is left to the consuming app so the library only depends on MAUI
Essentials. The sample app demonstrates a native "save as" dialog via
CommunityToolkit.Maui's `FileSaver` (see `samples/Txt2KML.Sample/KmlExporter.cs`).

## Build & test

```bash
dotnet build
dotnet test
```

### Platform notes

- The library cross-builds for Windows from any host (it depends only on MAUI Essentials).
- The **sample app**'s Windows head is only built on a Windows host — a Windows MAUI
  app requires the Windows XAML compiler / WindowsAppSDK tooling.
- The sample uses MAUI `10.0.60` (required by CommunityToolkit.Maui ≥ 14).
