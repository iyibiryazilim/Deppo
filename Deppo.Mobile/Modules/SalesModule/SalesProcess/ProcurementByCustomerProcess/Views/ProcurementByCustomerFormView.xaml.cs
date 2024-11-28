using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.Views;

public partial class ProcurementByCustomerFormView : ContentPage
{
	private readonly ProcurementByCustomerFormViewModel _viewModel;
	public ProcurementByCustomerFormView(ProcurementByCustomerFormViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}