using Plugin.Txt2KML;

namespace Plugin.Txt2KML.Tests;

/// <summary>
/// Exercises the library against the sample files bundled with the sample app:
/// one valid file that converts cleanly, and one malformed file that must fail
/// in a controlled, catchable way.
/// </summary>
public class SampleFileTests
{
    private static string SamplePath(string name)
        => Path.Combine(AppContext.BaseDirectory, "SampleData", name);

    private static string ReadSample(string name)
        => File.ReadAllText(SamplePath(name));

    [Fact]
    public void ValidSampleFile_ConvertsWithoutThrowing()
    {
        var text = ReadSample("valid-waypoints.txt");

        var waypoints = Txt2Kml.Parse(text);
        var kml = Txt2Kml.ToKml(waypoints, "Valid sample");

        Assert.Equal(6, waypoints.Count);
        Assert.Contains("<Placemark", kml);
    }

    [Fact]
    public void InvalidSampleFile_ThrowsTxt2KmlFormatExceptionWithLineNumber()
    {
        var text = ReadSample("invalid-waypoints.txt");

        var ex = Assert.Throws<Txt2KmlFormatException>(() => Txt2Kml.Parse(text));

        // The non-numeric latitude is on line 4 of the file.
        Assert.Equal(4, ex.LineNumber);
    }

    [Fact]
    public void TryConvert_ValidSampleFile_ReturnsTrue()
    {
        var text = ReadSample("valid-waypoints.txt");

        var ok = Txt2Kml.TryConvert(text, out var kml, out var error);

        Assert.True(ok);
        Assert.Null(error);
        Assert.NotNull(kml);
    }

    [Fact]
    public void TryConvert_InvalidSampleFile_ReturnsFalseWithoutThrowing()
    {
        var text = ReadSample("invalid-waypoints.txt");

        var ok = Txt2Kml.TryConvert(text, out var kml, out var error);

        Assert.False(ok);
        Assert.Null(kml);
        Assert.NotNull(error);
        Assert.Equal(4, error!.LineNumber);
    }
}
