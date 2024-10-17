using Deppo.Mobile.Modules.OutsourceModule.OutsourceMenu.ViewModels;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceMenu.Views;

public partial class OutsourceOutputTransactionView : ContentPage
{
    private readonly OutsourceOutputTransactionViewModel _viewModel;

    public OutsourceOutputTransactionView(OutsourceOutputTransactionViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.SearchText = searchBar;
    }
}