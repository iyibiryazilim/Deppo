using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ViewModels;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Views;

public partial class QuicklyProductionProcessView : ContentPage
{
	private readonly QuicklyProductionProcessViewModel _viewModel;
	public QuicklyProductionProcessView(QuicklyProductionProcessViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage= this;
	}
}