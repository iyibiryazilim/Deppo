using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransferV2.ViewModels;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransferV2.Views;

public partial class InputOutsourceTransferV2ProductListView : ContentPage
{
	private readonly InputOutsourceTransferV2ProductListViewModel _viewModel;
	public InputOutsourceTransferV2ProductListView(InputOutsourceTransferV2ProductListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
		_viewModel.SearchText = searchBar;
	}
}