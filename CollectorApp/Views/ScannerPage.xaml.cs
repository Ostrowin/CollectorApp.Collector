using CollectorApp.ViewModels;
using CollectorApp.Helpers;
using ZXing.Net.Maui;

namespace CollectorApp.Views;

public partial class ScannerPage : BasePage
{
	private readonly ScannerViewModel _viewModel;
	public ScannerPage() : this(ServiceLoctor.Get<ScannerViewModel>()) { }
    public ScannerPage(ScannerViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = viewModel;

        BarcodeReader.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.OneDimensional | BarcodeFormats.TwoDimensional,
            AutoRotate = true,
            Multiple = false
        };
    }

	private void BarcodeReader_BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
	{
		var barcode = e.Results.FirstOrDefault();
		if (barcode is null)
			return;

		MainThread.BeginInvokeOnMainThread(async () =>
		{
			await _viewModel.OnBarcodeDetectedAsync(
				barcode.Value,
				barcode.Format.ToString()
			);
		});
    }
}