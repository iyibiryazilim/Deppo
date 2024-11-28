using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransfer.ViewModels;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransfer.Views;

public partial class InputOutsourceTransferBasketLocationListView : ContentPage
{
    private readonly InputOutsourceTransferBasketLocationListViewModel _viewModel;

    public InputOutsourceTransferBasketLocationListView(InputOutsourceTransferBasketLocationListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
        _viewModel.SearchText = locationSearchBar;
    }
}