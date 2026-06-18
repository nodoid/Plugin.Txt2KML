using Plugin.Txt2KML;

namespace Txt2KML.Sample;

public partial class MainPage : ContentPage
{
	private const string SampleInput =
		"# latitude,longitude,name,description\n" +
		"-12.4634,130.8456,Darwin,Capital of the Northern Territory\n" +
		"-37.8136,144.9631,Melbourne\n" +
		"-33.8688,151.2093,Sydney,Opera House nearby";

	private KmlConversionResult? _lastResult;

	public MainPage()
	{
		InitializeComponent();
	}

	private void OnLoadSampleClicked(object? sender, EventArgs e)
	{
		InputEditor.Text = SampleInput;
		StatusLabel.Text = "Loaded sample input.";
	}

	private void OnConvertClicked(object? sender, EventArgs e)
	{
		try
		{
			var waypoints = Txt2Kml.Parse(InputEditor.Text);
			var kml = Txt2Kml.ToKml(waypoints, "Txt2KML Sample");
			OutputEditor.Text = kml;
			_lastResult = new KmlConversionResult(kml, "Txt2KML-Sample.kml", waypoints.Count);
			ExportButton.IsEnabled = true;
			StatusLabel.Text = $"Converted {waypoints.Count} waypoint(s).";
		}
		catch (FormatException ex)
		{
			OutputEditor.Text = string.Empty;
			_lastResult = null;
			ExportButton.IsEnabled = false;
			StatusLabel.Text = ex.Message;
		}
	}

	// Demonstrates the MAUI Essentials FilePicker workflow from the library.
	private async void OnPickFileClicked(object? sender, EventArgs e)
	{
		try
		{
			var result = await Txt2KmlFilePicker.PickAndConvertAsync();
			if (result is null)
			{
				StatusLabel.Text = "Pick cancelled.";
				return;
			}

			_lastResult = result;
			OutputEditor.Text = result.Kml;
			ExportButton.IsEnabled = true;
			StatusLabel.Text = $"Loaded '{result.SourceFileName}' — {result.WaypointCount} waypoint(s). Output: {result.SuggestedFileName}";
		}
		catch (Exception ex)
		{
			StatusLabel.Text = ex.Message;
		}
	}

	private async void OnExportClicked(object? sender, EventArgs e)
	{
		if (_lastResult is null)
			return;

		try
		{
			var savedPath = await KmlExporter.SaveAsync(_lastResult);
			StatusLabel.Text = savedPath is null
				? "Export cancelled."
				: $"Saved to {savedPath}";
		}
		catch (Exception ex)
		{
			StatusLabel.Text = ex.Message;
		}
	}
}
