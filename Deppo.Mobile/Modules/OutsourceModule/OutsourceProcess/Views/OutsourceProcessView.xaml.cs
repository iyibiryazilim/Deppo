using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.ViewModels;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.Views;

public partial class OutsourceProcessView : ContentPage
{
    private readonly OutsourceProcessViewModel _viewModel;

    public OutsourceProcessView(OutsourceProcessViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}