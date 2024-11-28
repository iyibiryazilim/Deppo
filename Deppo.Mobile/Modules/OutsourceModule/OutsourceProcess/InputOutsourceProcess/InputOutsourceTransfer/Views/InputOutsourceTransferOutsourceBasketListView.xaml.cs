using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransfer.ViewModels;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransfer.Views;

public partial class InputOutsourceTransferOutsourceBasketListView : ContentPage
{
    private readonly InputOutsourceTransferOutsourceBasketListViewModel _viewModel;

    public InputOutsourceTransferOutsourceBasketListView(InputOutsourceTransferOutsourceBasketListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}