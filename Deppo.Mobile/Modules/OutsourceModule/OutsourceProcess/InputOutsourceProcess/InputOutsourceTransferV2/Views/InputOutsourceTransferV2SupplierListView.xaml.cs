using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransferV2.ViewModels;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransferV2.Views;

public partial class InputOutsourceTransferV2SupplierListView : ContentPage
{
	private readonly InputOutsourceTransferV2SupplierListViewModel _viewModel;
	public InputOutsourceTransferV2SupplierListView(InputOutsourceTransferV2SupplierListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
		_viewModel.SearchText = searchBar;
	}
}