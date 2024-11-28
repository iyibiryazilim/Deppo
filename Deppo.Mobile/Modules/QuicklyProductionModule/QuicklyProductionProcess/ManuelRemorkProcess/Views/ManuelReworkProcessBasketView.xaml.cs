using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.ViewModels;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.Views;

public partial class ManuelReworkProcessBasketView : ContentPage
{
	private readonly ManuelReworkProcessBasketViewModel _viewModel;
	public ManuelReworkProcessBasketView(ManuelReworkProcessBasketViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}