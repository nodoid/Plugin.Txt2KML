# Changelog

All notable changes to this project are documented in this file. The format is
based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [Unreleased]

### Added
- Asynchronous converters: `Txt2Kml.ParseAsync`, `ConvertAsync`, `ToKmlAsync`, and
  `TryConvertAsync`, each accepting a `CancellationToken`.
- Non-throwing `Txt2Kml.TryParse` and `TryConvert`.
- `Txt2KmlFormatException` (derives from `FormatException`) exposing the failing
  `LineNumber`.
- Sample data files `valid-waypoints.txt` and `invalid-waypoints.txt` in the sample
  app, exercised by the unit tests.
- Documentation: expanded README, usage guide, and this changelog.

## [1.0.0]

### Added
- Core text-to-KML conversion (`Txt2Kml`).
- File-path helpers (`Txt2KmlFile`) for view-model use.
- MAUI Essentials `FilePicker` workflow (`Txt2KmlFilePicker`).
- Sample MAUI app (Mac + Windows) with a native save dialog via
  CommunityToolkit.Maui `FileSaver`.
- xUnit test suite.
- NuGet packaging (DILLIGAF license).
