using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.Views;

public partial class ProcurementByProductFormView : ContentPage
{
	private readonly ProcurementByProductFormViewModel _viewModel;
	public ProcurementByProductFormView(ProcurementByProductFormViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
		_viewModel.CurrentPage = this;
	}
}