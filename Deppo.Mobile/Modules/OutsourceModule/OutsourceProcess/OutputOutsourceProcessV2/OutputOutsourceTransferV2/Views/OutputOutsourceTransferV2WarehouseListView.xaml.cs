using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcessV2.OutputOutsourceTransferV2.ViewModels;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcessV2.OutputOutsourceTransferV2.Views;

public partial class OutputOutsourceTransferV2WarehouseListView : ContentPage
{
	private readonly OutputOutsourceTransferV2WarehouseListViewModel _viewModel;

	public OutputOutsourceTransferV2WarehouseListView(OutputOutsourceTransferV2WarehouseListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
        BindingContext = _viewModel;

    }
}