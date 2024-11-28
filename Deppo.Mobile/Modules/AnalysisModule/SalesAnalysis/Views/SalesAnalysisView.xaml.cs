using Deppo.Mobile.Modules.AnalysisModule.SalesAnalysis.ViewModels;

namespace Deppo.Mobile.Modules.AnalysisModule.SalesAnalysis.Views;

public partial class SalesAnalysisView : ContentPage
{
	private readonly SalesAnalysisViewModel _viewModel;
	public SalesAnalysisView(SalesAnalysisViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}