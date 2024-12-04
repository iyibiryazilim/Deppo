using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransferV2.ViewModels;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransferV2.Views;

public partial class InputOutsourceTransferV2MainProductLocationListView : ContentPage
{
	private readonly InputOutsourceTransferV2MainProductLocationListViewModel _viewModel;
	public InputOutsourceTransferV2MainProductLocationListView(InputOutsourceTransferV2MainProductLocationListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
		_viewModel.CurrentPage = this;
		_viewModel.SearchText = locationSearchBar;
	}
}