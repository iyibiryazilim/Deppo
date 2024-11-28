using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransfer.ViewModels;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransfer.Views;

public partial class InputOutsourceTransferWarehouseListView : ContentPage
{
    private readonly InputOutsourceTransferWarehouseListViewModel _viewModel;

    public InputOutsourceTransferWarehouseListView(InputOutsourceTransferWarehouseListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}