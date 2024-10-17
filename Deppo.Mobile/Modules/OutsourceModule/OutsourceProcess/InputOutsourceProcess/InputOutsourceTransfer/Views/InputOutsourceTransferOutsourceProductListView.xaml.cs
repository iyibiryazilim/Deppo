using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransfer.ViewModels;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransfer.Views;

public partial class InputOutsourceTransferOutsourceProductListView : ContentPage
{
    private readonly InputOutsourceTransferOutsourceProductListViewModel _viewModel;

    public InputOutsourceTransferOutsourceProductListView(InputOutsourceTransferOutsourceProductListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.SearchText = searchBar;
        _viewModel.CurrentPage = this;
    }
}