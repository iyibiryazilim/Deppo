using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransfer.ViewModels;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransfer.Views;

public partial class InputOutsourceTransferOutsourceSupplierListView : ContentPage
{
    private readonly InputOutsourceTransferOutsourceSupplierListViewModel _viewModel;

    public InputOutsourceTransferOutsourceSupplierListView(InputOutsourceTransferOutsourceSupplierListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
        _viewModel.SearchText = searchBar;
    }
}