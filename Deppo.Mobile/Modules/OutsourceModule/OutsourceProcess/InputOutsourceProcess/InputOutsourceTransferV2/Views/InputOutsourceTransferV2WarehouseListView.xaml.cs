using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransferV2.ViewModels;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransferV2.Views;

public partial class InputOutsourceTransferV2WarehouseListView : ContentPage
{
	private readonly InputOutsourceTransferV2WarehouseListViewModel _viewModel;
	public InputOutsourceTransferV2WarehouseListView(InputOutsourceTransferV2WarehouseListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}
}