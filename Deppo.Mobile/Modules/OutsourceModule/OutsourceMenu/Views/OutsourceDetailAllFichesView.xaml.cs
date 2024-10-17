using Deppo.Mobile.Modules.OutsourceModule.OutsourceMenu.ViewModels;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceMenu.Views;

public partial class OutsourceDetailAllFichesView : ContentPage
{
    private readonly OutsourceDetailAllFichesViewModel _viewModel;

    public OutsourceDetailAllFichesView(OutsourceDetailAllFichesViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}