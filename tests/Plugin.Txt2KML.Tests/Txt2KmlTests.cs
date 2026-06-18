using System.Xml.Linq;
using Plugin.Txt2KML;

namespace Plugin.Txt2KML.Tests;

public class Txt2KmlTests
{
    private static readonly XNamespace Kml = "http://www.opengis.net/kml/2.2";

    [Fact]
    public void Parse_NullOrEmpty_ReturnsEmpty()
    {
        Assert.Empty(Txt2Kml.Parse(null));
        Assert.Empty(Txt2Kml.Parse(""));
        Assert.Empty(Txt2Kml.Parse("   \n  \n"));
    }

    [Fact]
    public void Parse_SkipsBlankAndCommentLines()
    {
        const string text = "# a header comment\n\n-12.46,130.84\n   # indented comment\n-37.81,144.96\n";

        var waypoints = Txt2Kml.Parse(text);

        Assert.Equal(2, waypoints.Count);
    }

    [Fact]
    public void Parse_LatLonOnly_PopulatesCoordinates()
    {
        var wp = Assert.Single(Txt2Kml.Parse("-12.4634,130.8456"));

        Assert.Equal(-12.4634, wp.Latitude);
        Assert.Equal(130.8456, wp.Longitude);
        Assert.Null(wp.Name);
        Assert.Null(wp.Description);
    }

    [Fact]
    public void Parse_NameAndDescription_AreCaptured()
    {
        var wp = Assert.Single(Txt2Kml.Parse("-33.8688,151.2093,Sydney,Harbour city, with commas"));

        Assert.Equal("Sydney", wp.Name);
        // Everything after the third comma is treated as the description.
        Assert.Equal("Harbour city, with commas", wp.Description);
    }

    [Fact]
    public void Parse_TrimsWhitespaceAroundFields()
    {
        var wp = Assert.Single(Txt2Kml.Parse("  -12.46 , 130.84 ,  Darwin  "));

        Assert.Equal(-12.46, wp.Latitude);
        Assert.Equal("Darwin", wp.Name);
    }

    [Theory]
    [InlineData("not-a-number,130.0")]
    [InlineData("91.0,130.0")]   // latitude out of range
    [InlineData("-12.0,200.0")]  // longitude out of range
    [InlineData("-12.0")]        // missing longitude
    public void Parse_InvalidLine_Throws(string text)
    {
        // Txt2KmlFormatException derives from FormatException, so existing
        // catch (FormatException) handlers continue to work.
        var ex = Assert.Throws<Txt2KmlFormatException>(() => Txt2Kml.Parse(text));
        Assert.IsAssignableFrom<FormatException>(ex);
    }

    [Fact]
    public void ToKml_ProducesWellFormedDocumentWithCorrectNamespace()
    {
        var kml = Txt2Kml.Convert("-12.4634,130.8456,Darwin", "My Doc");

        var doc = XDocument.Parse(kml);
        Assert.Equal(Kml + "kml", doc.Root!.Name);
        Assert.Equal("My Doc", doc.Root.Element(Kml + "Document")!.Element(Kml + "name")!.Value);
    }

    [Fact]
    public void ToKml_UsesLonLatAltCoordinateOrder()
    {
        var kml = Txt2Kml.Convert("-12.4634,130.8456,Darwin");

        var doc = XDocument.Parse(kml);
        var coordinates = doc.Descendants(Kml + "coordinates").Single().Value;

        // KML order is longitude,latitude,altitude.
        Assert.Equal("130.8456,-12.4634,0", coordinates);
    }

    [Fact]
    public void ToKml_EmitsOnePlacemarkPerWaypoint()
    {
        var kml = Txt2Kml.Convert("-12.46,130.84\n-37.81,144.96\n-33.86,151.20");

        var doc = XDocument.Parse(kml);
        Assert.Equal(3, doc.Descendants(Kml + "Placemark").Count());
    }

    [Fact]
    public void ToKml_EscapesSpecialCharactersInNames()
    {
        var kml = Txt2Kml.ToKml(new[] { new Waypoint(0, 0, "Tom & Jerry <test>") });

        // Round-trips through XML parsing without error and preserves the raw value.
        var doc = XDocument.Parse(kml);
        Assert.Equal("Tom & Jerry <test>", doc.Descendants(Kml + "name").Single().Value);
    }
}
