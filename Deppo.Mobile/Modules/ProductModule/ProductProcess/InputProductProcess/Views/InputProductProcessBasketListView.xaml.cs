using CommunityToolkit.Maui.Core.Platform;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;

public partial class InputProductProcessBasketListView : ContentPage
{
	private readonly InputProductProcessBasketListViewModel _viewModel;
	public InputProductProcessBasketListView(InputProductProcessBasketListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
		//_viewModel.BarcodeEntry = barcodeEntry;
		//_viewModel.BarcodeEntry.Focus();
	}

	//private async void barcodeEntry_Focused(object sender, FocusEventArgs e)
	//{
	//	await barcodeEntry.HideKeyboardAsync(CancellationToken.None);
	//	await barcodeEntry.HideSoftInputAsync(CancellationToken.None);
	//}
}