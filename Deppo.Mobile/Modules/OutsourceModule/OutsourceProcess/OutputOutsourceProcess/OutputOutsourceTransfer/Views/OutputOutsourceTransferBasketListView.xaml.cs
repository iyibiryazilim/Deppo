using CommunityToolkit.Maui.Core.Platform;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcess.OutputOutsourceTransfer.ViewModels;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcess.OutputOutsourceTransfer.Views;

public partial class OutputOutsourceTransferBasketListView : ContentPage
{
	private readonly OutputOutsourceTransferBasketListViewModel _viewModel;
	public OutputOutsourceTransferBasketListView(OutputOutsourceTransferBasketListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
		_viewModel.BarcodeEntry = barcodeEntry;
		_viewModel.BarcodeEntry.Focus();
	}
}