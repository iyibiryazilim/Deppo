using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByLocationProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByLocationProcess.Views;

public partial class ProcurementByLocationFormView : ContentPage
{
	private readonly ProcurementByLocationFormViewModel _viewModel;
	public ProcurementByLocationFormView(ProcurementByLocationFormViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}