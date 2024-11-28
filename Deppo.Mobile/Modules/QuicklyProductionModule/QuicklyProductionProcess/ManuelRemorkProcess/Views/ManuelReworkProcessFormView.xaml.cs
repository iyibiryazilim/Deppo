using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.ViewModels;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.Views;

public partial class ManuelReworkProcessFormView : ContentPage
{
	private readonly ManuelReworkProcessFormViewModel _viewModel;
	public ManuelReworkProcessFormView(ManuelReworkProcessFormViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}