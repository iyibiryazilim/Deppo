using AndroidX.Lifecycle;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceMenu.ViewModels;
using static Android.App.Assist.AssistStructure;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceMenu.Views;

public partial class OutsourceInputTransactionView : ContentPage
{
    private readonly OutsourceInputTransactionViewModel _viewModel;

    public OutsourceInputTransactionView(OutsourceInputTransactionViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.SearchText = searchBar;
    }
}