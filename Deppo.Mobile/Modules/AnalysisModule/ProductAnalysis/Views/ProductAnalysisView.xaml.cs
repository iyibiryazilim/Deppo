using Deppo.Mobile.Modules.AnalysisModule.ProductAnalysis.ViewModels;

namespace Deppo.Mobile.Modules.AnalysisModule.ProductAnalysis.Views;

public partial class ProductAnalysisView : ContentPage
{
	private readonly ProductAnalysisViewModel _viewModel;
	public ProductAnalysisView(ProductAnalysisViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}