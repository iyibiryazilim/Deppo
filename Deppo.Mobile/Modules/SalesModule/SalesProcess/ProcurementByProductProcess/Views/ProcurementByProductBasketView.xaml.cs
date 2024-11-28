using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.Views;

public partial class ProcurementByProductBasketView : ContentPage
{
	private readonly ProcurementByProductBasketViewModel _viewModel;
	public ProcurementByProductBasketView(ProcurementByProductBasketViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}