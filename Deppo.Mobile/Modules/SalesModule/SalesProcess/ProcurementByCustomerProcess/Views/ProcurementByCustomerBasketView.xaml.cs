using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.Views;

public partial class ProcurementByCustomerBasketView : ContentPage
{
	private readonly ProcurementByCustomerBasketViewModel _viewModel;
	public ProcurementByCustomerBasketView(ProcurementByCustomerBasketViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}