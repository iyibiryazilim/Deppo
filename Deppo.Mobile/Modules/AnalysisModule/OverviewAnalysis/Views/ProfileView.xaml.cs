using Deppo.Mobile.Modules.AnalysisModule.OverviewAnalysis.ViewModels;
using DevExpress.Data.Async.Helpers;

namespace Deppo.Mobile.Modules.AnalysisModule.OverviewAnalysis.Views;

public partial class ProfileView : ContentPage
{
	private readonly ProfileViewModel _viewModel;
	public ProfileView(ProfileViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
}