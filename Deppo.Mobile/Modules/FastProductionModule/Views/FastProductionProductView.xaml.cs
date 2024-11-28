using Deppo.Mobile.Modules.FastProductionModule.ViewModels;

namespace Deppo.Mobile.Modules.FastProductionModule.Views;

public partial class FastProductionProductView : ContentPage
{
	private readonly FastProductionProductViewModel _viewModel;
	public FastProductionProductView(FastProductionProductViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}