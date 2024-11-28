using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcess.OutputOutsourceTransfer.ViewModels;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcess.OutputOutsourceTransfer.Views;

public partial class OutputOutsourceTransferWarehouseListView : ContentPage
{
	private readonly OutputOutsourceTransferWarehouseListViewModel _viewModel;
	public OutputOutsourceTransferWarehouseListView(OutputOutsourceTransferWarehouseListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}