using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.Views;

public partial class ProcurementSalesPackageBasketView : ContentPage
{
	private readonly ProcurementSalesPackageBasketViewModel _viewModel;
    public ProcurementSalesPackageBasketView(ProcurementSalesPackageBasketViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;


    }
}