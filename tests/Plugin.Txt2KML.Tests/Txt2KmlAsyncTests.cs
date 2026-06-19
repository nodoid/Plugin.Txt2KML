using Plugin.Txt2KML;

namespace Plugin.Txt2KML.Tests;

public class Txt2KmlAsyncTests
{
    private const string Valid = "-12.4634,130.8456,Darwin\n-37.8136,144.9631,Melbourne";
    private const string Invalid = "-12.4634,130.8456\nnotalatitude,130.0";

    [Fact]
    public async Task ParseAsync_ReturnsSameResultAsSync()
    {
        var sync = Txt2Kml.Parse(Valid);
        var async = await Txt2Kml.ParseAsync(Valid);

        Assert.Equal(sync.Count, async.Count);
        Assert.Equal(sync[0], async[0]);
    }

    [Fact]
    public async Task ConvertAsync_MatchesSyncOutput()
    {
        var sync = Txt2Kml.Convert(Valid, "Doc");
        var async = await Txt2Kml.ConvertAsync(Valid, "Doc");

        Assert.Equal(sync, async);
    }

    [Fact]
    public async Task ToKmlAsync_MatchesSyncOutput()
    {
        var waypoints = Txt2Kml.Parse(Valid);

        var sync = Txt2Kml.ToKml(waypoints, "Doc");
        var async = await Txt2Kml.ToKmlAsync(waypoints, "Doc");

        Assert.Equal(sync, async);
    }

    [Fact]
    public async Task ConvertAsync_InvalidInput_ThrowsTxt2KmlFormatException()
    {
        var ex = await Assert.ThrowsAsync<Txt2KmlFormatException>(
            () => Txt2Kml.ConvertAsync(Invalid));

        Assert.Equal(2, ex.LineNumber);
    }

    [Fact]
    public async Task TryConvertAsync_ValidInput_ReturnsSuccess()
    {
        var (success, kml, error) = await Txt2Kml.TryConvertAsync(Valid);

        Assert.True(success);
        Assert.NotNull(kml);
        Assert.Null(error);
    }

    [Fact]
    public async Task TryConvertAsync_InvalidInput_ReturnsFailureWithoutThrowing()
    {
        var (success, kml, error) = await Txt2Kml.TryConvertAsync(Invalid);

        Assert.False(success);
        Assert.Null(kml);
        Assert.NotNull(error);
        Assert.Equal(2, error!.LineNumber);
    }

    [Fact]
    public async Task ParseAsync_AlreadyCancelledToken_Cancels()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => Txt2Kml.ParseAsync(Valid, cts.Token));
    }
}
