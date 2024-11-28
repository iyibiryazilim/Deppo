using Deppo.Mobile.Modules.AnalysisModule.OverviewAnalysis.ViewModels;

namespace Deppo.Mobile.Modules.AnalysisModule.OverviewAnalysis.Views;

public partial class OverviewOutputTransactionListView : ContentPage
{
	private readonly OverviewOutputTransactionListViewModel _viewModel;
	public OverviewOutputTransactionListView(OverviewOutputTransactionListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}