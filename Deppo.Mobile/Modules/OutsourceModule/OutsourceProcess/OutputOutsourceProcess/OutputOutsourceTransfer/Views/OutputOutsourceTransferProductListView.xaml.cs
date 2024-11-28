using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcess.OutputOutsourceTransfer.ViewModels;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcess.OutputOutsourceTransfer.Views;

public partial class OutputOutsourceTransferProductListView : ContentPage
{
	private readonly OutputOutsourceTransferProductListViewModel _viewModel;
	public OutputOutsourceTransferProductListView(OutputOutsourceTransferProductListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
		_viewModel.SearchText = searchBar;
	}
}