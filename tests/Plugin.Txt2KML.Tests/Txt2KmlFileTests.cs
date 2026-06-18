using Plugin.Txt2KML;

namespace Plugin.Txt2KML.Tests;

public class Txt2KmlFileTests
{
    [Theory]
    [InlineData("waypoints.txt", "waypoints.kml")]
    [InlineData("/data/routes/trip.txt", "/data/routes/trip.kml")]
    [InlineData("no-extension", "no-extension.kml")]
    [InlineData("archive.tar.txt", "archive.tar.kml")]
    public void GetKmlPath_ReplacesExtensionWithKml(string input, string expected)
    {
        // Normalise separators so the assertion holds on Windows and Unix.
        var actual = Txt2KmlFile.GetKmlPath(input).Replace('\\', '/');
        Assert.Equal(expected.Replace('\\', '/'), actual);
    }

    [Fact]
    public void GetKmlPath_NullOrWhitespace_Throws()
    {
        Assert.Throws<ArgumentException>(() => Txt2KmlFile.GetKmlPath(" "));
    }

    [Fact]
    public void ConvertFile_WritesKmlNextToSourceWithKmlExtension()
    {
        var dir = CreateTempDir();
        try
        {
            var input = Path.Combine(dir, "places.txt");
            File.WriteAllText(input, "-12.4634,130.8456,Darwin\n-37.8136,144.9631,Melbourne");

            var output = Txt2KmlFile.ConvertFile(input);

            Assert.Equal(Path.Combine(dir, "places.kml"), output);
            Assert.True(File.Exists(output));
            Assert.Contains("<kml", File.ReadAllText(output));
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    [Fact]
    public void ConvertFile_HonoursExplicitOutputPath()
    {
        var dir = CreateTempDir();
        try
        {
            var input = Path.Combine(dir, "places.txt");
            File.WriteAllText(input, "-12.46,130.84");
            var explicitOut = Path.Combine(dir, "custom-name.kml");

            var output = Txt2KmlFile.ConvertFile(input, explicitOut);

            Assert.Equal(explicitOut, output);
            Assert.True(File.Exists(explicitOut));
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    [Fact]
    public async Task ConvertFileAsync_WritesKmlFile()
    {
        var dir = CreateTempDir();
        try
        {
            var input = Path.Combine(dir, "async.txt");
            await File.WriteAllTextAsync(input, "-33.8688,151.2093,Sydney");

            var output = await Txt2KmlFile.ConvertFileAsync(input);

            Assert.True(File.Exists(output));
            Assert.EndsWith("async.kml", output);
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    private static string CreateTempDir()
    {
        // Avoid Path.GetRandomFileName-on-temp collisions by using a GUID folder.
        var dir = Path.Combine(Path.GetTempPath(), "txt2kml-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        return dir;
    }
}
