using Deppo.Mobile.Modules.AnalysisModule.PurchaseAnalysis.ViewModels;

namespace Deppo.Mobile.Modules.AnalysisModule.PurchaseAnalysis.Views;

public partial class PurchaseAnalysisView : ContentPage
{
	private readonly PurchaseAnalysisViewModel _viewModel;
	public PurchaseAnalysisView(PurchaseAnalysisViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}