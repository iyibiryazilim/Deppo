using Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.ViewModels;

namespace Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.Views;

public partial class WarehouseCountingFormView : ContentPage
{
	private readonly WarehouseCountingFormViewModel _viewModel;
    public WarehouseCountingFormView(WarehouseCountingFormViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}