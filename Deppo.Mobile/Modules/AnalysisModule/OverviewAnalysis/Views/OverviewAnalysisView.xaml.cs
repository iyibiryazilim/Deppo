using Deppo.Mobile.Modules.AnalysisModule.OverviewAnalysis.ViewModels;

namespace Deppo.Mobile.Modules.AnalysisModule.OverviewAnalysis.Views;

public partial class OverviewAnalysisView : ContentPage
{
	private readonly OverviewAnalysisViewModel _viewModel;
	public OverviewAnalysisView(OverviewAnalysisViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}