using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByLocationProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByLocationProcess.Views;

public partial class ProcurementByLocationCustomerFormView : ContentPage
{
	private readonly ProcurementByLocationCustomerFormViewModel _viewModel;
	public ProcurementByLocationCustomerFormView(ProcurementByLocationCustomerFormViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}