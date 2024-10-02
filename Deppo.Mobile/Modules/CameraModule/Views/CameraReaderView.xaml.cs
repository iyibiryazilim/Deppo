using Deppo.Mobile.Modules.CameraModule.ViewModels;
using ZXing.Net.Maui;

namespace Deppo.Mobile.Modules.CameraModule.Views;

public partial class CameraReaderView : ContentPage
{
	private readonly CameraReaderViewModel _viewModel;
	public CameraReaderView(CameraReaderViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = viewModel;

		_viewModel.BarcodeReader = barcodeReader;

		_viewModel.BarcodeReader.Options = new BarcodeReaderOptions
		{
			Formats = BarcodeFormats.All,
			Multiple = true,
			AutoRotate = true,
		};
	}

	private void OnBarcodesDetected(object sender, BarcodeDetectionEventArgs e)
	{
		if (IsBusy)
			return;
		
		if (_viewModel.CameraDetectedCommand.CanExecute(e))
		{
			_viewModel.CameraDetectedCommand.Execute(e);
		}
	}
}