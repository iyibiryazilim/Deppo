using Deppo.Mobile.Modules.ProductModule.ProductProcess.DemandProcess.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.DemandProcess.Views;

public partial class DemandProcessFormView : ContentPage
{
	private readonly DemandProcessFormViewModel _viewModel;
    public DemandProcessFormView(DemandProcessFormViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}