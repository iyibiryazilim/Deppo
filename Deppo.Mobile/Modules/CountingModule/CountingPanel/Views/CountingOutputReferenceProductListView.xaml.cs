using Deppo.Mobile.Modules.CountingModule.CountingPanel.ViewModels;

namespace Deppo.Mobile.Modules.CountingModule.CountingPanel.Views;

public partial class CountingOutputReferenceProductListView : ContentPage
{
	private readonly CountingOutputReferenceProductListViewModel _viewModel;
	public CountingOutputReferenceProductListView(CountingOutputReferenceProductListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}