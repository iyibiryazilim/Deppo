using System.Diagnostics;
using Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.Views;

public partial class ProductCaptureImageView : ContentPage
{
	private readonly ProductCaptureImageViewModel _viewModel;
	public ProductCaptureImageView(ProductCaptureImageViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}



	private void cameraView_MediaCaptured(object? sender, CommunityToolkit.Maui.Views.MediaCapturedEventArgs e)
	{
		try
		{
			if (Dispatcher.IsDispatchRequired)
				Dispatcher.Dispatch(() =>
				{
					productImageData.Source = ImageSource.FromStream(() => e.Media);
				});

		}
		catch (System.Exception ex)
		{
			Debug.WriteLine(ex.Message);
		}

	}

	protected override void OnDisappearing()
	{
		cameraView.MediaCaptured -= cameraView_MediaCaptured;
		cameraView.Handler?.DisconnectHandler();

		base.OnDisappearing();

	}

}