using Deppo.Mobile.Modules.AnalysisModule.OverviewAnalysis.ViewModels;

namespace Deppo.Mobile.Modules.AnalysisModule.OverviewAnalysis.Views;

public partial class OverviewInputTransactionListView : ContentPage
{
	private readonly OverviewInputTransactionListViewModel _viewModel;
	public OverviewInputTransactionListView(OverviewInputTransactionListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}