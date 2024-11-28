using Deppo.Mobile.Modules.OutsourceModule.OutsourceMenu.ViewModels;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceMenu.Views;

public partial class OutsourceDetailView : ContentPage
{
    private readonly OutsourceDetailViewModel _viewModel;

    public OutsourceDetailView(OutsourceDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}