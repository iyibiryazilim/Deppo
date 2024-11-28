using Deppo.Mobile.Modules.AnalysisModule.OverviewAnalysis.ViewModels;

namespace Deppo.Mobile.Modules.AnalysisModule.OverviewAnalysis.Views;

public partial class NotificationListView : ContentPage
{
	private readonly NotificationListViewModel _viewModel;
	public NotificationListView(NotificationListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}